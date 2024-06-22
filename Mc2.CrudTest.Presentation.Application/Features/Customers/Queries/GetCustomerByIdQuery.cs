using Ardalis.Result;
using Mc2.CrudTest.Presentation.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Features.Customers.Queries
{
    public class GetCustomerByIdQuery : IRequest<Result<CustomerQueryModel>>
    {
        public Guid Id { get; }
        public GetCustomerByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
