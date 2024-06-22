using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
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
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerQueryModel>>
    {
        private readonly ICustomerReadOnlyRepository _customerRepository;
        private readonly IValidator<GetCustomerByIdQuery> _validator;
        public GetCustomerByIdQueryHandler(ICustomerReadOnlyRepository customerRepository, IValidator<GetCustomerByIdQuery> validator)
        {
            _customerRepository = customerRepository;
            _validator = validator;
        }

        public async Task<Result<CustomerQueryModel>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Result<CustomerQueryModel>.Invalid(validationResult.AsErrors());
            }

            var customer = await _customerRepository.GetByIdAsync(request.Id);

            return customer == null
                ? Result<CustomerQueryModel>.NotFound($"No customer found by Id : {request.Id}")
                : Result<CustomerQueryModel>.Success(customer);
        }
    }
}
