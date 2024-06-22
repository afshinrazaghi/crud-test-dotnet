using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Features.Customers.Responses
{
    public class CreatedCustomerResponse : IResponse
    {
        public CreatedCustomerResponse(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
