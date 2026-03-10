using Application.Common;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public static class DependencyInjection  
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
        {

            //services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPrivilegeService, PrivilegeService>();
            services.AddScoped<IRolePrivilegeService, RolePrivilegeService>();
            services.AddScoped<ISupplierService, SupplierService>();
            //register api response
            services.AddScoped<APIResponse>();

            //automapper register
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
