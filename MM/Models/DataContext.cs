using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MM.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<AccountDAO> Account { get; set; }
        public virtual DbSet<AccountFoodFavoriteDAO> AccountFoodFavorite { get; set; }
        public virtual DbSet<FoodDAO> Food { get; set; }
        public virtual DbSet<FoodFoodGroupingMappingDAO> FoodFoodGroupingMapping { get; set; }
        public virtual DbSet<FoodFoodTypeMappingDAO> FoodFoodTypeMapping { get; set; }
        public virtual DbSet<FoodGroupingDAO> FoodGrouping { get; set; }
        public virtual DbSet<FoodTypeDAO> FoodType { get; set; }
        public virtual DbSet<NotificaitonDAO> Notificaiton { get; set; }
        public virtual DbSet<OrderDAO> Order { get; set; }
        public virtual DbSet<OrderContentDAO> OrderContent { get; set; }
        public virtual DbSet<TableDAO> Table { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=112.137.129.216,1699;initial catalog=MM;persist security info=True;user id=sa;password=123456a@;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountDAO>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(2000);

                entity.Property(e => e.CreatedAt).HasColumnType("date");

                entity.Property(e => e.DeletedAt).HasColumnType("date");

                entity.Property(e => e.DisplayName).HasMaxLength(500);

                entity.Property(e => e.Dob).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ExpiredTimeCode).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PasswordRecoveryCode).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Salt).HasMaxLength(2000);

                entity.Property(e => e.UpdatedAt).HasColumnType("date");
            });

            modelBuilder.Entity<AccountFoodFavoriteDAO>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.FoodId });

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountFoodFavorites)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountFoodFavorite_Account");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.AccountFoodFavorites)
                    .HasForeignKey(d => d.FoodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountFoodFavorite_Food");
            });

            modelBuilder.Entity<FoodDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("date");

                entity.Property(e => e.DeletedAt).HasColumnType("date");

                entity.Property(e => e.Descreption).HasMaxLength(2000);

                entity.Property(e => e.DiscountRate).HasColumnType("decimal(10, 4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.PriceEach).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UpdatedAt).HasColumnType("date");
            });

            modelBuilder.Entity<FoodFoodGroupingMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.FoodId, e.FoodGroupingId });

                entity.HasOne(d => d.FoodGrouping)
                    .WithMany(p => p.FoodFoodGroupingMappings)
                    .HasForeignKey(d => d.FoodGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FoodFoodGroupingMapping_FoodGrouping");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.FoodFoodGroupingMappings)
                    .HasForeignKey(d => d.FoodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FoodFoodGroupingMapping_Food");
            });

            modelBuilder.Entity<FoodFoodTypeMappingDAO>(entity =>
            {
                entity.HasOne(d => d.Food)
                    .WithMany(p => p.FoodFoodTypeMappings)
                    .HasForeignKey(d => d.FoodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FoodFoodTypeMapping_Food");

                entity.HasOne(d => d.FoodType)
                    .WithMany(p => p.FoodFoodTypeMappings)
                    .HasForeignKey(d => d.FoodTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FoodFoodTypeMapping_FoodType");
            });

            modelBuilder.Entity<FoodGroupingDAO>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<FoodTypeDAO>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<NotificaitonDAO>(entity =>
            {
                entity.Property(e => e.Content).HasMaxLength(500);

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Notificaitons)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notificaiton_Account");
            });

            modelBuilder.Entity<OrderDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Descreption).HasMaxLength(2000);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PayDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Customer");
            });

            modelBuilder.Entity<OrderContentDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.FoodFoodTypeMapping)
                    .WithMany(p => p.OrderContents)
                    .HasForeignKey(d => d.FoodFoodTypeMappingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderContent_FoodFoodTypeMapping");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderContents)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderContent_Order");
            });

            modelBuilder.Entity<TableDAO>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Tables)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_Table_Order");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
