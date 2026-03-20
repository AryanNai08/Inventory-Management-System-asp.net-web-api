using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public partial class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }
    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    public virtual DbSet<PurchaseOrderStatus> PurchaseOrderStatuses { get; set; }

    public virtual DbSet<SalesOrder> SalesOrders { get; set; }

    public virtual DbSet<SalesOrderItem> SalesOrderItems { get; set; }
    public virtual DbSet<SalesOrderStatus> SalesOrderStatuses { get; set; }

    public virtual DbSet<AdjustmentType> AdjustmentTypes { get; set; }
    public virtual DbSet<StockAdjustment> StockAdjustments { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRoles");
                    });
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);

            entity.HasMany(r => r.Privileges)
                .WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePrivilege",
                    r => r.HasOne<Privilege>().WithMany().HasForeignKey("PrivilegeId"),
                    l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    j =>
                    {
                        j.HasKey("RoleId", "PrivilegeId");
                        j.ToTable("RolePrivileges");
                    });
        });

        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07CAB3DFD1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsRevoked).HasDefaultValue(false);
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Users");
        });


        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC0723469CD3");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplier__3214EC077270E153");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ContactPerson).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Warehous__3214EC0701DB87F5");

            entity.HasIndex(e => e.Name, "UQ__Warehous__737584F62A1917AB").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07BAD9413A");

            entity.HasIndex(e => e.Sku, "UQ__Products__CA1ECF0DF6464C23").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.CurrentStock).HasDefaultValue(0);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supplier)
                            .WithMany()
                            .HasForeignKey(e => e.SupplierId)
                            .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Warehouse)
                            .WithMany()
                            .HasForeignKey(e => e.WarehouseId)
                            .OnDelete(DeleteBehavior.SetNull);

        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Purchase__3214EC0760B9A814");

            entity.HasIndex(e => e.OrderNumber, "UQ__Purchase__CAC5E7434E76D019").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Status).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Statu__41EDCAC5");

            entity.HasOne(d => d.Supplier).WithMany()
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__PurchaseO__Suppl__40058253");
        });

        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Purchase__3214EC078CE1D351");

            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([Quantity]*[UnitCost])", true)
                .HasColumnType("decimal(29, 2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(d => d.PurchaseOrderId)
                .HasConstraintName("FK__PurchaseO__Purch__46B27FE2");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__PurchaseO__Produ__47A6A41B");
        });

        modelBuilder.Entity<PurchaseOrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Purchase__3214EC07CF422035");

            entity.HasIndex(e => e.Name, "UQ__Purchase__737584F6E3F9E1A0").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });


        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesOrd__3214EC0723D8332F");

            entity.HasIndex(e => e.OrderNumber, "UQ__SalesOrd__CAC5E743B2B7A032").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Status).WithMany(p => p.SalesOrders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesOrde__Statu__55009F39");

            entity.HasOne(d => d.Customer).WithMany()
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__SalesOrde__Custo__540C7B00");
        });

        modelBuilder.Entity<SalesOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesOrd__3214EC07B23B3E00");

            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(29, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.SalesOrder).WithMany(p => p.SalesOrderItems)
                .HasForeignKey(d => d.SalesOrderId)
                .HasConstraintName("FK__SalesOrde__Sales__59C55456");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__SalesOrde__Produ__5AB9788F");
        });

        modelBuilder.Entity<SalesOrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesOrd__3214EC070CC03D19");

            entity.HasIndex(e => e.Name, "UQ__SalesOrd__737584F6B01DF097").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<AdjustmentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Adjustme__3214EC07CA927496");
            entity.HasIndex(e => e.Name, "UQ__Adjustme__737584F6A6F57F3D").IsUnique();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StockAdj__3214EC07139D96DB");
            entity.Property(e => e.AdjustmentDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Reason).HasMaxLength(500);

            entity.HasOne(d => d.AdjustmentType).WithMany(p => p.StockAdjustments)
                .HasForeignKey(d => d.AdjustmentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockAdju__Adjus__625A9A57");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__StockAdju__Produ__607251E5");

            entity.HasOne(d => d.Warehouse).WithMany()
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__StockAdju__Wareh__6166761E");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.AdjustedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__StockAdju__Adjus__634EBE90");
        });
    }
}