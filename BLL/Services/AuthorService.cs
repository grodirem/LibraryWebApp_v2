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

    public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
    {
        var authors = await _authorRepository.GetAllAsync();
        var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);

        foreach (var authorDto in authorDtos)
        {
            var validationResult = await _authorDtoValidator.ValidateAsync(authorDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        return authorDtos;
    }

    public async Task<AuthorDto> GetAuthorByIdAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        var authorDto = _mapper.Map<AuthorDto>(author);
        var validationResult = await _authorDtoValidator.ValidateAsync(authorDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return authorDto;
    }

    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
    {
        var validationResult = await _createAuthorDtoValidator.ValidateAsync(createAuthorDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var author = _mapper.Map<Author>(createAuthorDto);

        try
        {
            await _authorRepository.AddAsync(author);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при создании автора.", ex);
        }

        return _mapper.Map<AuthorDto>(author);
    }

    public async Task UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto)
    {
        var validationResult = await _updateAuthorDtoValidator.ValidateAsync(updateAuthorDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var author = _mapper.Map<Author>(updateAuthorDto);

        try
        {
            await _authorRepository.UpdateAsync(author);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при обновлении автора.", ex);
        }
    }

    public async Task DeleteAuthorAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);

        if (author == null)
        {
            throw new Exception("Автор не найден.");
        }

        await _authorRepository.DeleteAsync(author);
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int id)
    {
        var books = await _bookRepository.GetBooksByAuthorIdAsync(id);
        return books;
    }
}
