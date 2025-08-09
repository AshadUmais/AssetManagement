using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AssetMgmt.Models
{
    public class UserMaster
    {
        [Key] public int UserID { get; set; }
        [Required] public string FullName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        public string Designation { get; set; }
        public string AssignedBuilding { get; set; }
        public string AssignedFloor { get; set; }
        public string AssignedRoom { get; set; }
        public string PasswordHash { get; set; }

        public ICollection<Asset> CustodiedAssets { get; set; }
    }
}
