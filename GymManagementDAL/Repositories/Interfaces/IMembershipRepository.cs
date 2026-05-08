using GymManagementDAL.Entities;
using System.Linq.Expressions;

namespace GymManagementDAL.Repositories.Interfaces
{
	public interface IMembershipRepository : IGenericRepository<MembershipEntity>
	{
        Task<List<MembershipEntity>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<MembershipEntity, bool>>? predicate = null,CancellationToken ct = default);
    }
}
