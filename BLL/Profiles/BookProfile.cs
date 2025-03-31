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
         .ForMember(dest => dest.ImagePath, opt => opt.Ignore());

        CreateMap<UpdateBookDto, Book>()
            .ForMember(dest => dest.ImagePath, opt => opt.Ignore());

        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.Ignore())
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath));

        CreateMap<UserBooks, UserBooksDto>().ReverseMap();
    }
}
