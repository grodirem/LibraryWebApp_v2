using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b =>  b.Id);

        builder.Property(b => b.AuthorId)
            .IsRequired();

        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(13);

        builder.HasIndex(b => b.ISBN)
            .IsUnique();

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Genre)
            .HasMaxLength(30);

        builder.Property(b => b.Description)
            .HasMaxLength(400);

        builder.Property(b => b.BorrowedAt)
            .IsRequired(false);
        
        builder.Property(b => b.ReturnBy)
            .IsRequired(false);

        builder.Property(b => b.IsBorrowed)
            .IsRequired();
    }
}
