using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Mc2.CrudTest.Presentation.Domain.ValueObjects.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }
        public Email() { }
        private Email(string value)
        {
            Value = value.ToLowerInvariant().Trim();
        }

        public static Result<Email> Create(string email)
        {
            var emailValidator = new EmailValidator();
            var validationResult = emailValidator.Validate(email);
            if (!validationResult.IsValid)
                return Result<Email>.Invalid(validationResult.AsErrors());
            return Result<Email>.Success(new Email(email));
        }

        public override string ToString() => Value;
    }
}
