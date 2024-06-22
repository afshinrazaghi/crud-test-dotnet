using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.SharedKernel.Command
{
    public class BaseEntity : IEntity<Guid>
    {
        private readonly List<BaseEvent> _domainEvents = new();

        protected BaseEntity() => Id = Guid.NewGuid();

        protected BaseEntity(Guid id) => Id = id;

        public IEnumerable<BaseEvent> DomainEvents =>
            _domainEvents.AsReadOnly();

        public Guid Id { get; private init; }

        protected void AddDomainEvent(BaseEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() =>
            _domainEvents.Clear();


    }
}
