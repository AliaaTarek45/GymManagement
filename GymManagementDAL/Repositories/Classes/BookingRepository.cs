using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementDAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<BookingEntity>, IBookingRepository
	{
		private readonly GymDbContext _dbContext;

		public BookingRepository(GymDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

        public Task<List<BookingEntity>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default)
            => _dbContext.Bookings.AsNoTracking().Include(b => b.Member).Where(b => b.SessionId == sessionId).ToListAsync(ct);

      
    }
}
