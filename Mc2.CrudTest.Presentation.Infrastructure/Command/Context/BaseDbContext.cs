using Mc2.CrudTest.Presentation.Infrastructure.Command.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.Context
{
    public class BaseDbContext<TContext> : DbContext
        where TContext : DbContext
    {
        public BaseDbContext(DbContextOptions<TContext> options)
            : base(options)
        {

        }

        public const string Collation = "Latin1_General_CI_AI";
        public override ChangeTracker ChangeTracker
        {
            get
            {
                base.ChangeTracker.LazyLoadingEnabled = false;
                base.ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
                base.ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
                return base.ChangeTracker;
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<string>()
                .AreUnicode(false)
                .HaveMaxLength(300);

            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation(Collation)
                .RemoveCascadeDeleteConvention();

            base.OnModelCreating(modelBuilder);
        }
    }
}
