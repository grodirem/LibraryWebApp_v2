using AutoMapper;
using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BLL.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IUserBooksRepository _userBooksRepository;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;
    private readonly IValidator<CreateBookDto> _createBookValidator;
    private readonly IValidator<UpdateBookDto> _updateBookValidator;
    private readonly IValidator<BorrowBookRequest> _borrowBookDto;

    public BookService(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        IUserBooksRepository userBooksRepository,
        IMapper mapper,
        IImageService imageService,
        IValidator<CreateBookDto> createBookValidator,
        IValidator<UpdateBookDto> updateBookValidator,
        IValidator<BorrowBookRequest> borrowBookDto)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _userBooksRepository = userBooksRepository;
        _mapper = mapper;
        _imageService = imageService;
        _createBookValidator = createBookValidator;
        _updateBookValidator = updateBookValidator;
        _borrowBookDto = borrowBookDto;
    }

    public async Task<PaginatedList<Book>> GetAllBooksPaginatedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _bookRepository.GetAllPaginated(pageIndex, pageSize, cancellationToken);
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksFilteredAsync(string? title, string? genre, string? authorName, CancellationToken cancellationToken = default)
    {
        int? authorId = null;

        if (!string.IsNullOrWhiteSpace(authorName))
        {
            var author = await _authorRepository.GetByNameAsync(authorName, cancellationToken);
            if (author == null) return Enumerable.Empty<BookDto>();
            authorId = author.Id;
        }

        var books = await _bookRepository.GetAllBooksFilteredAsync(title, genre, authorId, cancellationToken);
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<BookDto> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdWithAuthorAsync(id, cancellationToken);
        if (book == null) throw new NotFoundException("Книга не найдена.");
        return _mapper.Map<BookDto>(book);
    }

    public async Task<BookDto> GetBookByISBNAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByISBNAsync(isbn, cancellationToken);
        if (book == null) throw new NotFoundException("Книга не найдена.");
        return _mapper.Map<BookDto>(book);
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto, CancellationToken cancellationToken = default)
    {
        await _createBookValidator.ValidateAndThrowAsync(createBookDto, cancellationToken);

        var existingAuthor = await _authorRepository.GetByIdAsync(createBookDto.AuthorId, cancellationToken);
        if (existingAuthor == null)
        {
            throw new NotFoundException("Автор не найден");
        }

        if (!string.IsNullOrEmpty(createBookDto.ISBN))
        {
            var existingBook = await _bookRepository.GetByISBNAsync(createBookDto.ISBN, cancellationToken);
            if (existingBook != null)
            {
                throw new InvalidOperationException("Книга с таким ISBN уже существует");
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
        await _updateBookValidator.ValidateAndThrowAsync(updateBookDto, cancellationToken);

        var book = await _bookRepository.GetByIdAsync(updateBookDto.Id, cancellationToken);
        if (book == null)
        {
            throw new NotFoundException("Книга не найдена");
        }

        var authorExists = await _authorRepository.GetByIdAsync(updateBookDto.AuthorId, cancellationToken);
        if (authorExists == null)
        {
            throw new NotFoundException("Автор не найден");
        }

        if (!string.IsNullOrEmpty(updateBookDto.ISBN) && updateBookDto.ISBN != book.ISBN)
        {
            var existingBook = await _bookRepository.GetByISBNAsync(updateBookDto.ISBN, cancellationToken);
            if (existingBook != null)
            {
                throw new InvalidOperationException("Книга с таким ISBN уже существует");
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
        if (book == null) throw new NotFoundException("Книга не найдена.");
        await _bookRepository.DeleteAsync(book, cancellationToken);
    }

    public async Task<string> UploadImageAsync(int id, IFormFile file, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        if (book == null) throw new NotFoundException("Книга не найдена.");

        var imagePath = await _imageService.UploadImageAsync(file, cancellationToken);
        book.ImagePath = imagePath;
        await _bookRepository.UpdateAsync(book, cancellationToken);

        return imagePath;
    }

    public async Task BorrowBookAsync(string userId, BorrowBookRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("Введите id пользователя");

        await _borrowBookDto.ValidateAndThrowAsync(request, cancellationToken);

        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book == null) throw new NotFoundException("Книга не найдена.");
        if (book.IsBorrowed) throw new InvalidOperationException("Книга уже взята.");

        await _userBooksRepository.AddAsync(new UserBooks
        {
            BookId = book.Id,
            UserId = userId
        }, cancellationToken);

        book.BorrowedAt = DateTime.UtcNow;
        book.ReturnBy = request.ReturnBy;
        book.IsBorrowed = true;
        await _bookRepository.UpdateAsync(book, cancellationToken);
    }

    public async Task ReturnBookAsync(string userId, int bookId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("Введите id пользователя");

        var rental = (await _userBooksRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(r => r.UserId == userId && r.BookId == bookId);

        if (rental == null) throw new NotFoundException("Запись не найдена.");

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
        var rentals = await _userBooksRepository.GetUserRentalsAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<UserBooksDto>>(rentals);
    }
}