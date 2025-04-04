using AutoMapper;
using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly Repository<UserBooks> _userBooksRepository;
    private readonly IMapper _mapper;
    private readonly ImageService _imageService;

    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, UserBooksRepository userBooksRepository, IMapper mapper, ImageService imageService)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _userBooksRepository = userBooksRepository;
        _mapper = mapper;
        _imageService = imageService;
    }

    public async Task<PaginatedList<Book>> GetAllBooksPaginatedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var books = await _bookRepository.GetAllPaginated(pageIndex, pageSize, cancellationToken);
        return books;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksFilteredAsync(string? title, string? genre, string? authorName, CancellationToken cancellationToken = default)
    {
        int? authorId = null;

        if (!string.IsNullOrWhiteSpace(authorName))
        {
            var author = await _authorRepository.GetByNameAsync(authorName, cancellationToken);

            if (author == null)
            {
                return Enumerable.Empty<BookDto>();
            }

            authorId = author.Id;
        }

        var books = await _bookRepository.GetAllBooksFilteredAsync(title, genre, authorId, cancellationToken);
        var authorIds = books.Select(b => b.AuthorId).Distinct();
        var authors = await _authorRepository.GetAllAsync(cancellationToken);
        authors = authors.Where(a => authorIds.Contains(a.Id)).ToList();
        var authorDictionary = authors.ToDictionary(a => a.Id, a => $"{a.FirstName} {a.LastName}");

        return books.Select(b => new BookDto
        {
            Id = b.Id,
            AuthorName = authorDictionary.TryGetValue(b.AuthorId, out var name) ? name : "Автор не найден",
            ISBN = b.ISBN,
            Title = b.Title,
            Genre = b.Genre,
            Description = b.Description,
            BorrowedAt = b.BorrowedAt,
            ReturnBy = b.ReturnBy,
            ImagePath = b.ImagePath
        });
    }

    public async Task<BookDto> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        var author = await _authorRepository.GetByIdAsync(book.AuthorId, cancellationToken);

        return new BookDto
        {
            Id = id,
            AuthorName = $"{author.FirstName} {author.LastName}",
            ISBN = book.ISBN,
            Title = book.Title,
            Genre = book.Genre,
            Description = book.Description,
            BorrowedAt = book.BorrowedAt,
            ReturnBy = book.ReturnBy,
            ImagePath = book.ImagePath
        };
    }

    public async Task<BookDto> GetBookByISBNAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByISBNAsync(isbn, cancellationToken);
        var author = await _authorRepository.GetByIdAsync(book.AuthorId, cancellationToken);

        return new BookDto
        {
            Id = book.Id,
            AuthorName = $"{author.FirstName} {author.LastName}",
            ISBN = book.ISBN,
            Title = book.Title,
            Genre = book.Genre,
            Description = book.Description,
            BorrowedAt = book.BorrowedAt,
            ReturnBy = book.ReturnBy,
            ImagePath = book.ImagePath
        };
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto, CancellationToken cancellationToken = default)
    {
        if (createBookDto == null)
        {
            throw new ArgumentNullException(nameof(createBookDto));
        }

        if (createBookDto.AuthorId == 0)
        {
            throw new ArgumentException("Введите id автора.");
        }

        var existingAuthor = await _authorRepository.GetByIdAsync(createBookDto.AuthorId, cancellationToken);

        if (existingAuthor == null)
        {
            throw new ArgumentException("Автор не найден.");
        }

        if (!string.IsNullOrWhiteSpace(createBookDto.ISBN))
        {
            var existingBook = await _bookRepository.GetByISBNAsync(createBookDto.ISBN, cancellationToken);
            if (existingBook != null)
            {
                throw new ArgumentException("Книга с таким ISBN уже существует.");
            }
        }

        var book = _mapper.Map<Book>(createBookDto);

        if (createBookDto.Image != null)
        {
            book.ImagePath = await _imageService.UploadImageAsync(createBookDto.Image, cancellationToken);
        }

        await _bookRepository.AddAsync(book, cancellationToken);
        return _mapper.Map<BookDto>(book);
    }

    public async Task UpdateBookAsync(UpdateBookDto updateBookDto, CancellationToken cancellationToken = default)
    {
        if (updateBookDto == null)
        {
            throw new ArgumentNullException(nameof(updateBookDto));
        }

        var book = await _bookRepository.GetByIdAsync(updateBookDto.Id, cancellationToken);

        if (book == null)
        {
            throw new KeyNotFoundException("Книга не найдена.");
        }

        if (updateBookDto.AuthorId == 0)
        {
            throw new ArgumentException("Введите id автора.");
        }

        var existingAuthor = await _authorRepository.GetByIdAsync(updateBookDto.AuthorId, cancellationToken);

        if (existingAuthor == null)
        {
            throw new ArgumentException("Автор не найден.");
        }

        if (!string.IsNullOrWhiteSpace(updateBookDto.ISBN) && updateBookDto.ISBN != book.ISBN)
        {
            var existingBook = await _bookRepository.GetByISBNAsync(updateBookDto.ISBN, cancellationToken);
            if (existingBook != null)
            {
                throw new ArgumentException("Книга с таким ISBN уже существует.");
            }
        }

        _mapper.Map(updateBookDto, book);

        if (updateBookDto.Image != null)
        {
            _imageService.DeleteImage(book.ImagePath);
            book.ImagePath = await _imageService.UploadImageAsync(updateBookDto.Image, cancellationToken);
        }

        await _bookRepository.UpdateAsync(book, cancellationToken);
    }

    public async Task DeleteBookAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);

        if (book == null)
        {
            throw new KeyNotFoundException("Книга не найдена.");
        }

        await _bookRepository.DeleteAsync(book, cancellationToken);
    }

    public async Task<string> UploadImageAsync(int id, IFormFile file, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);

        if (book == null)
        {
            throw new KeyNotFoundException("Книга не найдена.");
        }

        var imagePath = await _imageService.UploadImageAsync(file, cancellationToken);

        book.ImagePath = imagePath;
        await _bookRepository.UpdateAsync(book, cancellationToken);

        return book.ImagePath;
    }

    public async Task BorrowBookAsync(string userId, BorrowBookRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("Введите id пользователя");
        }

        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);

        if (book == null)
        {
            throw new KeyNotFoundException("Книга не найдена.");
        }

        if (book.IsBorrowed)
        {
            throw new Exception("Книга уже взята.");
        }

        var rental = new UserBooks
        {
            BookId = book.Id,
            UserId = userId,
        };

        await _userBooksRepository.AddAsync(rental, cancellationToken);

        book.BorrowedAt = DateTime.UtcNow;
        book.ReturnBy = request.ReturnBy;
        book.IsBorrowed = true;
        await _bookRepository.UpdateAsync(book, cancellationToken);
    }

    public async Task ReturnBookAsync(string userId, int bookId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("Введите id пользователя");
        }

        var rentals = await _userBooksRepository.GetAllAsync(cancellationToken);
        var rental = rentals.FirstOrDefault(r => r.UserId == userId && r.BookId == bookId);

        if (rental == null) 
        {
            throw new KeyNotFoundException("Запись не найдена.");
        }

        await _userBooksRepository.DeleteAsync(rental, cancellationToken);

        var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);

        if (book != null)
        {
            book.BorrowedAt = null;
            book.ReturnBy = null;
            book.IsBorrowed = false;
            await _bookRepository.UpdateAsync(book, cancellationToken);
        }
    }

    public async Task<IEnumerable<UserBooksDto>> GetUserRentalsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var rentals = await _userBooksRepository.GetAllAsync(cancellationToken);
        var userRentals = rentals.Where(r => r.UserId == userId).ToList();
        var rentalsDto = new List<UserBooksDto>();

        foreach (var rental in userRentals)
        {
            var book = await _bookRepository.GetByIdAsync(rental.BookId, cancellationToken);

            if (book != null)
            {
                var author = await _authorRepository.GetByIdAsync(book.AuthorId, cancellationToken);

                rentalsDto.Add(new UserBooksDto
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Genre = book.Genre,
                    Description = book.Description,
                    AuthorName = $"{author?.FirstName} {author?.LastName}",
                    BorrowedAt = book.BorrowedAt,
                    ReturnedBy = book.ReturnBy
                });
            }
        }

        return rentalsDto;
    }

}
