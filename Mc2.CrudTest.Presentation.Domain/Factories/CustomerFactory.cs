using Ardalis.Result;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.Factories
{
    public static class CustomerFactory
    {
        public static Result<Customer> Create(string firstName, string lastName, DateTime dateOfBirth,
            string phoneNumber, string email, string bankAccountNumber)
        {
            Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(phoneNumber);
            if (!phoneNumberResult.IsSuccess)
                return Result<Customer>.Error(new ErrorList(phoneNumberResult.Errors));

            Result<Email> emailResult = Email.Create(email);
            if (!emailResult.IsSuccess)
                return Result<Customer>.Error(new ErrorList(emailResult.Errors));

            Result<BankAccountNumber> bankAccountNumberResult = BankAccountNumber.Create(bankAccountNumber);
            if (!bankAccountNumberResult.IsSuccess)
                return Result<Customer>.Error(new ErrorList(bankAccountNumberResult.Errors));

            return Result<Customer>.Success(new Customer(firstName, lastName, dateOfBirth, phoneNumberResult.Value, emailResult.Value, bankAccountNumberResult.Value));
        }

        public static Customer Create(string firstName, string lastName, DateTime dateOfBirth, PhoneNumber phoneNumber, Email email, BankAccountNumber bankAccountNumber)
            => new Customer(firstName, lastName, dateOfBirth, phoneNumber, email, bankAccountNumber);

    }
}
