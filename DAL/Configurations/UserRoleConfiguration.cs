using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(
            new IdentityUserRole<string>
            {
                UserId = "5e6f7g8h-e5f6-7890-1234-56789abcdef5",
                RoleId = "c2f2b3e8-4f0a-453b-9b6e-bb8e1f7a1c1d"
            }
        );
    }
}
