using Microsoft.EntityFrameworkCore;
using AssetMgmt.Models;

namespace AssetMgmt.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions opts) : base(opts) { }

        public DbSet<Location> Locations { get; set; }
        public DbSet<UserMaster> UserMasters { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetTransferLog> AssetTransferLogs { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; } // ✅ Add this

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Asset → Location
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Location)
                .WithMany(l => l.Assets)
                .HasForeignKey(a => a.LocationID)
                .OnDelete(DeleteBehavior.SetNull);

            // Asset → Custodian
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Custodian)
                .WithMany(u => u.CustodiedAssets)
                .HasForeignKey(a => a.CustodianID)
                .OnDelete(DeleteBehavior.SetNull);

            // Asset → Category
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany()
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Asset → SubCategory
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.SubCategory)
                .WithMany()
                .HasForeignKey(a => a.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // SubCategory → Category
            modelBuilder.Entity<SubCategory>()
                .HasOne(sc => sc.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfer log → Asset
            modelBuilder.Entity<AssetTransferLog>()
                .HasOne(t => t.Asset)
                .WithMany()
                .HasForeignKey(t => t.AssetID);

            // Transfer log → From Location
            modelBuilder.Entity<AssetTransferLog>()
                .HasOne(t => t.FromLocation)
                .WithMany()
                .HasForeignKey(t => t.FromLocationID)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfer log → To Location
            modelBuilder.Entity<AssetTransferLog>()
                .HasOne(t => t.ToLocation)
                .WithMany()
                .HasForeignKey(t => t.ToLocationID)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfer log → Transferred By
            modelBuilder.Entity<AssetTransferLog>()
                .HasOne(t => t.TransferredByUser)
                .WithMany()
                .HasForeignKey(t => t.TransferredBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfer log → Transferred To
            modelBuilder.Entity<AssetTransferLog>()
                .HasOne(t => t.TransferredToUser)
                .WithMany()
                .HasForeignKey(t => t.TransferredTo)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
