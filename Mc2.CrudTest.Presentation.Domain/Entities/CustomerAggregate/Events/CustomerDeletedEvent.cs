using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Events
{
    public class CustomerDeletedEvent : CustomerBaseEvent
    {
        public CustomerDeletedEvent(Guid id, string firstName, string lastName, DateTime dateOfBirth, string phoneNumber, string email, string bankAccountNumber)
           : base(id, firstName, lastName, dateOfBirth, PhoneNumber.Create(phoneNumber).Value, Email.Create(email).Value, BankAccountNumber.Create(bankAccountNumber).Value)
        {

        }
    }
}
