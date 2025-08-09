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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Location)
                .WithMany(l => l.Assets)
                .HasForeignKey(a => a.LocationID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Custodian)
                .WithMany(u => u.CustodiedAssets)
                .HasForeignKey(a => a.CustodianID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AssetTransferLog>()
                .HasOne(t => t.Asset)
                .WithMany()
                .HasForeignKey(t => t.AssetID);

            modelBuilder.Entity<AssetTransferLog>()
                .HasOne(t => t.FromLocation)
                .WithMany()
                .HasForeignKey(t => t.FromLocationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetTransferLog>()
                .HasOne(t => t.ToLocation)
                .WithMany()
                .HasForeignKey(t => t.ToLocationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetTransferLog>()
                .HasOne(t => t.TransferredByUser)
                .WithMany()
                .HasForeignKey(t => t.TransferredBy)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
