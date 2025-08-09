using System;
using System.ComponentModel.DataAnnotations;

namespace AssetMgmt.Models
{
    public class Asset
    {
        [Key]
        public int AssetID { get; set; }
        [Required] public string AssetName { get; set; }
        [Required] public string AssetTagNumber { get; set; }
        public string MajorCategory { get; set; }
        public string SubCategory { get; set; }

        public int? LocationID { get; set; }
        public Location Location { get; set; }

        public int? CustodianID { get; set; }
        public UserMaster Custodian { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public string Status { get; set; }
    }
}
