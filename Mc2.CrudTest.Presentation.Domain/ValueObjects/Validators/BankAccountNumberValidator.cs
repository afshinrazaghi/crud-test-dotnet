using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.ValueObjects.Validators
{
    public class BankAccountNumberValidator : AbstractValidator<string>
    {
        public BankAccountNumberValidator()
        {
            RuleFor(ban => ban)
                .NotEmpty()
                .WithMessage("Back Account Number cannot be empty");

            RuleFor(ban => ban)
                .Matches("^[0-9]{2}\\s[0-9]{2}\\s[0-9]{2}\\s[0-9]{3}$")
                .WithMessage("Back Account Number is not valid");
        }
    }
}
