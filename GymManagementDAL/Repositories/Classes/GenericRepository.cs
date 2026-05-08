using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace GymManagementDAL.Repositories.Classes
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
	{
		private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _set;

        public GenericRepository(GymDbContext dbContext)
		{
			_dbContext = dbContext;
            _set = dbContext.Set<TEntity>();
        }
        public void Add(TEntity entity) => _set.Add(entity);
        public void Update(TEntity entity) => _set.Update(entity);
        public void Delete(TEntity entity) => _set.Remove(entity);


        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
                                                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                                        bool tracking = false,
                                                        CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            if (include is not null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate, ct);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => _set.AsNoTracking().AnyAsync(predicate, ct);

        public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? _set.AsNoTracking().CountAsync(ct) : _set.AsNoTracking().CountAsync(predicate, ct);
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,bool tracking = false,CancellationToken ct = default)
        {
            IQueryable <TEntity> query = tracking ? _set : _set.AsNoTracking();
            if (include is not null) query = include(query);
            if (predicate is not null) query = query.Where(predicate);
            return await query.ToListAsync(ct);
        }
        public Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default) => _set.FindAsync([id], ct).AsTask();

    }
}
