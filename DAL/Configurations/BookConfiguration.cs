using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b =>  b.Id);

        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

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
