﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Keytietkiem.Models;

public partial class KeytietkiemDbContext : DbContext
{
    public KeytietkiemDbContext()
    {
    }

    public KeytietkiemDbContext(DbContextOptions<KeytietkiemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Badge> Badges { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductBadge> ProductBadges { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductKey> ProductKeys { get; set; }

    public virtual DbSet<RefundRequest> RefundRequests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketReply> TicketReplies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WarrantyClaim> WarrantyClaims { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:MyCnn");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasIndex(e => e.UserId, "UQ__Accounts__1788CC4D419BAFE0").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Accounts__536C85E42AF68A9A").IsUnique();

            entity.Property(e => e.AccountId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LastLoginAt).HasPrecision(3);
            entity.Property(e => e.LockedUntil).HasPrecision(3);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.UpdatedAt).HasPrecision(3);
            entity.Property(e => e.Username).HasMaxLength(60);

            entity.HasOne(d => d.User).WithOne(p => p.Account)
                .HasForeignKey<Account>(d => d.UserId)
                .HasConstraintName("FK_Accounts_User");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__9C6270E81527D8E2");

            entity.Property(e => e.ArticleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(120);

            entity.HasOne(d => d.Author).WithMany(p => p.Articles)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Articles_Author");

            entity.HasMany(d => d.Tags).WithMany(p => p.Articles)
                .UsingEntity<Dictionary<string, object>>(
                    "ArticleTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ArticleTags_Tag"),
                    l => l.HasOne<Article>().WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ArticleTags_Article"),
                    j =>
                    {
                        j.HasKey("ArticleId", "TagId");
                        j.ToTable("ArticleTags");
                    });
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F2398A18807CA");

            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ActorEmail).HasMaxLength(254);
            entity.Property(e => e.EntityId).HasMaxLength(128);
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.OccurredAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Resource)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserAgent).HasMaxLength(200);
        });

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.BadgeCode).HasName("PK__Badges__8BF404E7E8E58195");

            entity.Property(e => e.BadgeCode).HasMaxLength(32);
            entity.Property(e => e.ColorHex)
                .HasMaxLength(9)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DisplayName).HasMaxLength(64);
            entity.Property(e => e.Icon).HasMaxLength(64);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B3D87958B");

            entity.HasIndex(e => e.CategoryCode, "UQ__Categori__371BA9557A9ED073").IsUnique();

            entity.Property(e => e.CategoryCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasPrecision(3);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("PK__Modules__2B7477A7E4CEE4DD");

            entity.HasIndex(e => e.ModuleName, "UQ__Modules__EAC9AEC36273EAF2").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.ModuleName).HasMaxLength(80);
            entity.Property(e => e.UpdatedAt).HasPrecision(3);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFF3D8A7BE");

            entity.Property(e => e.OrderId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.FinalAmount)
                .HasComputedColumnSql("([TotalAmount]-[DiscountAmount])", true)
                .HasColumnType("decimal(13, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_User");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D36CACBE2BC8");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Key).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.KeyId)
                .HasConstraintName("FK_OrderDetails_Key");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Product");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38A839FADE");

            entity.Property(e => e.PaymentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Order");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB2F30590168");

            entity.HasIndex(e => e.PermissionName, "UQ__Permissi__0FFDA3579892B162").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.PermissionName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasPrecision(3);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CD878185B6");

            entity.HasIndex(e => e.ProductCode, "UQ__Products__2F4E024F4FF26E21").IsUnique();

            entity.Property(e => e.ProductId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CostPrice).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.ProductType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SalePrice).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(512);
            entity.Property(e => e.UpdatedAt).HasPrecision(3);

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Supplier");

            entity.HasMany(d => d.Categories).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_ProductCategories_Category"),
                    l => l.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductCategories_Product"),
                    j =>
                    {
                        j.HasKey("ProductId", "CategoryId");
                        j.ToTable("ProductCategories");
                    });
        });

        modelBuilder.Entity<ProductBadge>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.Badge });

            entity.Property(e => e.Badge).HasMaxLength(32);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.BadgeNavigation).WithMany(p => p.ProductBadges)
                .HasForeignKey(d => d.Badge)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductBadges_Badges");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductBadges)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductBadges_Products");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__ProductI__7516F70C233B7C73");

            entity.HasIndex(e => new { e.ProductId, e.SortOrder }, "IX_ProductImages_Product_Sort");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Url).HasMaxLength(512);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductImages_Products");
        });

        modelBuilder.Entity<ProductKey>(entity =>
        {
            entity.HasKey(e => e.KeyId).HasName("PK__ProductK__21F5BE471DEEF99E");

            entity.HasIndex(e => e.KeyString, "UQ__ProductK__BE7B08A92F7D8864").IsUnique();

            entity.Property(e => e.KeyId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ImportedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.KeyString).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductKeys)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductKeys_Product");
        });

        modelBuilder.Entity<RefundRequest>(entity =>
        {
            entity.HasKey(e => e.RefundId).HasName("PK__RefundRe__725AB92000819157");

            entity.Property(e => e.RefundId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.SubmittedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Order).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefundRequests_Order");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A7920A3EA");

            entity.Property(e => e.RoleId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(60);
            entity.Property(e => e.UpdatedAt).HasPrecision(3);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId, e.ModuleId });

            entity.Property(e => e.RoleId).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Module).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_RolePermissions_Module");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK_RolePermissions_Perm");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RolePermissions_Role");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B44F7A42A5");

            entity.Property(e => e.ContactEmail).HasMaxLength(254);
            entity.Property(e => e.ContactPhone).HasMaxLength(32);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__657CF9AC1B1C0174");

            entity.HasIndex(e => e.TagName, "UQ__Tags__BDE0FD1DFD8B4FBB").IsUnique();

            entity.Property(e => e.TagName).HasMaxLength(50);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__712CC6070FF23DDC");

            entity.Property(e => e.TicketId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Subject).HasMaxLength(120);

            entity.HasOne(d => d.User).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tickets_User");
        });

        modelBuilder.Entity<TicketReply>(entity =>
        {
            entity.HasKey(e => e.ReplyId).HasName("PK__TicketRe__C25E460980D04417");

            entity.Property(e => e.SentAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Sender).WithMany(p => p.TicketReplies)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketReplies_User");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketReplies)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketReplies_Ticket");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534A06274DC").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(254);
            entity.Property(e => e.FirstName).HasMaxLength(80);
            entity.Property(e => e.FullName).HasMaxLength(160);
            entity.Property(e => e.LastName).HasMaxLength(80);
            entity.Property(e => e.Phone).HasMaxLength(32);
            entity.Property(e => e.Status)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasDefaultValue("Active");
            entity.Property(e => e.UpdatedAt).HasPrecision(3);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserRoles_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRoles_User"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRoles");
                        j.IndexerProperty<string>("RoleId").HasMaxLength(50);
                    });
        });

        modelBuilder.Entity<WarrantyClaim>(entity =>
        {
            entity.HasKey(e => e.ClaimId).HasName("PK__Warranty__EF2E139BDB4857C0");

            entity.Property(e => e.ClaimId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.SubmittedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.WarrantyClaims)
                .HasForeignKey(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WarrantyClaims_OrderDetail");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
