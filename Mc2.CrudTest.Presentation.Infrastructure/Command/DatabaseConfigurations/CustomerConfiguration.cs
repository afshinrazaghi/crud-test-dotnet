using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Domain.ValueObjects;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure.Command.DatabaseConfigurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder
                .ConfigureBaseEntity();

            builder.Property(customer => customer.FirstName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(customer => customer.LastName)
                .IsRequired()
                .HasMaxLength(200);

            builder.
                Property(customer => customer.DateOfBirth)
                .IsRequired()
                .HasColumnType("date");

            builder.OwnsOne(customer => customer.PhoneNumber,
                entity =>
                {
                    entity.Property(phoneNumber => phoneNumber.Value)
                    .IsRequired()
                    .HasColumnName(nameof(Customer.PhoneNumber))
                    .HasMaxLength(13);
                });

            builder.OwnsOne(customer => customer.Email, entity =>
            {
                entity.Property(email => email.Value)
                .IsRequired()
                .HasColumnName(nameof(Customer.Email))
                .HasMaxLength(250);

                entity
                .HasIndex(email => email.Value)
                .IsUnique();
            });

            builder.OwnsOne(customer => customer.BankAccountNumber, entity =>
            {
                entity.Property(bankAccountNumber => bankAccountNumber.Value)
                .IsRequired()
                .HasColumnName(nameof(Customer.BankAccountNumber))
                .HasMaxLength(20);
            });

        }
    }
}
