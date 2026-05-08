using GymManagementBLL.Common;
using GymManagementBLL.ViewModels.MembershipViewModels;

namespace GymManagementBLL.Services.Interfaces
{
	public interface IMembershipService
	{
        Task<IReadOnlyList<MemberShipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default);
        Task<IReadOnlyList<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default);
        Task<Result> CreateMembershipAsync(CreateMemberShipViewModel model, CancellationToken ct = default);
        Task<Result> DeleteActiveMembershipAsync(int memberId, CancellationToken ct = default);

    }
}
