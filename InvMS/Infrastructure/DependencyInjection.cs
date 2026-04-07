using Domain.Interfaces;
using Domain.Interfaces.Auth;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.ThirdPartyServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();

            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("InventoryDb")));

            // Current user service (reads JWT claims for audit fields)
            services.AddScoped<CurrentUserService>();
            services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());

            //repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPrivilegeRepository, PrivilegeRepository>();
            services.AddScoped<IRolePrivilegeRepository, RolePrivilegeRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
            services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();
            services.AddScoped<IProductWarehouseStockRepository, ProductWarehouseStockRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IReportPDFService,ReportPDFService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
