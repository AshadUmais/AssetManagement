using System;
using System.ComponentModel.DataAnnotations;

namespace AssetMgmt.Models
{
    public class AssetTransferLog
    {
        [Key]
        public int TransferID { get; set; }
        public int AssetID { get; set; }
        public Asset Asset { get; set; }

        public int FromLocationID { get; set; }
        public Location FromLocation { get; set; }

        public int ToLocationID { get; set; }
        public Location ToLocation { get; set; }
        public int TransferredTo { get; set; }
        public UserMaster TransferredToUser { get; set; }

        public int TransferredBy { get; set; }
        public UserMaster TransferredByUser { get; set; }

        public DateTime TransferDate { get; set; }
        public string Remarks { get; set; }
    }
}
