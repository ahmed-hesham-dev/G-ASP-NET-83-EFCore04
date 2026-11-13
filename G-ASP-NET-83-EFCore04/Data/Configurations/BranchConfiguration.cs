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
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(b => b.Code);
            builder.Property(b => b.Code).HasMaxLength(20);
            builder.Property(b => b.Name).HasMaxLength(100).IsRequired();
            builder.Property(b => b.Address).HasMaxLength(200).IsRequired();
            builder.Property(b => b.PhoneNumber).HasMaxLength(20).IsRequired();

            builder.HasOne(b => b.Manager)
                   .WithOne(m => m.Branch)
                   .HasForeignKey<Branch>(b => b.ManagerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Seed
            builder.HasData(
                new Branch { Code = "CAI-01", Name = "Cairo Main Branch", Address = "Tahrir Square, Cairo", PhoneNumber = "0223456789", ManagerId = 1 },
                new Branch { Code = "ALX-01", Name = "Alexandria Branch", Address = "Corniche Road, Alexandria", PhoneNumber = "0334567890", ManagerId = 2 },
                new Branch { Code = "GIZ-01", Name = "Giza Branch", Address = "Mohandessin, Giza", PhoneNumber = "0235678901", ManagerId = 3 }
            );
        }
    }
}
