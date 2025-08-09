using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AssetMgmt.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        [Required] public string Region { get; set; }
        [Required] public string Site { get; set; }
        [Required] public string Building { get; set; }
        [Required] public string Floor { get; set; }
        [Required] public string Room { get; set; }

        public ICollection<Asset> Assets { get; set; }
    }
}
