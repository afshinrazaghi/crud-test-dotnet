using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence
{
    internal sealed class EventStoreRepository : IEventStoreRepository
    {
        private readonly EventStoreDbContext _context;
        public EventStoreRepository(EventStoreDbContext context)
        {
            _context = context;
        }

        public async Task StoreAsync(IEnumerable<EventStore> eventStores)
        {
            await _context.EventStores.AddRangeAsync(eventStores);
            await _context.SaveChangesAsync();
        }

        #region IDisposable
        private bool _disposed;

        ~EventStoreRepository() => Dispose(false);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
                return;

            if (disposing)
                _context.Dispose();

            _disposed = true;
        }
        #endregion
    }
}
