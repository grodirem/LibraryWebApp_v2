using BLL.Profiles;
using DAL;
using DAL.Models;
using LibraryWebApp_v2.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace LibraryWebApp_v2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAutoMapper(typeof(UserProfile), typeof(BookProfile), typeof(AuthorProfile), typeof(UserBooksProfile));
        builder.Services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        var jwtSettings = builder.Configuration.GetSection("JWTSettings");

        builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"]))
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("OnlyAdminUsers", policy => policy.RequireRole("Admin"));
            options.AddPolicy("AuthenticatedUsers", policy => policy.RequireAuthenticatedUser());
        });

        builder.Services.AddSingleton<JwtHandler>();
        builder.Services.AddControllers();

        builder.Services.AddRepositories();
        builder.Services.AddServices();
        builder.Services.AddValidators();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 5 * 1024 * 1024;
        });

        builder.WebHost.UseUrls("http://+:8080");

        var app = builder.Build();
        ConfigureAppSettings(app);
        ConfigureImageDirectories();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationContext>();
            context.Database.Migrate();
        }

        app.MapControllers();
        app.Run();
    }

    private static void ConfigureAppSettings(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseMiddleware<Middleware>();
        app.UseCors("AllowReactApp");
        app.UseAuthentication();
        app.UseAuthorization();
    }

    private static void ConfigureImageDirectories()
    {
        var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
        if (!Directory.Exists(imagesPath))
        {
            Directory.CreateDirectory(imagesPath);
        }
    }
}