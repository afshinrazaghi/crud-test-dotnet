using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.SharedKernel.Command
{
    public class BaseEvent : INotification
    {
        public string MessageType { get; protected init; }
        public Guid AggregateId { get; protected init; }
        public DateTime OccurredOn { get; private set; }
    }
}
