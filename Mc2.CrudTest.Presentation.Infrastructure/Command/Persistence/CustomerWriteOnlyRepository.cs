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

        public Task<bool> ExistsByEmailAsync(Email email)
        => Context.Customers.AnyAsync(customer => customer.Email.Value == email.Value);

        public Task<bool> ExistsByEmailAsync(Email email, Guid currentId)
      => Context.Customers.AnyAsync(customer => customer.Email.Value == email.Value && customer.Id != currentId);
    }
}
