using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Common.Interfaces.Abstractions
{
    public interface ICustomerReadOnlyRepository : IReadOnlyRepository<CustomerQueryModel, Guid>
    {
        Task<IEnumerable<CustomerQueryModel>> GetAllAsync();
    }


}
