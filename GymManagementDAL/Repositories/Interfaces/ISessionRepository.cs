using GymManagementDAL.Entities;
using System.Linq.Expressions;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<SessionEntity>
    {

        Task<IEnumerable<SessionEntity>> GetAllSessionsWithTrainerAndCategoryAsync(
       Expression<Func<SessionEntity, bool>>? predicate = null,
       CancellationToken ct = default);

        Task<SessionEntity?> GetSessionWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default);

        Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default);
    }
}
