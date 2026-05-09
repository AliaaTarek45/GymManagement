using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace GymManagementDAL.Repositories.Interfaces
{
	public interface IGenericRepository<TEntity> where TEntity : BaseEntity
	{
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,bool tracking = false,CancellationToken ct = default);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,bool tracking = false,CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
        void Add(TEntity entity);
		void Update(TEntity entity);
		void Delete(TEntity entity);
	}
}
