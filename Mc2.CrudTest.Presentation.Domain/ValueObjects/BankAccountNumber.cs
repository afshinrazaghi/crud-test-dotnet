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
    public sealed record BankAccountNumber
    {
        public string Value { get; }

        public BankAccountNumber() { }
        private BankAccountNumber(string value)
        {
            Value = value;
        }

        public static Result<BankAccountNumber> Create(string bankAccountNumber)
        {
            BankAccountNumberValidator bankAccountNumberValidator = new BankAccountNumberValidator();
            FluentValidation.Results.ValidationResult validationResult = bankAccountNumberValidator.Validate(bankAccountNumber);
            if (!validationResult.IsValid)
                return Result<BankAccountNumber>.Invalid(validationResult.AsErrors());
            return Result<BankAccountNumber>.Success(new BankAccountNumber(bankAccountNumber));
        }

        public override string ToString() => Value;
    }
}
