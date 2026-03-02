using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappings
{
    public class UserProfile : Profile
    {

        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterDto,User>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                    .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());
            CreateMap<UpdateUserDto, User>();


        }
    }
}
