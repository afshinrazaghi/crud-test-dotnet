using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Responses;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Domain.Factories;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Features.Customers.CommandHandlers
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CreatedCustomerResponse>>
    {
        private readonly IValidator<CreateCustomerCommand> _validator;
        private readonly ICustomerWriteOnlyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCustomerCommandHandler(IValidator<CreateCustomerCommand> validator, ICustomerWriteOnlyRepository repository, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreatedCustomerResponse>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<CreatedCustomerResponse>.Invalid(validationResult.AsErrors());
            }

            var phoneNumber = PhoneNumber.Create(request.PhoneNumber).Value;
            var email = Email.Create(request.Email).Value;
            var bankAccountNumber = BankAccountNumber.Create(request.BankAccountNumber).Value;

            if (await _repository.ExistsByEmailAsync(email))
                return Result<CreatedCustomerResponse>.Error("email already exists");

            if (await _repository.ExistsAsync(request.FirstName, request.LastName, request.DateOfBirth))
                return Result<CreatedCustomerResponse>.Error("customer already registered");

            var customer = CustomerFactory.Create(
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                phoneNumber,
                email,
                bankAccountNumber);

            _repository.Add(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<CreatedCustomerResponse>.Success(
                new CreatedCustomerResponse(customer.Id), "Successfully registered!");
        }
    }
}
