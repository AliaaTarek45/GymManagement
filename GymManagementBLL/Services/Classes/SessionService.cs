using AutoMapper;
using GymManagementBLL.Common;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;

namespace GymManagementBLL.Services.Classes
{
	public class SessionService(IUnitOfWork unitOfWork, IMapper mapper) : ISessionService
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly IMapper _mapper = mapper;

        public async Task<IReadOnlyList<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
			var sessions = (await _unitOfWork.SessionRepository.GetAllAsync(ct: ct)).OrderByDescending(X => X.StartDate); ;

			if (sessions?.Any() ?? true) return null;

			var MappedSessions = _mapper.Map<IReadOnlyList<SessionViewModel>>(sessions);

			foreach (var session in MappedSessions)
			{
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
			}
			return MappedSessions;

		}
        public async Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryAsync(sessionId, ct);

			if (session == null)
				return null;

			var MappedSession = _mapper.Map<SessionEntity, SessionViewModel>(session);
			MappedSession.AvailableSlots = MappedSession.Capacity - ( await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct));
			return MappedSession;
		}
        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.GetRepository<SessionEntity>().GetByIdAsync(sessionId, ct);
            if (session is null) return null;
            if (!await IsSessionValidForUpdatingAsync(session, ct)) return null;
            return _mapper.Map<UpdateSessionViewModel>(session);
        }
        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate)
                return Result.Validation("End date must be after the start date.");
            if (model.StartDate <= DateTime.Now)
                return Result.Validation("Start date must be in the future.");

            var trainerExists = await _unitOfWork.GetRepository<TrainerEntity>()
                .AnyAsync(t => t.Id == model.TrainerId, ct);
            if (!trainerExists) return Result.NotFound("Trainer not found.");

            var categoryExists = await _unitOfWork.GetRepository<CategoryEntity>()
                .AnyAsync(c => c.Id == model.CategoryId, ct);
            if (!categoryExists) return Result.NotFound("Category not found.");

            var entity = _mapper.Map<SessionEntity>(model);
            _unitOfWork.GetRepository<SessionEntity>().Add(entity);
           var result =  await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok(): Result.Fail("Failed To Create Session");
        }
        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<SessionEntity>();
            var session = await repo.GetByIdAsync(id, ct);
            if (session is null) return Result.NotFound("Session not found.");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot edit a session that has already started.");

            var bookedCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bookedCount > 0)
                return Result.Fail("Cannot edit a session that already has bookings.");

            if (model.EndDate <= model.StartDate)
                return Result.Validation("End date must be after the start date.");
            if (model.StartDate <= DateTime.Now)
                return Result.Validation("Start date must be in the future.");

            var trainerExists = await _unitOfWork.GetRepository<TrainerEntity>()
                .AnyAsync(t => t.Id == model.TrainerId, ct);
            if (!trainerExists) return Result.NotFound("Trainer not found.");

            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            repo.Update(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Update Session");
        }
        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<SessionEntity>();
            var session = await repo.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session not found.");

            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Cannot delete a session that has not yet ended.");

            var bookedCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookedCount > 0)
                return Result.Fail("Cannot delete a session that has bookings.");

            repo.Delete(session);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();
        }
        public async Task<IReadOnlyList<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<TrainerEntity>().GetAllAsync(ct: ct);
			return _mapper.Map<IReadOnlyList<TrainerSelectViewModel>>(trainers);
		}

        public async Task<IReadOnlyList<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.GetRepository<CategoryEntity>().GetAllAsync(ct: ct);
            return _mapper.Map<List<CategorySelectViewModel>>(categories); ;
        }

    
        #region Helper Methods
        private async Task<bool> IsSessionValidForUpdatingAsync(SessionEntity session, CancellationToken ct = default)
        {
            if (session.StartDate <= DateTime.Now) return false;
            var booked = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            return booked == 0;
        }
		#endregion
	}
}