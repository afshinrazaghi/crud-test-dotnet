﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.SharedKernel.Command
{
    public interface IEventStoreRepository : IDisposable
    {
        Task StoreAsync(IEnumerable<EventStore> eventStores);
    }
}
