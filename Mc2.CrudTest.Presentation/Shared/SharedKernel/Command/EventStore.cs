using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.SharedKernel.Command
{
    public class EventStore : BaseEvent
    {
        public EventStore(Guid aggregationId, string messageType, string data)
        {
            AggregateId = aggregationId;
            MessageType = messageType;
            Data = data;
        }

        public EventStore()
        {

        }

        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Data { get; private set; }
    }
}
