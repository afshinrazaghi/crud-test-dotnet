using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.SharedKernel.Command
{
    public interface IWriteOnlyRepository<TEntity, in TKey> : IDisposable
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        void Add(TEntity entity);

        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
    }
}
