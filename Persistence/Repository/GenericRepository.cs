
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System.Linq.Expressions;

namespace Persistence.Repository
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> 
        where TEntity : BaseEntity<TKey>

    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity) => 
            await _dbSet.AddAsync(entity);
        public async Task AddRangeAsync(IEnumerable<TEntity> entities) => 
            await _dbSet.AddRangeAsync(entities);
        public void  Update(TEntity entity) => 
            _dbSet.Update(entity);
        public void  UpdateRangeAsync(IEnumerable<TEntity> entities) =>  
            _dbSet.UpdateRange(entities);
        public void  RemoveAsync(TEntity entity) => 
            _dbSet.Remove(entity);
        public void RemoveRangeAsync(IEnumerable<TEntity> entities) =>  
            _dbSet.RemoveRange(entities);
        public async Task<TEntity?> GetByIdAsync(TKey id) => 
            await _dbSet.FindAsync(id);
        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            bool trackChanges = false, 
            int? pageNumber = null,
            int? pageSize = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = trackChanges ? _dbSet : _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includes is {Length: > 0 })
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }
            return await query.ToListAsync();
        }

        public IQueryable<TEntity> GetAllQueryable(
            Expression<Func<TEntity, bool>>? predicate = null,
            int? pageNumber = null,
            int? pageSize = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includes is { Length: > 0 })
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }
            return query;
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null, 
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includes is { Length: > 0 })
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.AnyAsync(predicate);
        }
        public async Task<TEntity> FindBy(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.CountAsync();
        }

    }

}
