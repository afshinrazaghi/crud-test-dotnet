using FluentValidation;
using FluentValidation.Validators;
using Mc2.CrudTest.Presentation.Domain.ValueObjects.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate
{
    public class CustomerValidator : AbstractValidator<Customer>
    {

        public CustomerValidator()
        {
            RuleFor(c => c.FirstName)
                .NotNull()
                .NotEmpty()
                .WithMessage("FirstName cannot be empty");

            RuleFor(c => c.LastName)
                .NotNull()
                .NotEmpty()
                .WithMessage("LastName cannot be empty");

            RuleFor(c => c.DateOfBirth)
                .NotNull()
                .NotEmpty()
                .WithMessage("BirthOfDate cannot be empty");

            //RuleFor(c => c.PhoneNumber)
            //    .SetValidator(new PhoneNumberValidator());

            //RuleFor(c => c.Email)
            //    .SetValidator(new EmailValidator());

            //RuleFor(c => c.BankAccountNumber)
            //    .SetValidator(new BankAccountNumberValidator());


        }
    }
}
