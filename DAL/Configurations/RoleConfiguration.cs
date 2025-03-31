using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            { 
                Id = "c2f2b3e8-4f0a-453b-9b6e-bb8e1f7a1c1d",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "d3e4f5a6-7b8c-9d0e-1f2a-3b4c5d6e7f8g",
                Name = "User",
                NormalizedName = "USER"
            }
        );
    }
}
