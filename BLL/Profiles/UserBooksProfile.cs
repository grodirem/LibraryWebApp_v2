using AutoMapper;
using BLL.DTOs.Models;
using DAL.Models;

namespace BLL.Profiles;

public class UserBooksProfile : Profile
{
    public UserBooksProfile()
    {
        CreateMap<UserBooks, UserBooksDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Book.Genre))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Book.Description))
            .ForMember(dest => dest.BorrowedAt, opt => opt.MapFrom(src => src.Book.BorrowedAt))
            .ForMember(dest => dest.ReturnedBy, opt => opt.MapFrom(src => src.Book.ReturnBy))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src =>
            src.Book.Author != null
            ? $"{src.Book.Author.FirstName} {src.Book.Author.LastName}"
            : ""));
    }
}
