using GymManagementBLL.Common;
using GymManagementBLL.ViewModels.TrainerViewModels;

namespace GymManagementBLL.Services.Interfaces
{
	public interface ITrainerService
	{
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default);
        Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default);
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);
        Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default);
        Task<Result> RemoveTrainerAsync(int trainerId, CancellationToken ct = default);
	}
}
