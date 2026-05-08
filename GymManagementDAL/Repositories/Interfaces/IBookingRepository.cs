using GymManagementDAL.Entities;

namespace GymManagementDAL.Repositories.Interfaces
{
	public interface IBookingRepository : IGenericRepository<BookingEntity>
	{
        public Task<List<BookingEntity>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default);

    }
}
