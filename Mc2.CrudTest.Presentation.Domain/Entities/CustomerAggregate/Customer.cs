using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Events;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate
{
    public partial class Customer : BaseEntity, IAggregateRoot
    {
        private bool _idDeleted;

        public Customer()
        {

        }
        public Customer(string firstName, string lastName, DateTime dateOfBirth, PhoneNumber phoneNumber, Email email, BankAccountNumber bankAccountNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
            BankAccountNumber = bankAccountNumber;

            AddDomainEvent(new CustomerCreatedEvent(Id, firstName, lastName, DateOfBirth, phoneNumber.Value, email.Value, bankAccountNumber.Value));
        }

        public void ChangeDetail(string firstName, string lastName, DateTime dateOfBirth, PhoneNumber phoneNumber, Email email, BankAccountNumber bankAccountNumber)
        {
            if (firstName.Equals(FirstName) && lastName.Equals(LastName) && dateOfBirth.Equals(DateOfBirth) && phoneNumber.Equals(PhoneNumber) && email.Equals(Email) && bankAccountNumber.Equals(BankAccountNumber))
                return;

            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
            BankAccountNumber = bankAccountNumber;

            AddDomainEvent(new CustomerUpdatedEvent(Id, firstName, lastName, dateOfBirth, phoneNumber.Value, email.Value, bankAccountNumber.Value));
        }

        public void Delete()
        {
            if (_idDeleted)
                return;

            _idDeleted = true;
            AddDomainEvent(new CustomerDeletedEvent(Id, FirstName, LastName, DateOfBirth, PhoneNumber.Value, Email.Value, BankAccountNumber.Value));
        }
        
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Email Email { get; private set; }
        public BankAccountNumber BankAccountNumber { get; private set; }
    }
}
