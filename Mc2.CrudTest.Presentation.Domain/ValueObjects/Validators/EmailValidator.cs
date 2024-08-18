using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.ValueObjects.Validators
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(email => email)
                .NotEmpty()
                .WithMessage("Email cannot be empty");

            When(email => !string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email), () => {
                RuleFor(email => email).EmailAddress()
                .WithMessage("Email is not valid");
            });
        }
    }
}
