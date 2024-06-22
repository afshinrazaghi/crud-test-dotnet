using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.Extensions
{
    public static class ModelBuilderExtensions
    {

        internal static void RemoveCascadeDeleteConvention(this ModelBuilder modelBuilder)
        {
            var foreignKeys = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(entity => entity.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                .ToList();

            foreach (var fk in foreignKeys)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
