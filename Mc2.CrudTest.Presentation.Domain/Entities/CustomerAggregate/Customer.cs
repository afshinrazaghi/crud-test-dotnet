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
        public Customer()
        {
            
        }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Email Email { get; private set; }
        public BankAccountNumber BankAccountNumber { get; private set; }
    }
}
