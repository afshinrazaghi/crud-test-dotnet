using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.DatabaseConfigurations
{
    public class EventStoreConfiguration : IEntityTypeConfiguration<EventStore>
    {
        public void Configure(EntityTypeBuilder<EventStore> builder)
        {
            builder
                .HasKey(entity => entity.Id);

            builder.
                Property(entity => entity.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(entity => entity.AggregateId)
                .IsRequired();

            builder.
                Property(entity => entity.MessageType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(entity => entity.Data)
                .IsRequired()
                .HasComment("JSON serialized event");

            builder.Property(entity => entity.OccurredOn)
                .IsRequired()
                .HasComment("CreatedAt");
        }
    }
}
