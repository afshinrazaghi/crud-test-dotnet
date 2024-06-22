using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Query;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Query.Persistence.Common
{
    public class BaseReadOnlyRepository<TQueryModel, TKey> : IReadOnlyRepository<TQueryModel, TKey>
        where TQueryModel : IQueryModel<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IReadDbContext _readDbContext;
        protected readonly IMongoCollection<TQueryModel> Collection;
        public BaseReadOnlyRepository(IReadDbContext readDbContext)
        {
            _readDbContext = readDbContext;
            Collection = readDbContext.GetCollection<TQueryModel>();
        }

        public async Task<TQueryModel> GetByIdAsync(TKey id)
        {
            using var asyncCursor = await Collection.FindAsync(queryModel => queryModel.Id.Equals(id));
            return await asyncCursor.FirstOrDefaultAsync();
        }
    }
}
