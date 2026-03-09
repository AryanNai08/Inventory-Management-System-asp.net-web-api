using Application.DTOs.RolesAndPrivileges;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>();

            CreateMap<RoleDto, Role>()
                  .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());

            CreateMap<UpdateRoleDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
        }

    }
}
