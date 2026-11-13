using G_ASP_NET_83_EFCore04.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NationalBank.Data.Configurations;

public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
    {
        builder.Property(m => m.FullName).HasMaxLength(100).IsRequired();
        builder.Property(m => m.Email).HasMaxLength(100).IsRequired();
        builder.Property(m => m.PhoneNumber).HasMaxLength(20).IsRequired();

        // Seed
        builder.HasData(
            new Manager { Id = 1, FullName = "Hassan Mahmoud", Email = "hassan.mahmoud@nationalbank.com", PhoneNumber = "01011111111", HireDate = new DateTime(2018, 3, 1) },
            new Manager { Id = 2, FullName = "Sara Khaled", Email = "sara.khaled@nationalbank.com", PhoneNumber = "01022222222", HireDate = new DateTime(2020, 7, 15) },
            new Manager { Id = 3, FullName = "Omar Fathy", Email = "omar.fathy@nationalbank.com", PhoneNumber = "01033333333", HireDate = new DateTime(2021, 1, 10) }
        );
    }
}