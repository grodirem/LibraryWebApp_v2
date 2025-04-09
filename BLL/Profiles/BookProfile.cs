using AutoMapper;
using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using DAL.Models;

namespace BLL.Profiles;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<CreateBookDto, Book>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.BorrowedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ReturnBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsBorrowed, opt => opt.Ignore());

        CreateMap<UpdateBookDto, Book>()
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.BorrowedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ReturnBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsBorrowed, opt => opt.Ignore());

        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(src =>
                    src.Author != null
                        ? $"{src.Author.FirstName} {src.Author.LastName}"
                        : "Автор не найден"));

        CreateMap<UserBooks, UserBooksDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Book.Genre))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Book.Description))
            .ForMember(dest => dest.BorrowedAt, opt => opt.MapFrom(src => src.Book.BorrowedAt))
            .ForMember(dest => dest.ReturnedBy, opt => opt.MapFrom(src => src.Book.ReturnBy))
            .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(src =>
                    src.Book.Author != null
                        ? $"{src.Book.Author.FirstName} {src.Book.Author.LastName}"
                        : "Автор не найден"));

        CreateMap<UserBooksDto, UserBooks>()
            .ForMember(dest => dest.Book, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}