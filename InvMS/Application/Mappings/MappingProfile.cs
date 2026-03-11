using Application.DTOs.Auth;
using Application.DTOs.Category;
using Application.DTOs.Customer;
using Application.DTOs.RolesAndPrivileges;
using Application.DTOs.Supplier;
using Application.DTOs.Warehouse;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<User, UserDto>();
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Username, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

            // Category Mappings
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());
            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

            // Role Mappings
            CreateMap<Role, RoleDto>();
            CreateMap<RoleDto, Role>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
            CreateMap<UpdateRoleDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());


            //Privilege Mapping
            // Read
            CreateMap<Privilege, ReadPrivilegeDto>();
            // Create & Update
            CreateMap<PrivilegeDto, Privilege>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //for roleprivilage
            CreateMap<Privilege, PrivilegeDto>();

            //Supplier
            //read
            CreateMap<Supplier, SupplierDto>();
            //create
            CreateMap<CreateSupplierDto, Supplier>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());
            //update
            CreateMap<UpdateSupplierDto, Supplier>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

            //customer
            //read
            CreateMap<Customer, CustomerDto>();
            //create
            CreateMap<CreateCustomerDto, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());
            //update
            CreateMap<UpdateCustomerDto, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());


            //Warehouse
            //read
            CreateMap<Warehouse, WarehouseDto>();
            //create
            CreateMap<CreateWarehouseDto, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());
            //update
            CreateMap<UpdateWarehouseDto, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

        }
    }
}
