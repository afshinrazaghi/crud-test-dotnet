using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Features.Customers.Commands
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(customer => customer.OriginalEmail)
                .NotEmpty()
                .MaximumLength(250)
                .EmailAddress();

            RuleFor(customer => customer.FirstName)
               .NotEmpty()
               .MaximumLength(200);

            RuleFor(customer => customer.LastName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(customer => customer.DateOfBirth)
                .NotEmpty();

            RuleFor(customer => customer.PhoneNumber)
                .NotEmpty()
                .MaximumLength(13);

            RuleFor(customer => customer.Email)
                .NotEmpty()
                .MaximumLength(250)
                .EmailAddress();

            RuleFor(customer => customer.BankAccountNumber)
                .NotEmpty()
                .MaximumLength(20);
                //.Matches("^[0-9]{2}\\s[0-9]{2}\\s[0-9]{2}\\s[0-9]{3}$");
        }
    }
}
