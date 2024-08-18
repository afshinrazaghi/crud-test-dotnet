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
    public class GetCustomerByEmailQueryHandler : IRequestHandler<GetCustomerByEmailQuery, Result<CustomerQueryModel?>>
    {
        private readonly ICustomerReadOnlyRepository _repository;

        public GetCustomerByEmailQueryHandler(ICustomerReadOnlyRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<CustomerQueryModel?>> Handle(GetCustomerByEmailQuery request, CancellationToken cancellationToken)
        {
            return Result<CustomerQueryModel?>.Success(await _repository.GetByEmailAsync(request.Email));
        }
    }
}
