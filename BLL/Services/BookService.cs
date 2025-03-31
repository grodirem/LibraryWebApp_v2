using AutoMapper;
using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using DAL;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly Repository<UserBooks> _userBooksRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationContext _context;

    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, UserBooksRepository userBooksRepository, IMapper mapper, ApplicationContext context)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _userBooksRepository = userBooksRepository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync(string? search, string? genre, string? author)
    {
        var booksQuery = _bookRepository.GetAllBooksQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            booksQuery = booksQuery.Where(b => b.Title.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            booksQuery = booksQuery.Where(b => b.Genre == genre);
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            var authorEntity = await _authorRepository.GetByNameAsync(author);
            if (authorEntity != null)
            {
                booksQuery = booksQuery.Where(b => b.AuthorId == authorEntity.Id);
            }
            else
            {
                return new List<BookDto>();
            }
        }

        var booksWithAuthors = await booksQuery
            .Join(
                _context.Authors,
                book => book.AuthorId,
                author => author.Id,
                (book, author) => new
                {
                    Book = book,
                    AuthorName = $"{author.FirstName} {author.LastName}"
                }
            )
            .ToListAsync();

        var bookDtos = booksWithAuthors.Select(b => new BookDto
        {
            Id = b.Book.Id,
            AuthorName = b.AuthorName,
            ISBN = b.Book.ISBN,
            Title = b.Book.Title,
            Genre = b.Book.Genre,
            Description = b.Book.Description,
            BorrowedAt = b.Book.BorrowedAt,
            ReturnBy = b.Book.ReturnBy
        });

        return bookDtos;
    }

    public async Task<BookDto> GetBookByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        var author = await _authorRepository.GetByIdAsync(book.AuthorId);

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

    public async Task<BookDto> GetBookByISBNAsync(string isbn)
    {
        var book = await _bookRepository.GetByISBNAsync(isbn);
        var author = await _authorRepository.GetByIdAsync(book.AuthorId);

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

    public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
    {
        if (createBookDto == null)
        {
            throw new ArgumentNullException(nameof(createBookDto));
        }

        if (createBookDto.AuthorId == 0)
        {
            throw new ArgumentException("Введите id автора.");
        }

        var existingAuthor = await _authorRepository.GetByIdAsync(createBookDto.AuthorId);

        if (existingAuthor == null)
        {
            throw new ArgumentException("Автор не найден.");
        }

        var book = _mapper.Map<Book>(createBookDto);

        if (createBookDto.Image != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createBookDto.Image.FileName);
            var filePath = Path.Combine("wwwroot/Images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await createBookDto.Image.CopyToAsync(stream);
            }

            book.ImagePath = $"/Images/{fileName}";
        }

        await _bookRepository.AddAsync(book);
        return _mapper.Map<BookDto>(book);
    }

    public async Task UpdateBookAsync(UpdateBookDto updateBookDto)
    {
        var book = await _bookRepository.GetByIdAsync(updateBookDto.Id);

        if (book == null)
        {
            throw new KeyNotFoundException("Книга не найдена.");
        }

        _mapper.Map(updateBookDto, book);

        if (updateBookDto.Image != null)
        {
            if (!string.IsNullOrEmpty(book.ImagePath))
            {
                var oldFilePath = Path.Combine("wwwroot", book.ImagePath.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateBookDto.Image.FileName);
            var filePath = Path.Combine("wwwroot/Images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await updateBookDto.Image.CopyToAsync(stream);
            }

            book.ImagePath = $"/Images/{fileName}";
        }

        await _bookRepository.UpdateAsync(book);
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
        {
            throw new KeyNotFoundException("Книга не найдена.");
        }

        await _bookRepository.DeleteAsync(book);
    }

    public async Task<string> UploadImageAsync(int id, IFormFile file)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
        {
            throw new KeyNotFoundException("Книга не найдена.");
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine("wwwroot/Images", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        book.ImagePath = $"/Images/{fileName}";
        await _bookRepository.UpdateAsync(book);

        return book.ImagePath;
    }

    public async Task BorrowBookAsync(string userId, BorrowBookRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId);

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

        await _userBooksRepository.AddAsync(rental);

        book.BorrowedAt = DateTime.UtcNow;
        book.ReturnBy = request.ReturnBy;
        book.IsBorrowed = true;
        await _bookRepository.UpdateAsync(book);
    }

    public async Task ReturnBookAsync(string userId, int bookId)
    {
        var rentals = await _userBooksRepository.GetAllAsync();
        var rental = rentals.FirstOrDefault(r => r.UserId == userId && r.BookId == bookId);

        if (rental == null) 
        {
            throw new KeyNotFoundException("Запись не найдена.");
        }

        await _userBooksRepository.DeleteAsync(rental);

        var book = await _bookRepository.GetByIdAsync(bookId);
        book.BorrowedAt = null;
        book.ReturnBy = null;
        book.IsBorrowed = false;
        await _bookRepository.UpdateAsync(book);
    }

    public async Task<IEnumerable<UserBooksDto>> GetUserRentalsAsync(string userId)
    {
        var rentals = await _userBooksRepository.GetAllAsync();
        var userRentals = rentals.Where(r => r.UserId == userId).ToList();
        var rentalsDto = new List<UserBooksDto>();

        foreach (var rental in userRentals)
        {
            var book = await _bookRepository.GetByIdAsync(rental.BookId);

            if (book != null)
            {
                var author = await _authorRepository.GetByIdAsync(book.AuthorId);

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

    public async Task<PaginatedList<Book>> GetAllBooksPaginatedAsync(int pageIndex, int pageSize)
    {
        var books = await _bookRepository.GetAllPaginated(pageIndex, pageSize);
        return books;
    }
}
