using AutoMapper;
using GymManagementBLL.Common;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.TrainerViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;

namespace GymManagementBLL.Services.Classes
{
	public class TrainerService : ITrainerService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public TrainerService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<TrainerEntity>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
        }
        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<TrainerEntity>().GetByIdAsync(trainerId, ct);
            return trainer is null ? null : _mapper.Map<TrainerViewModel>(trainer);
        }
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<TrainerEntity>();

            if (await repo.AnyAsync(t => t.Email == model.Email, ct))
                return Result.Fail("A trainer with this email already exists.");
            if (await repo.AnyAsync(t => t.Phone == model.Phone, ct))
                return Result.Fail("A trainer with this phone number already exists.");

            var entity = _mapper.Map<TrainerEntity>(model);
            repo.Add(entity);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Trainer");
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<TrainerEntity>().GetByIdAsync(trainerId, ct);
            return trainer is null ? null : _mapper.Map<TrainerToUpdateViewModel>(trainer);
        }
        public async Task<Result> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<TrainerEntity>();
            var trainer = await repo.GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer not found.");

            var hasFutureSessions = await _unitOfWork.GetRepository<SessionEntity>()
                .AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now, ct);
            if (hasFutureSessions)
                return Result.Fail("Cannot delete a trainer with upcoming sessions.");

            repo.Delete(trainer);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();
        }
  
        public async Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<TrainerEntity>();
            var trainer = await repo.GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer not found.");

            // BUG FIX from original: original code checked against MemberEntity (wrong table).
            if (await repo.AnyAsync(t => t.Email == model.Email && t.Id != trainerId, ct))
                return Result.Fail("Another trainer is already using this email.");
            if (await repo.AnyAsync(t => t.Phone == model.Phone && t.Id != trainerId, ct))
                return Result.Fail("Another trainer is already using this phone number.");

            _mapper.Map(model, trainer);
			trainer.UpdatedAt = DateTime.Now;
            repo.Update(trainer);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() :  Result.Fail("Failed To Update Trainer");
        }

	}
}
