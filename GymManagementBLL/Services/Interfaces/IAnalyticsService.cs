using GymManagementBLL.ViewModels.AnalyticsViewModels;

namespace GymManagementBLL.Services.Interfaces
{
	public interface IAnalyticsService
	{
        Task<AnalyticsViewModel> GetAnalyticsDataAsync(CancellationToken ct = default);
    }
}
