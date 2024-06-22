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
        public DbSet<EventStore> EventStores => Set<EventStore>();

        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
        }
    }
}
