using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using BLL.Interfaces;
using BLL.Services;
using BLL.Validators;
using DAL.Interfaces;
using DAL.Repositories;
using FluentValidation;

namespace LibraryWebApp_v2;

public static class ServiceRegistrationExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUserBooksRepository, UserBooksRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IImageService, ImageService>();
    }

    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AuthorDto>, AuthorDtoValidator>();
        services.AddScoped<IValidator<CreateAuthorDto>, CreateAuthorDtoValidator>();
        services.AddScoped<IValidator<UpdateAuthorDto>, UpdateAuthorDtoValidator>();
        services.AddScoped<IValidator<CreateBookDto>, CreateBookDtoValidator>();
        services.AddScoped<IValidator<UpdateBookDto>, UpdateBookDtoValidator>();
        services.AddScoped<IValidator<BorrowBookRequest>, BorrowBookRequestValidator>();
    }
}
