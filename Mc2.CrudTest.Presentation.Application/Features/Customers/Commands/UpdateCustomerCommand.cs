using Ardalis.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Features.Customers.Commands
{
    public class UpdateCustomerCommand : IRequest<Result>
    {
        [Required]
        [MaxLength(200)]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(200)]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(13)]
        [DataType(DataType.Text)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(250)]
        [DataType(DataType.EmailAddress)]
        public string OriginalEmail { get; set; }

        [Required]
        [MaxLength(250)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        [DataType(DataType.Text)]
        public string BankAccountNumber { get; set; }
    }
}
