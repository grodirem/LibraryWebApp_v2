using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations;

public class UserBooksConfiguration : IEntityTypeConfiguration<UserBooks>
{
    public void Configure(EntityTypeBuilder<UserBooks> builder)
    {
        builder.HasKey(ub => ub.Id);

        builder.HasOne(ub => ub.User)
            .WithMany()
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ub => ub.Book)
            .WithMany()
            .HasForeignKey(ub => ub.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}