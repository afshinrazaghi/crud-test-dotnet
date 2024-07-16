using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Abstractions;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Application.Models;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Persistence.Common;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Query.Persistence
{
    public class CustomerReadOnlyRepository : BaseReadOnlyRepository<CustomerQueryModel, Guid>, ICustomerReadOnlyRepository
    {
        public CustomerReadOnlyRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }

        public async Task<IEnumerable<CustomerQueryModel>> GetAllAsync()
        {
            var sort = Builders<CustomerQueryModel>.Sort
                .Ascending(customer => customer.FirstName)
                .Descending(customer => customer.DateOfBirth);

            var findOptions = new FindOptions<CustomerQueryModel>
            {
                Sort = sort
            };

            using var asyncCursor = await Collection.FindAsync(Builders<CustomerQueryModel>.Filter.Empty, findOptions);
            return await asyncCursor.ToListAsync();
        }

        public async Task<CustomerQueryModel?> GetByEmailAsync(string email)
        {

            using var asyncCursor = await Collection.FindAsync(Builders<CustomerQueryModel>.Filter.Eq(x => x.Email, email));
            return await asyncCursor.FirstOrDefaultAsync();
        }


    }
}
