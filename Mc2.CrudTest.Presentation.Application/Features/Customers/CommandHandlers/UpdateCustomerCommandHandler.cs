using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
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
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result>
    {
        private readonly ICustomerWriteOnlyRepository _repository;
        private readonly IValidator<UpdateCustomerCommand> _validator;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerCommandHandler(IValidator<UpdateCustomerCommand> validator, ICustomerWriteOnlyRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Invalid(validationResult.AsErrors());

            var customer = await _repository.GetByEmailAsync(Email.Create(request.OriginalEmail).Value);
            if (customer == null)
                return Result.NotFound($"No customer found by Email : {request.OriginalEmail}");

            var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);
            if (!phoneNumberResult.IsSuccess)
                return Result.Error(new ErrorList(phoneNumberResult.Errors.ToArray()));

            var emailResult = Email.Create(request.Email);
            if (!emailResult.IsSuccess)
                return Result.Error(new ErrorList(emailResult.Errors.ToArray()));

            var bankAccountNumberResult = BankAccountNumber.Create(request.BankAccountNumber);
            if (!bankAccountNumberResult.IsSuccess)
                return Result.Error(new ErrorList(bankAccountNumberResult.Errors.ToArray()));

            if (request.OriginalEmail != request.Email)
            {
                if (await _repository.ExistsByEmailAsync(emailResult.Value))
                    return Result.Error("email already exists");
            }

            if (await _repository.ExistsAsync(request.FirstName, request.LastName, request.DateOfBirth, Email.Create(request.OriginalEmail).Value))
                return Result.Error("customer with target info already exists");

            customer.ChangeDetail(request.FirstName, request.LastName, request.DateOfBirth, phoneNumberResult.Value, emailResult.Value, bankAccountNumberResult.Value);

            await _repository.UpdateCustomer(Email.Create(request.OriginalEmail).Value, customer);
            await _unitOfWork.SaveChangesAsync();

            return Result.SuccessWithMessage("Updated successfully!");

        }
    }
}
