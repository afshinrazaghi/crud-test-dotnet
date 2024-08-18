using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate
{
    public interface ICustomerWriteOnlyRepository : IWriteOnlyRepository<Customer, Guid>
    {
        Task<bool> ExistsByEmailAsync(Email email);
        Task<bool> ExistsByEmailAsync(Email email, Guid currentId);
        Task<bool> ExistsAsync(string firstName, string lastName, DateTime dateOfBirth);
        Task<bool> ExistsAsync(string firstName, string lastName, DateTime dateOfBirth, Guid currentId);
        Task<bool> ExistsAsync(string firstName, string lastName, DateTime dateOfBirth, Email currentEmail);
        Task<Customer?> GetByEmailAsync(Email email);
        Task UpdateCustomer(Email email, Customer customer);
    }
}
