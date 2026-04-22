using Application.DTOs.Auth;
using Application.DTOs.Category;
using Application.DTOs.Customer;
using Application.DTOs.Product;
using Application.DTOs.PurchaseOrder;
using Application.DTOs.RolesAndPrivileges;
using Application.DTOs.SalesOrder;
using Application.DTOs.StockAdjustment;
using Application.DTOs.Supplier;
using Application.DTOs.Warehouse;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name)));
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


            //Product
            // Product
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
                .ForMember(dest => dest.TotalStock, opt => opt.MapFrom(src => src.ProductWarehouseStocks.Sum(s => (int?)s.Quantity) ?? 0))
                .ForMember(dest => dest.StockStatus, opt => opt.MapFrom(src => 
                    StockStatusHelper.GetStatus(src.ProductWarehouseStocks.Sum(s => (int?)s.Quantity) ?? 0, src.ReorderLevel)))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion != null ? Convert.ToBase64String(src.RowVersion) : null));

            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore());
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore());  // handled manually in service

            // Read Model Mapping
            CreateMap<Domain.Models.ProductReadModel, ProductDto>()
                .ForMember(dest => dest.StockStatus, opt => opt.MapFrom(src =>
                    StockStatusHelper.GetStatus(src.TotalStock, src.ReorderLevel)))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)));

            //purchaseorder
            //create
            CreateMap<CreatePurchaseOrderDto, PurchaseOrder>();
            //createpurchaseorderitem
            CreateMap<CreatePurchaseOrderItemDto, PurchaseOrderItem>();
            //readpurchaseorder
            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : "Unknown Warehouse"))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name));
            //readpurchaseorderitem
            CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.Sku));

            // Purchase Order Mappings
            CreateMap<CreatePurchaseOrderDto, PurchaseOrder>()
                .ForMember(dest => dest.PurchaseOrderItems, opt => opt.MapFrom(src => src.Items));
            CreateMap<CreatePurchaseOrderItemDto, PurchaseOrderItem>();

            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : "Unknown Supplier"))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : "Unknown Warehouse"))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : "Unknown Status"))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PurchaseOrderItems));
                
            CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Unknown Product"))
                .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product != null ? src.Product.Sku : "Unknown SKU"));

            // Sales Order Mappings
            CreateMap<CreateSalesOrderDto, SalesOrder>()
                .ForMember(dest => dest.SalesOrderItems, opt => opt.MapFrom(src => src.Items));
            CreateMap<CreateSalesOrderItemDto, SalesOrderItem>();

            CreateMap<SalesOrder, SalesOrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : "Unknown Customer"))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : "Unknown Warehouse"))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : "Unknown Status"))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SalesOrderItems));

            CreateMap<SalesOrderItem, SalesOrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Unknown Product"))
                .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product != null ? src.Product.Sku : "Unknown SKU"));

            // Stock Adjustment Mappings
            CreateMap<AdjustmentType, AdjustmentTypeDto>();
            CreateMap<StockAdjustment, StockAdjustmentDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Unknown Product"))
                .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product != null ? src.Product.Sku : "Unknown SKU"))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : "Unknown Warehouse"))
                .ForMember(dest => dest.AdjustmentTypeName, opt => opt.MapFrom(src => src.AdjustmentType != null ? src.AdjustmentType.Name : "Unknown Type"))
                .ForMember(dest => dest.AdjustedByUserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : "Unknown User"));

            CreateMap<CreateStockAdjustmentDto, StockAdjustment>();
        }
    }
}
