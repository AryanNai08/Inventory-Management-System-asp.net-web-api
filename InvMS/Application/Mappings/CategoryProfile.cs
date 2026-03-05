using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappings
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile() 
        {
            CreateMap<Category,CategoryDto>();
            CreateMap<CreateCategoryDto,Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore()); 
            CreateMap<UpdateCategoryDto, Category>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore()); ;
        }
    }
}
