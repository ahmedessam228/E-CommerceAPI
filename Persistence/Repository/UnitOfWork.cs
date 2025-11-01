using Domain.Interfaces;
using Domain.Models;
using Persistence.Data;

namespace Persistence.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<string, object> _repositories = [];
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var typeName = typeof(TEntity).Name;

            if (_repositories.ContainsKey(typeName))
                return (IGenericRepository < TEntity, TKey >) _repositories[typeName];

            var repo = new GenericRepository<TEntity, TKey>(_context);
            _repositories[typeName] = repo;
            return repo;
        }
    }
}
