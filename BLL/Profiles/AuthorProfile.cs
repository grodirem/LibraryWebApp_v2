using AutoMapper;
using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using DAL.Models;

namespace BLL.Profiles;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<Author, AuthorDto>().ReverseMap();
        CreateMap<CreateAuthorDto, Author>().ReverseMap();
        CreateMap<UpdateAuthorDto, Author>().ReverseMap();
    }
}
