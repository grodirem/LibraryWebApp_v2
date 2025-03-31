using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var user = new User
        {
            Id = "5e6f7g8h-e5f6-7890-1234-56789abcdef5",
            FirstName = "a",
            LastName = "a",
            UserName = "a",
            NormalizedUserName = "A",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            EmailConfirmed = true,
            SecurityStamp = "fixed-security-stamp"
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, "aaa111!!!");

        builder.HasData(user);
    }
}
