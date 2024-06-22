using Ardalis.Result;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Abstractions;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Queries;
using Mc2.CrudTest.Presentation.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Features.Customers.QueryHandlers
{
    public class GetAllCustomerQueryHandler :
        IRequestHandler<GetAllCustomerQuery, Result<IEnumerable<CustomerQueryModel>>>
    {
        private readonly ICustomerReadOnlyRepository _repository;

        public GetAllCustomerQueryHandler(ICustomerReadOnlyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<IEnumerable<CustomerQueryModel>>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
        {
            return Result<IEnumerable<CustomerQueryModel>>.Success(await _repository.GetAllAsync());
        }
    }
}
