using AutoMapper;
using Mc2.CrudTest.Presentation.Application.Models;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Profiles
{
    public class EventToQueryModelProfile : Profile
    {
        public EventToQueryModelProfile()
        {
            CreateMap<CustomerCreatedEvent, CustomerQueryModel>(MemberList.Destination)
                .ConstructUsing(@event => CreateCustomerQueryModel(@event));

            CreateMap<CustomerUpdatedEvent, CustomerQueryModel>(MemberList.Destination)
                .ConstructUsing(@event => CreateCustomerQueryModel(@event));

            CreateMap<CustomerDeletedEvent, CustomerQueryModel>(MemberList.Destination)
                .ConstructUsing(@event => CreateCustomerQueryModel(@event));
        }


        public override string ProfileName => nameof(EventToQueryModelProfile);

        private static CustomerQueryModel CreateCustomerQueryModel<TEvent>(TEvent @event)
            where TEvent : CustomerBaseEvent =>
            new CustomerQueryModel(@event.Id, @event.FirstName, @event.LastName, @event.DateOfBirth, @event.PhoneNumber.Value, @event.Email.Value, @event.BankAccountNumber.Value);
    }

}
