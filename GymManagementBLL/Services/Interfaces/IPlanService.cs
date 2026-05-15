using GymManagementBLL.Common;
using GymManagementBLL.ViewModels.PlanViewModels;

namespace GymManagementBLL.Services.Interfaces
{
	public interface IPlanService
	{
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);

        Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default);
        Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
    }
}
