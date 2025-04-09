using AutoMapper;
using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using BLL.Interfaces;
using BLL.Profiles;
using BLL.Services;
using BLL.Validators;
using DAL;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;

public class AuthorServiceTests
{
    private readonly DbContextOptions<ApplicationContext> _options;
    private readonly IMapper _mapper;
    private readonly IAuthorService _authorService;

    public AuthorServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AuthorProfile());
        });
        _mapper = mappingConfig.CreateMapper();

        using (var context = new ApplicationContext(_options))
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        var authorRepository = new AuthorRepository(new ApplicationContext(_options));
        var bookRepository = new BookRepository(new ApplicationContext(_options));
        var authorDtoValidator = new AuthorDtoValidator();
        var createAuthorDtoValidator = new CreateAuthorDtoValidator(authorRepository);
        var updateAuthorDtoValidator = new UpdateAuthorDtoValidator(authorRepository);

        _authorService = new AuthorService(authorRepository, bookRepository, _mapper, authorDtoValidator, createAuthorDtoValidator, updateAuthorDtoValidator);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_ShouldReturnAuthor_WhenAuthorExists()
    {
        Author author;
        using (var context = new ApplicationContext(_options))
        {
            var authorRepository = new AuthorRepository(context);
            author = new Author
            {
                Id = 1,
                FirstName = "Шыбулбек",
                LastName = "Абдулаев",
                BirthDate = new DateTime(1980, 1, 1),
                Country = "Австралия"
            };
            await authorRepository.AddAsync(author);
            await context.SaveChangesAsync();
        }

        AuthorDto result;
        result = await _authorService.GetAuthorByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Шыбулбек", result.FirstName);
        Assert.Equal("Абдулаев", result.LastName);
        Assert.Equal(new DateTime(1980, 1, 1), result.BirthDate);
        Assert.Equal("Австралия", result.Country);
    }

    [Fact]
    public async Task GetAllAuthorsAsync_ShouldReturnAllAuthors_WhenAuthorsExist()
    {
        using (var context = new ApplicationContext(_options))
        {
            var authorRepository = new AuthorRepository(context);
            var authors = new List<Author>
            {
                new Author { Id = 1, FirstName = "Шыбулбек", LastName = "Абдулаев", BirthDate = new DateTime(1980, 1, 1), Country = "Австралия" },
                new Author { Id = 2, FirstName = "Иван", LastName = "Иванов", BirthDate = new DateTime(1990, 1, 1), Country = "Россия" }
            };

            foreach (var author in authors)
            {
                await authorRepository.AddAsync(author);
            }
            await context.SaveChangesAsync();
        }

        IEnumerable<AuthorDto> result;
        result = await _authorService.GetAllAuthorsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a.FirstName == "Шыбулбек");
        Assert.Contains(result, a => a.FirstName == "Иван");
    }

    [Fact]
    public async Task CreateAuthorAsync_ShouldReturnAuthorDto_WhenAuthorIsCreated()
    {
        var createAuthorDto = new CreateAuthorDto
        {
            FirstName = "Шыбулбек",
            LastName = "Абдулаев",
            BirthDate = new DateTime(1980, 1, 1),
            Country = "Австралия"
        };

        var result = await _authorService.CreateAuthorAsync(createAuthorDto);

        Assert.NotNull(result);
        Assert.Equal("Шыбулбек", result.FirstName);
        Assert.Equal("Абдулаев", result.LastName);
        Assert.Equal(new DateTime(1980, 1, 1), result.BirthDate);
        Assert.Equal("Австралия", result.Country);
    }

    [Fact]
    public async Task CreateAuthorAsync_ShouldThrowValidationException_WhenFirstNameIsEmpty()
    {
        var createAuthorDto = new CreateAuthorDto
        {
            FirstName = "",
            LastName = "Абдулаев",
            BirthDate = new DateTime(1980, 1, 1),
            Country = "Австралия"
        };

        var exception = await Assert.ThrowsAsync<ValidationException>(() => _authorService.CreateAuthorAsync(createAuthorDto));
        Assert.NotEmpty(exception.Errors);
        Assert.Contains(exception.Errors, e => e.ErrorMessage == "Введите имя.");
    }

    [Fact]
    public async Task CreateAuthorAsync_ShouldThrowException_WhenRepositoryFails()
    {
        var createAuthorDto = new CreateAuthorDto
        {
            FirstName = "Шыбулбек",
            LastName = "Абдулаев",
            BirthDate = new DateTime(1980, 1, 1),
            Country = "Австралия"
        };

        var authorRepositoryMock = new Mock<IAuthorRepository>();
        authorRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Ошибка базы данных"));

        var bookRepository = new BookRepository(new ApplicationContext(_options));
        var authorDtoValidator = new AuthorDtoValidator();
        var createAuthorDtoValidator = new CreateAuthorDtoValidator(authorRepositoryMock.Object);
        var updateAuthorDtoValidator = new UpdateAuthorDtoValidator(authorRepositoryMock.Object);
        var authorService = new AuthorService(authorRepositoryMock.Object, bookRepository, _mapper, authorDtoValidator, createAuthorDtoValidator, updateAuthorDtoValidator);

        var exception = await Assert.ThrowsAsync<Exception>(() => authorService.CreateAuthorAsync(createAuthorDto));
        Assert.Equal("Ошибка при создании автора.", exception.Message);
    }

    [Fact]
    public async Task DeleteAuthorAsync_ShouldThrowException_WhenAuthorNotFound()
    {
        var authorRepositoryMock = new Mock<IAuthorRepository>();
        authorRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author)null);

        var authorService = new AuthorService(authorRepositoryMock.Object, null, _mapper, null, null, null);

        var exception = await Assert.ThrowsAsync<Exception>(() => authorService.DeleteAuthorAsync(1));
        Assert.Equal("Автор не найден.", exception.Message);
    }

    [Fact]
    public async Task GetBooksByAuthorIdAsync_ShouldReturnBooks_WhenBooksExist()
    {
        var authorId = 1;
        var books = new List<Book>
    {
        new Book { Id = 1, Title = "Книга", AuthorId = authorId },
        new Book { Id = 2, Title = "Книииииииига", AuthorId = authorId }
    };

        var bookRepositoryMock = new Mock<IBookRepository>();
        bookRepositoryMock.Setup(repo => repo.GetBooksByAuthorIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        var authorService = new AuthorService(null, bookRepositoryMock.Object, _mapper, null, null, null);
        var result = await authorService.GetBooksByAuthorIdAsync(authorId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, b => b.Title == "Книга");
        Assert.Contains(result, b => b.Title == "Книииииииига");
    }

    [Fact]
    public async Task GetBooksByAuthorIdAsync_ShouldReturnEmpty_WhenNoBooksExist()
    {
        var authorId = 1;

        var bookRepositoryMock = new Mock<IBookRepository>();
        bookRepositoryMock.Setup(repo => repo.GetBooksByAuthorIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>());

        var authorService = new AuthorService(null, bookRepositoryMock.Object, _mapper, null, null, null);
        var result = await authorService.GetBooksByAuthorIdAsync(authorId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}