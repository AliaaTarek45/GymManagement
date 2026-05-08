using GymManagementBLL.Common;
using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementBLL.ViewModels.MembershipViewModels;
using GymManagementBLL.ViewModels.SessionViewModels;

namespace GymManagementBLL.Services.Interfaces
{
	public interface IBookingService
	{
        Task<IReadOnlyList<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<MemberForSessionViewModel>> GetMembersForUpcomingBySessionIdAsync(int sessionId, CancellationToken ct = default);
        Task<IReadOnlyList<MemberForSessionViewModel>> GetMembersForOngoingBySessionIdAsync(int sessionId, CancellationToken ct = default);
        Task<IReadOnlyList<MemberSelectListViewModel>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct = default);

        Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default);
        Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default);
    }
}
