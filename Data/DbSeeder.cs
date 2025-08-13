using System;
using System.Linq;
using AssetMgmt.Models;

namespace AssetMgmt.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext ctx)
        {
            ctx.Database.EnsureCreated();

            if (!ctx.Locations.Any())
            {
                var locs = new[]
                {
                    new Location { Region = "Abu Dhabi", Site = "Musaffah", Building = "Bldg-A", Floor = "G", Room = "R-101" },
                    new Location { Region = "Abu Dhabi", Site = "Musaffah", Building = "Bldg-A", Floor = "1", Room = "R-102" },
                    new Location { Region = "Dubai", Site = "JLT", Building = "Tower-B", Floor = "2", Room = "R-203" },
                    new Location { Region = "Sharjah", Site = "SAIF Zone", Building = "Office-1", Floor = "G", Room = "R-001" }
                };
                ctx.Locations.AddRange(locs);
                ctx.SaveChanges();
            }

            if (!ctx.UserMasters.Any())
            {
                var users = new[]
                {
                    new UserMaster { FullName = "Ahmed Al Mansoori", Email = "ahmed@example.com", Designation = "IT Coordinator", AssignedBuilding = "Bldg-A", AssignedFloor = "G", AssignedRoom = "R-101", PasswordHash = "pass123"  },
                    new UserMaster { FullName = "Fatima Al Suwaidi", Email = "fatima@example.com", Designation = "Finance Officer", AssignedBuilding = "Tower-B", AssignedFloor = "2", AssignedRoom = "R-203", PasswordHash = "pass123" }
                };
                ctx.UserMasters.AddRange(users);
                ctx.SaveChanges();
            }

            if (!ctx.Assets.Any())
            {
                var assets = new[]
                {
                    new Asset { AssetName = "Lenovo Laptop", AssetTagNumber = "AE-LTP-01",  LocationID = ctx.Locations.First().LocationID, CustodianID = ctx.UserMasters.First().UserID, PurchaseDate = new DateTime(2023,1,1), Status = "Active" },
                    new Asset { AssetName = "Office Chair", AssetTagNumber = "AE-CHR-02",  LocationID = ctx.Locations.First().LocationID, CustodianID = ctx.UserMasters.First().UserID, PurchaseDate = new DateTime(2022,5,10), Status = "Active" },
                    new Asset { AssetName = "Dell Monitor", AssetTagNumber = "AE-MTR-01", LocationID = ctx.Locations.Skip(2).First().LocationID, CustodianID = ctx.UserMasters.Skip(1).First().UserID, PurchaseDate = new DateTime(2021,11,5), Status = "Active" }
                };
                ctx.Assets.AddRange(assets);
                ctx.SaveChanges();
            }

            if (!ctx.AssetTransferLogs.Any())
            {
                ctx.AssetTransferLogs.Add(new AssetTransferLog
                {
                    AssetID = ctx.Assets.First().AssetID,
                    FromLocationID = ctx.Locations.First().LocationID,
                    ToLocationID = ctx.Locations.Skip(1).First().LocationID,
                    TransferredBy = ctx.UserMasters.First().UserID,
                    TransferDate = DateTime.UtcNow.AddDays(-10),
                    Remarks = "Room change"
                });
                ctx.SaveChanges();
            }
        }
    }
}
