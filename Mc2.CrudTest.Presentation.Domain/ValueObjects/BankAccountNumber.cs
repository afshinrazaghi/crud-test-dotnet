using Ardalis.Result;
using Mc2.CrudTest.Presentation.Domain.ValueObjects.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.ValueObjects
{
    public class BankAccountNumber
    {
        public string Value { get; }

        public BankAccountNumber() { }
        private BankAccountNumber(string value)
        {
            Value = value;
        }

        public static Result<BankAccountNumber> Create(string bankAccountNumber)
        {
            var bankAccountNumberValidator = new BankAccountNumberValidator();
            var validationResult = bankAccountNumberValidator.Validate(bankAccountNumber);
            if (!validationResult.IsValid)
                return Result<BankAccountNumber>.Error(validationResult.Errors.First().ErrorMessage);
            return Result<BankAccountNumber>.Success(new BankAccountNumber(bankAccountNumber));
        }
    }
}
