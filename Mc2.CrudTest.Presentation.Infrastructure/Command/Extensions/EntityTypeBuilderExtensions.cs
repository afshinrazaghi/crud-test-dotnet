using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.Extensions
{
    internal static class EntityTypeBuilderExtensions
    {
        internal static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : BaseEntity
        {
            builder.
                HasKey(entity => entity.Id);

            builder.Property(entity => entity.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Ignore(entity => entity.DomainEvents);
        }
    }
}
