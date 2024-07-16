using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence
{
    public class CustomerWriteOnlyRepository : BaseWriteOnlyRepository<Customer, Guid>, ICustomerWriteOnlyRepository
    {
        public CustomerWriteOnlyRepository(WriteDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsAsync(string firstName, string lastName, DateTime dateOfBirth)
        => Context.Customers
            .AnyAsync(customer => customer.FirstName == firstName && customer.LastName == lastName && customer.DateOfBirth == dateOfBirth);

        public Task<bool> ExistsAsync(string firstName, string lastName, DateTime dateOfBirth, Guid currentId)
            => Context.Customers.AnyAsync(customer => customer.FirstName == firstName && customer.LastName == lastName && customer.DateOfBirth == dateOfBirth && customer.Id != currentId);

        public Task<bool> ExistsAsync(string firstName, string lastName, DateTime dateOfBirth, Email currentEmail)
            => Context.Customers.AnyAsync(customer => customer.FirstName == firstName && customer.LastName == lastName && customer.DateOfBirth == dateOfBirth && customer.Email.Value != currentEmail.Value);

        public Task<bool> ExistsByEmailAsync(Email email)
        => Context.Customers.AnyAsync(customer => customer.Email.Value == email.Value);

        public Task<bool> ExistsByEmailAsync(Email email, Guid currentId)
            => Context.Customers.AnyAsync(customer => customer.Email.Value == email.Value && customer.Id != currentId);

        public Task<Customer?> GetByEmailAsync(Email email)
            => Context.Customers.FirstOrDefaultAsync(customer => customer.Email.Value == email.Value);

        public async Task UpdateCustomer(Email email, Customer customer)
        {
            var dbCustomer = await Context.Customers.FirstOrDefaultAsync(c => c.Email.Value == email.Value);
            Update(dbCustomer);
        }
    }
}
