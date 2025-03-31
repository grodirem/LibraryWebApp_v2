using AutoMapper;
using BLL.DTOs.Models;
using DAL.Models;

namespace BLL.Profiles;

public class UserBooksProfile : Profile
{
    public UserBooksProfile()
    {
        CreateMap<UserBooks, UserBooksDto>().ReverseMap();
    }
}
