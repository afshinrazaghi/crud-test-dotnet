using Amazon.Runtime.Internal;
using Ardalis.Result;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Features.Customers.Queries
{
    public class GetAllCustomerQuery : IRequest<Result<IEnumerable<CustomerQueryModel>>>
    {
    }
}
