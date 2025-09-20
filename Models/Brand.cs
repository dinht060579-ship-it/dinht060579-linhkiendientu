using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicsStore.Models
{
    public class Brand
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        
        public ICollection<Product> Products { get; set; } = new List<Product>();
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Not mapped property for display purposes
        [NotMapped]
        public int ProductCount { get; set; }
    }
}
