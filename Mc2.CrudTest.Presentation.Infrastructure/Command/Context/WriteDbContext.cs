using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Infrastructure.Command.DatabaseConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.Context
{
    public class WriteDbContext : BaseDbContext<WriteDbContext>
    {
        public WriteDbContext(DbContextOptions<WriteDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        }
    }
}
