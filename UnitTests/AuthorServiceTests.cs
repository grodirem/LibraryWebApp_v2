using AutoMapper;
using BLL.DTOs.Requests;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Profiles;
using BLL.Services;
using BLL.Validators;
using DAL;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

public class AuthorServiceTests : IDisposable
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly IAuthorService _authorService;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;

    public AuthorServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationContext(options);
        _context.Database.EnsureCreated();

        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AuthorProfile());
            mc.AddProfile(new BookProfile());
        });
        _mapper = mappingConfig.CreateMapper();

        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();

        var authorDtoValidator = new AuthorDtoValidator();
        var createAuthorDtoValidator = new CreateAuthorDtoValidator();
        var updateAuthorDtoValidator = new UpdateAuthorDtoValidator();

        _authorService = new AuthorService(
            _authorRepositoryMock.Object,
            _bookRepositoryMock.Object,
            _mapper,
            authorDtoValidator,
            createAuthorDtoValidator,
            updateAuthorDtoValidator);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_ShouldReturnAuthor_WhenAuthorExists()
    {
        var author = new Author
        {
            Id = 1,
            FirstName = "Шыбулбек",
            LastName = "Абдулаев",
            BirthDate = new DateTime(1980, 1, 1),
            Country = "Австралия"
        };

        _authorRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _authorService.GetAuthorByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Шыбулбек", result.FirstName);
        Assert.Equal("Абдулаев", result.LastName);
        Assert.Equal(new DateTime(1980, 1, 1), result.BirthDate);
        Assert.Equal("Австралия", result.Country);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_ShouldThrowNotFoundException_WhenAuthorNotExists()
    {
        _authorRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _authorService.GetAuthorByIdAsync(1));
    }

    [Fact]
    public async Task GetAllAuthorsAsync_ShouldReturnAllAuthors()
    {
        var authors = new List<Author>
        {
            new Author { Id = 1, FirstName = "Шыбулбек", LastName = "Абдулаев" },
            new Author { Id = 2, FirstName = "Иван", LastName = "Иванов" }
        };

        _authorRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        var result = await _authorService.GetAllAuthorsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a.FirstName == "Шыбулбек");
        Assert.Contains(result, a => a.FirstName == "Иван");
    }

    [Fact]
    public async Task CreateAuthorAsync_ShouldReturnAuthorDto_WhenValid()
    {
        var createAuthorDto = new CreateAuthorDto
        {
            FirstName = "Шыбулбек",
            LastName = "Абдулаев",
            BirthDate = new DateTime(1980, 1, 1),
            Country = "Австралия"
        };

        _authorRepositoryMock.Setup(x => x.GetByNameAsync("Шыбулбек Абдулаев", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author)null);

        _authorRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<Author, CancellationToken>((a, _) => a.Id = 1);

        var result = await _authorService.CreateAuthorAsync(createAuthorDto);

        Assert.NotNull(result);
        Assert.Equal("Шыбулбек", result.FirstName);
        Assert.Equal("Абдулаев", result.LastName);
    }

    [Fact]
    public async Task CreateAuthorAsync_ShouldThrowBusinessException_WhenAuthorExists()
    {
        var createAuthorDto = new CreateAuthorDto
        {
            FirstName = "Шыбулбек",
            LastName = "Абдулаев",
            BirthDate = new DateTime(1980, 1, 1),
            Country = "Австралия"
        };

        _authorRepositoryMock.Setup(x => x.GetByNameAsync("Шыбулбек Абдулаев", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Author());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _authorService.CreateAuthorAsync(createAuthorDto));
    }

    [Fact]
    public async Task UpdateAuthorAsync_ShouldUpdateAuthor_WhenValid()
    {
        var updateAuthorDto = new UpdateAuthorDto
        {
            Id = 1,
            FirstName = "Шыбулбек",
            LastName = "Абдулаев",
            BirthDate = new DateTime(1980, 1, 1),
            Country = "Австралия"
        };

        var existingAuthor = new Author { Id = 1, FirstName = "Old", LastName = "Name" };

        _authorRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAuthor);

        _authorRepositoryMock.Setup(x => x.GetByNameAsync("Шыбулбек Абдулаев", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author)null);

        await _authorService.UpdateAuthorAsync(updateAuthorDto);

        _authorRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAuthorAsync_ShouldDeleteAuthor_WhenNoBooks()
    {
        var author = new Author { Id = 1 };

        _authorRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        _bookRepositoryMock.Setup(x => x.GetBooksByAuthorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>());

        await _authorService.DeleteAuthorAsync(1);

        _authorRepositoryMock.Verify(x => x.DeleteAsync(author, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAuthorAsync_ShouldThrowBusinessException_WhenAuthorHasBooks()
    {
        var author = new Author { Id = 1 };
        var books = new List<Book> { new Book() };

        _authorRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        _bookRepositoryMock.Setup(x => x.GetBooksByAuthorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _authorService.DeleteAuthorAsync(1));
    }

    [Fact]
    public async Task GetBooksByAuthorIdAsync_ShouldReturnBooks()
    {
        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Книга 1", AuthorId = 1 },
            new Book { Id = 2, Title = "Книга 2", AuthorId = 1 }
        };

        _bookRepositoryMock.Setup(x => x.GetBooksByAuthorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        var result = await _authorService.GetBooksByAuthorIdAsync(1);

        Assert.Equal(2, result.Count());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}