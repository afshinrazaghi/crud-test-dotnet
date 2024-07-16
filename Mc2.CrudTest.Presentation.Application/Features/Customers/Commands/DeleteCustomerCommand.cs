using Ardalis.Result;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Features.Customers.Commands
{
    public class DeleteCustomerCommand : IRequest<Result>
    {
        public DeleteCustomerCommand()
        {

        }
        public DeleteCustomerCommand(string email)
        {
            Email = email;
        }
        public string Email { get; set; }
    }
}
