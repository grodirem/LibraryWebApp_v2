using AutoMapper;
using BLL.DTOs.Requests;
using BLL.DTOs.Responses;
using DAL.Models;

namespace BLL.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<User, AuthResponseDto>()
           .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken))
           .ForMember(dest => dest.IsAuthenticated, opt => opt.MapFrom(src => true))
           .ForMember(dest => dest.ErrorMessage, opt => opt.Ignore())
           .ForMember(dest => dest.Token, opt => opt.Ignore());
    }
}
