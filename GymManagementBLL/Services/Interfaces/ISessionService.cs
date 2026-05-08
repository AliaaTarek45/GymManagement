using GymManagementBLL.Common;
using GymManagementBLL.ViewModels.SessionViewModels;

namespace GymManagementBLL.Services.Interfaces
{
	public interface ISessionService
	{
        Task<IReadOnlyList<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default);
        Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default);
        Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);
        Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default);
        Task<IReadOnlyList<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default);
        Task<IReadOnlyList<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default);
	}
}
