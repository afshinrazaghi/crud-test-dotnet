using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence.Common
{
    public abstract class BaseWriteOnlyRepository<TEntity, TKey> : IWriteOnlyRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly DbSet<TEntity> _dbSet;
        protected readonly WriteDbContext Context;

        public BaseWriteOnlyRepository(WriteDbContext context)
        {
            Context = context;
            _dbSet = Context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        => _dbSet.Add(entity);



        public async Task<IEnumerable<TEntity>> GetAllAsync()
        => await _dbSet.AsNoTrackingWithIdentityResolution().ToListAsync();

        public async Task<TEntity?> GetByIdAsync(TKey id)
        => await _dbSet.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(entity => entity.Id.Equals(id));

        public void Remove(TEntity entity)
        => _dbSet.Remove(entity);

        public void Update(TEntity entity)
        => _dbSet.Update(entity);

        #region IDisposable

        private bool _disposed;

        ~BaseWriteOnlyRepository() => Dispose(false);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                Context.Dispose();

            _disposed = true;
        }
        #endregion
    }
}
