using AutoMapper;
using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using FluentValidation;

namespace BLL.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<AuthorDto> _authorDtoValidator;
    private readonly IValidator<CreateAuthorDto> _createAuthorDtoValidator;
    private readonly IValidator<UpdateAuthorDto> _updateAuthorDtoValidator;

    public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository, IMapper mapper, IValidator<AuthorDto> authorDtoValidator, IValidator<CreateAuthorDto> createAuthorDtoValidator, IValidator<UpdateAuthorDto> updateAuthorDtoValidator)
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
            throw new NotFoundException("Автор не найден.");
        }

        var authorDto = _mapper.Map<AuthorDto>(author);
        return authorDto;
    }

    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto, CancellationToken cancellationToken = default)
    {
        await _createAuthorDtoValidator.ValidateAndThrowAsync(createAuthorDto, cancellationToken);

        var existingAuthor = await _authorRepository.GetByNameAsync(
            $"{createAuthorDto.FirstName} {createAuthorDto.LastName}",
            cancellationToken);

        if (existingAuthor != null)
        {
            throw new InvalidOperationException($"Автор {createAuthorDto.FirstName} {createAuthorDto.LastName} уже существует");
        }

        var author = _mapper.Map<Author>(createAuthorDto);
        await _authorRepository.AddAsync(author, cancellationToken);

        return _mapper.Map<AuthorDto>(author);
    }

    public async Task UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto, CancellationToken cancellationToken = default)
    {
        await _updateAuthorDtoValidator.ValidateAndThrowAsync(updateAuthorDto, cancellationToken);

        var existingAuthor = await _authorRepository.GetByIdAsync(updateAuthorDto.Id, cancellationToken)
            ?? throw new NotFoundException($"Автор с ID {updateAuthorDto.Id} не найден");

        var newFullName = $"{updateAuthorDto.FirstName} {updateAuthorDto.LastName}";
        if ($"{existingAuthor.FirstName} {existingAuthor.LastName}" != newFullName)
        {
            var authorWithSameName = await _authorRepository.GetByNameAsync(newFullName, cancellationToken);
            if (authorWithSameName != null)
            {
                throw new InvalidOperationException($"Автор {newFullName} уже существует");
            }
        }

        _mapper.Map(updateAuthorDto, existingAuthor);
        await _authorRepository.UpdateAsync(existingAuthor, cancellationToken);
    }

    public async Task DeleteAuthorAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetByIdAsync(id, cancellationToken);
        if (author == null)
        {
            throw new NotFoundException("Автор не найден.");
        }

        var books = await _bookRepository.GetBooksByAuthorIdAsync(id, cancellationToken);
        if (books.Any())
        {
            throw new InvalidOperationException("Нельзя удалить автора у которого есть книги.");
        }

        await _authorRepository.DeleteAsync(author, cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var books = await _bookRepository.GetBooksByAuthorIdAsync(id, cancellationToken);
        return books;
    }
}
