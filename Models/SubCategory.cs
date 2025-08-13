using System.ComponentModel.DataAnnotations;

namespace AssetMgmt.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryId { get; set; }
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}

