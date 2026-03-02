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
            

            //automapper register
            services.AddAutoMapper(typeof(UserProfile));

            return services;
        }
    }
}
