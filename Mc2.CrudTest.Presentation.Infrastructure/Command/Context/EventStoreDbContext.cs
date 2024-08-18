using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Infrastructure.Command.DatabaseConfigurations;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.Context
{
    public class EventStoreDbContext : BaseDbContext<EventStoreDbContext>
    {
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
        {
        }
        public DbSet<EventStore> EventStores => Set<EventStore>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<EventStore>().ToTable("EventStores");
            modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
        }
    }
}
