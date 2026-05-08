using AutoMapper;
using GymManagementBLL.Common;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementBLL.ViewModels.MembershipViewModels;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementBLL.Services.Classes
{
    public class BookingService : IBookingService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper ,ILogger<BookingService> logger)
        {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
            _logger = logger;

        }
        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session not found.");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot cancel a booking for a session that has already started.");

            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.SessionId == sessionId && b.MemberId == memberId, tracking: true, ct: ct);
            if (booking is null) return Result.NotFound("Booking not found.");

            _unitOfWork.BookingRepository.Delete(booking);
          var result =   await _unitOfWork.SaveChangesAsync(ct);
            return  result > 0 ? Result.Ok() : Result.Fail("Booking Cancel Failed");
        }
        public async Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync( b => b.MemberId == memberId && b.SessionId == sessionId, tracking: true, ct: ct);
            if (booking is null) return Result.NotFound("Booking not found.");

            booking.IsAttended = true;
			booking.UpdatedAt = DateTime.Now;
            _unitOfWork.BookingRepository.Update(booking);

           var result =  await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Mark As Attended") ;
        }
        public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId, ct);
            if (session is null) return Result.NotFound("Session not found.");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot book a session that has already started.");

            var hasActiveMembership = await _unitOfWork.MembershipRepository
                .AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, ct);
            if (!hasActiveMembership)
                return Result.Fail("Member does not have an active membership.");

            // Prevent double-booking the same member into the same session.
            var alreadyBooked = await _unitOfWork.BookingRepository
                .AnyAsync(b => b.SessionId == model.SessionId && b.MemberId == model.MemberId, ct);
            if (alreadyBooked)
                return Result.Fail("Member is already booked for this session.");

            var booked = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(model.SessionId, ct);
            if (booked >= session.Capacity)
                return Result.Fail("Session is full.");

            _unitOfWork.BookingRepository.Add(new BookingEntity
            {
                MemberId = model.MemberId,
                SessionId = model.SessionId,
                IsAttended = false,
                CreatedAt = DateTime.Now,
            });

          var result =  await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Book Session");
        }
        public async Task<IReadOnlyList<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {

            var bookings =  await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(x => x.EndDate >= DateTime.Now);
            if (bookings.Count == 0) return null!;
            var MappedSession = _mapper.Map<IReadOnlyList<SessionViewModel>>(bookings);
            foreach (var item in MappedSession)
            {
                item.AvailableSlots =  item.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(item.Id); 
            }
            return MappedSession;
        }
        public async Task<IReadOnlyList<MemberForSessionViewModel>> GetMembersForUpcomingBySessionIdAsync(
         int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = sessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            }).ToList();
        }
        public async Task<IReadOnlyList<MemberForSessionViewModel>> GetMembersForOngoingBySessionIdAsync(
         int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = sessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                IsAttended = b.IsAttended,
            }).ToList();
        }
        public async Task<IReadOnlyList<MemberSelectListViewModel>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository
                                             .GetAllAsync(x => x.SessionId == sessionId);

            var bookedMemberIds = booking.Select(x => x.MemberId);

            var availableMembers =  await _unitOfWork.GetRepository<MemberEntity>()
                                              .GetAllAsync(x => !bookedMemberIds.Contains(x.Id));

            return _mapper.Map<IReadOnlyList<MemberSelectListViewModel>>(availableMembers);
        }
    }
}
