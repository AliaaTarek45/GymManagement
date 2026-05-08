using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace GymManagementDAL.Repositories.Interfaces
{
	public interface IUnitOfWork
	{
		public IMembershipRepository MembershipRepository { get; }
		public ISessionRepository SessionRepository { get; }
		public IBookingRepository BookingRepository { get; }
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);

    }
}
