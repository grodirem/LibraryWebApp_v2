using AutoMapper;
using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using BLL.Validators;
using DAL.Interfaces;
using DAL.Models;
using FluentValidation;

namespace BLL.Services;

public class AuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly AuthorDtoValidator _authorDtoValidator;
    private readonly CreateAuthorDtoValidator _createAuthorDtoValidator;
    private readonly UpdateAuthorDtoValidator _updateAuthorDtoValidator;

    public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository, IMapper mapper, AuthorDtoValidator authorDtoValidator, CreateAuthorDtoValidator createAuthorDtoValidator, UpdateAuthorDtoValidator updateAuthorDtoValidator)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _mapper = mapper;
        _authorDtoValidator = authorDtoValidator;
        _createAuthorDtoValidator = createAuthorDtoValidator;
        _updateAuthorDtoValidator = updateAuthorDtoValidator;
    }

    public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync(CancellationToken cancellationToken = default)
    {
        var authors = await _authorRepository.GetAllAsync(cancellationToken);
        var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);
        return authorDtos;
    }

    public async Task<AuthorDto> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetByIdAsync(id, cancellationToken);

        if (author == null)
        {
            throw new Exception("Автор не найден.");
        }

        var authorDto = _mapper.Map<AuthorDto>(author);
        return authorDto;
    }

    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createAuthorDtoValidator.ValidateAsync(createAuthorDto, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var author = _mapper.Map<Author>(createAuthorDto);

        try
        {
            await _authorRepository.AddAsync(author, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при создании автора.", ex);
        }

        return _mapper.Map<AuthorDto>(author);
    }

    public async Task UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateAuthorDtoValidator.ValidateAsync(updateAuthorDto, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var author = _mapper.Map<Author>(updateAuthorDto);

        try
        {
            await _authorRepository.UpdateAsync(author, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при обновлении автора.", ex);
        }
    }

    public async Task DeleteAuthorAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetByIdAsync(id, cancellationToken);

        if (author == null)
        {
            throw new Exception("Автор не найден.");
        }

        await _authorRepository.DeleteAsync(author, cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var books = await _bookRepository.GetBooksByAuthorIdAsync(id, cancellationToken);
        return books;
    }
}
