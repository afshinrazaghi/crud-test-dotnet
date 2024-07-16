using Mc2.CrudTest.Presentation.Shared.SharedKernel.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Models
{
    public class CustomerQueryModel : IQueryModel<Guid>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string BankAccountNumber { get; set; }

        public string FullName => (FirstName + " " + LastName).Trim();

        public CustomerQueryModel()
        {

        }

        public CustomerQueryModel(Guid id, string firstName, string lastName, DateTime dateOfBirth, string phoneNumber, string email, string bankAccountNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
            BankAccountNumber = bankAccountNumber;
        }


        public override bool Equals(object? obj)
        {
            var item = obj as CustomerQueryModel;
            if (item == null) return false;
            return item.FirstName == FirstName && item.LastName == LastName && item.DateOfBirth == DateOfBirth &&
                item.PhoneNumber == PhoneNumber && item.Email == Email && item.BankAccountNumber == BankAccountNumber;
        }

    }
}
