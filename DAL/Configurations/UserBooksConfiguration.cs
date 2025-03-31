using DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configurations;

public class UserBooksConfiguration : IEntityTypeConfiguration<UserBooks>
{
    public void Configure(EntityTypeBuilder<UserBooks> builder)
    {
        builder.HasKey(ub => ub.Id);

        builder.Property(ub => ub.UserId)
            .IsRequired();

        builder.Property(ub => ub.BookId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Book>()
            .WithMany()
            .HasForeignKey(ub => ub.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}