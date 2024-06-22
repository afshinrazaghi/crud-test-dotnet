using Ardalis.Result;
using Mc2.CrudTest.Presentation.Domain.ValueObjects.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.ValueObjects
{
    public class Email
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
                return Result<Email>.Error(validationResult.Errors.First().ErrorMessage);
            return Result<Email>.Success(new Email(email));
        }
    }
}
