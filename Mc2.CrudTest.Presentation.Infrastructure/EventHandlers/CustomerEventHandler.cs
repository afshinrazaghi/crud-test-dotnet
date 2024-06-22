using AutoMapper;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Application.Models;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.EventHandlers
{
    public class CustomerEventHandler : INotificationHandler<CustomerCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly ISynchronizeDb _synchronizeDb;
        private readonly ILogger<CustomerEventHandler> _logger;

        public CustomerEventHandler(IMapper mapper, ISynchronizeDb synchronizeDb, ILogger<CustomerEventHandler> logger)
        {
            _mapper = mapper;
            _synchronizeDb = synchronizeDb;
            _logger = logger;
        }

        public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
        {
            LogEvent(notification);

            var customerQueryModel = _mapper.Map<CustomerQueryModel>(notification);
            await _synchronizeDb.UpsertAsync(customerQueryModel, filter => filter.Id == customerQueryModel.Id);
        }


        private void LogEvent<TEvent>(TEvent @event) where TEvent : CustomerBaseEvent =>
            _logger.LogInformation("----- Triggering the event {EventName}, model: {EventModel}", typeof(TEvent).Name, @event.ToJson());
    }
}
