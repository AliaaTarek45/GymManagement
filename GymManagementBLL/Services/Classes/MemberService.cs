using AutoMapper;
using GymManagementBLL.Common;
using GymManagementBLL.Services.AttachmentService;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModel;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagementBLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;
        private readonly ILogger<MemberService> _logger;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService, ILogger<MemberService> logger)

        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _logger = logger;

        }
        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<MemberEntity>();

            if (await repo.AnyAsync(m => m.Email == model.Email, ct))
                return Result.Fail("A member with this email already exists.");
            if (await repo.AnyAsync(m => m.Phone == model.Phone, ct))
                return Result.Fail("A member with this phone number already exists.");

            var photo = await _attachmentService.UploadAsync(model.PhotoFile, "MembersPictures", ct);
            if (string.IsNullOrEmpty(photo))
                return Result.Validation("Profile photo upload failed (check file type and size).");

            var entity = _mapper.Map<MemberEntity>(model);
            entity.Photo = photo;

            repo.Add(entity);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Member");
        }
        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<MemberEntity>().GetAllAsync(ct: ct);
            return _mapper.Map<List<MemberViewModel>>(members);
        }
        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<MemberEntity>().GetByIdAsync(memberId, ct);

            if (member is null) return null;

            var viewModel = _mapper.Map<MemberViewModel>(member);

            var activeMembership = (await _unitOfWork.GetRepository<MembershipEntity>().GetAllAsync(MP => MP.MemberId == memberId
                 && MP.EndDate >= DateTime.Now, ct: ct)).FirstOrDefault();

            if (activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<PlanEntity>().GetByIdAsync(activeMembership.PlanId, ct);

                viewModel.PlanName = activePlan?.Name;
                viewModel.MembershipStartDate = activeMembership.CreatedAt.ToShortDateString();
                viewModel.MembershipEndDate = activeMembership.EndDate.ToShortDateString();
            }

            return viewModel;
        }
        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecordEntity>().FirstOrDefaultAsync(x => x.MemberId == memberId, ct: ct);
            return record is null ? null : _mapper.Map<HealthRecordViewModel>(record);
        }
        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<MemberEntity>().GetByIdAsync(memberId, ct);
            return member is null ? null : _mapper.Map<MemberToUpdateViewModel>(member);
        }
        public async Task<Result> RemoveMemberAsync(int memberId, CancellationToken ct = default)
        {
            var memberRepo = _unitOfWork.GetRepository<MemberEntity>();
            var member = await memberRepo.GetByIdAsync(memberId, ct);
            if (member is null) return Result.NotFound("Member not found.");


            var hasFutureSessions = await _unitOfWork.BookingRepository.AnyAsync(b => b.MemberId == memberId && b.Session.StartDate > DateTime.Now);

            if (hasFutureSessions)
                return Result.Fail("Cannot delete a member with upcoming sessions.");

            var memberships = await _unitOfWork.MembershipRepository
                .GetAllAsync(m => m.MemberId == memberId, tracking: true, ct: ct);
            foreach (var ms in memberships)
                _unitOfWork.MembershipRepository.Delete(ms);

            memberRepo.Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            if (result > 0)
            {
                if (!string.IsNullOrEmpty(member.Photo))
                    _attachmentService.Delete(member.Photo, "members");

                return Result.Ok();
            }
            return Result.Fail("Failed To Delete Member");
        }
        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<MemberEntity>();
            var member = await repo.GetByIdAsync(id, ct);
            if (member is null) return Result.NotFound("Member not found.");

            // Self-exclusion: check if email/phone exists on a DIFFERENT member.
            if (await repo.AnyAsync(m => m.Email == model.Email && m.Id != id, ct))
                return Result.Fail("Another member is already using this email.");
            if (await repo.AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct))
                return Result.Fail("Another member is already using this phone number.");

            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;
            repo.Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To update Member");
        }

    }
}
