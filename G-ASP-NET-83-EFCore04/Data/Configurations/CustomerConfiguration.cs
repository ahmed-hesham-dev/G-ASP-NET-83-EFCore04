using G_ASP_NET_83_EFCore04.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_ASP_NET_83_EFCore04.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.FullName).HasMaxLength(100).IsRequired();
            builder.Property(c => c.NationalId).HasMaxLength(14).IsRequired();
            builder.HasIndex(c => c.NationalId).IsUnique();
            builder.Property(c => c.Email).HasMaxLength(100).IsRequired();
            builder.Property(c => c.PhoneNumber).HasMaxLength(20).IsRequired();
            builder.Property(c => c.Address).HasMaxLength(200).IsRequired();
            builder.Property(c => c.CustomerType).HasConversion<string>().HasMaxLength(20);
        }
    }
}
