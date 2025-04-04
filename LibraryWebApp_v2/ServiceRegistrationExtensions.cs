using BLL.Services;
using BLL.Validators;
using DAL.Interfaces;
using DAL.Repositories;

namespace LibraryWebApp_v2;

public static class ServiceRegistrationExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<UserBooksRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<BookService>();
        services.AddScoped<AuthService>();
        services.AddScoped<AuthorService>();
        services.AddScoped<TokenService>();
        services.AddScoped<ImageService>();
    }

    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<AuthorDtoValidator>();
        services.AddScoped<CreateAuthorDtoValidator>();
        services.AddScoped<UpdateAuthorDtoValidator>();
    }
}
