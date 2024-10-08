﻿using AutoMapper;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Application.Models;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mc2.CrudTest.Presentation.Shared.Extensions;

namespace Mc2.CrudTest.Presentation.Infrastructure.EventHandlers
{
    public class CustomerEventHandler : INotificationHandler<CustomerCreatedEvent>, INotificationHandler<CustomerUpdatedEvent>, INotificationHandler<CustomerDeletedEvent>
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

            CustomerQueryModel customerQueryModel = _mapper.Map<CustomerQueryModel>(notification);
            await _synchronizeDb.UpsertAsync(customerQueryModel, filter => filter.Id == customerQueryModel.Id);
        }

        public async Task Handle(CustomerUpdatedEvent notification, CancellationToken cancellationToken)
        {
            LogEvent(notification);

            CustomerQueryModel customerQueryModel = _mapper.Map<CustomerQueryModel>(notification);
            await _synchronizeDb.UpsertAsync(customerQueryModel, filter => filter.Id == customerQueryModel.Id);
        }

        public async Task Handle(CustomerDeletedEvent notification, CancellationToken cancellationToken)
        {
            LogEvent(notification);
            await _synchronizeDb.DeleteAsync<CustomerQueryModel>(filter => filter.Email == notification.Email.Value);
        }

        private void LogEvent<TEvent>(TEvent @event) where TEvent : CustomerBaseEvent =>
            _logger.LogInformation("----- Triggering the event {EventName}, model: {EventModel}", typeof(TEvent).Name, @event.ToJson());
    }
}
