using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Mc2.CrudTest.Presentation.Shared.Extensions;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly WriteDbContext _writeDbContext;
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(WriteDbContext writeDbContext, IEventStoreRepository eventStoreRepository, IMediator mediator, ILogger<UnitOfWork> logger)
        {
            _writeDbContext = writeDbContext;
            _eventStoreRepository = eventStoreRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task SaveChangesAsync()
        {
            var strategy = _writeDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _writeDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

                _logger.LogInformation("----- Begin transaction: '{TransactionId}'", transaction.TransactionId);

                try
                {
                    var (domainEvents, eventStores) = BeforeSaveChanges();

                    var rowsAffected = await _writeDbContext.SaveChangesAsync();

                    _logger.LogInformation("----- Commit transaction: '{TransactionId}'", transaction.TransactionId);
                    await transaction.CommitAsync();

                    await AfterSaveChangesAsync(domainEvents, eventStores);

                    _logger.LogInformation(
                        "----- Transaction successfully confirmed: '{TransactionId}', Rows Affected: {RowsAffected}", transaction.TransactionId, rowsAffected);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An unexpected exception occurred while committing the transaction : '{TransactionId}', message:{Message}",
                        transaction.TransactionId, ex.Message);

                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }




        private (IReadOnlyList<BaseEvent> domainEvents, IReadOnlyList<EventStore> eventStores) BeforeSaveChanges()
        {
            var domainEntities = _writeDbContext
                .ChangeTracker
                .Entries<BaseEntity>()
                .Where(entry => entry.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            var eventStores = domainEvents
                .ConvertAll(@event => new EventStore(@event.AggregateId, @event.GetGenericTypeName(), @event.ToJson()));

            domainEntities.ForEach(entry => entry.Entity.ClearDomainEvents());

            return (domainEvents.AsReadOnly(), eventStores.AsReadOnly());
        }

        private async Task AfterSaveChangesAsync(
            IReadOnlyList<BaseEvent> domainEvents,
            IReadOnlyList<EventStore> eventStores)
        {
            if (!domainEvents.Any() && !eventStores.Any())
                return;


            var tasks = domainEvents
                .AsParallel()
                .Select(@event => _mediator.Publish(@event))
                .ToList();

            await Task.WhenAll(tasks);

            await _eventStoreRepository.StoreAsync(eventStores);
        }


        #region IDisposable

        private bool _disposed;

        ~UnitOfWork() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed) return;

            if (disposing)
            {
                _writeDbContext.Dispose();
                _eventStoreRepository.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }
}
