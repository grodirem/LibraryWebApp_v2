using DAL;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UnitTests;

public class AuthorRepositoryTests
{
    private static readonly DbContextOptions<ApplicationContext> _options;

    static AuthorRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldSaveAuthor()
    {
        using (var context = new ApplicationContext(_options))
        {
            var repository = new Repository<Author>(context);
            var newAuthor = new Author { Id = 1, FirstName = "Шыбулбек", LastName = "Абдулаев", BirthDate = new DateTime(1980, 1, 1), Country = "Австралия" };

            await repository.AddAsync(newAuthor);
        }

        using (var context = new ApplicationContext(_options))
        {
            var author = await context.Set<Author>().FindAsync(1);
            Assert.NotNull(author);
            Assert.Equal("Шыбулбек", author.FirstName);
            Assert.Equal("Абдулаев", author.LastName);
            Assert.Equal(new DateTime(1980, 1, 1), author.BirthDate);
            Assert.Equal("Австралия", author.Country);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAuthor()
    {
        using (var context = new ApplicationContext(_options))
        {
            var repository = new Repository<Author>(context);
            var newAuthor = new Author { Id = 2, FirstName = "Шыбулбек", LastName = "Абдулаев", BirthDate = new DateTime(1980, 1, 1), Country = "Австралия" };
            await repository.AddAsync(newAuthor);
        }

        Author result;
        using (var context = new ApplicationContext(_options))
        {
            var repository = new Repository<Author>(context);
            result = await repository.GetByIdAsync(2);
        }

        Assert.NotNull(result);
        Assert.Equal("Шыбулбек", result.FirstName);
        Assert.Equal("Абдулаев", result.LastName);
        Assert.Equal(new DateTime(1980, 1, 1), result.BirthDate);
        Assert.Equal("Австралия", result.Country);
    }
}