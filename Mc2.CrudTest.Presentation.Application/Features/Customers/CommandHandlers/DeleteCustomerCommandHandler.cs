using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Features.Customers.CommandHandlers
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>
    {
        private readonly ICustomerWriteOnlyRepository _repository;
        private readonly IValidator<DeleteCustomerCommand> _validator;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerCommandHandler(IValidator<DeleteCustomerCommand> validator, ICustomerWriteOnlyRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var customer = await _repository.GetByIdAsync(request.Id);
            if (customer is null)
                return Result.NotFound($"No customer found by Id : {request.Id}");

            customer.Delete();

            _repository.Remove(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result.SuccessWithMessage("Successfully removed!");
        }
    }
}
