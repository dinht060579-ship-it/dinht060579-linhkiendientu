using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public class ProductAttribute
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string AttributeName { get; set; } = string.Empty; // VD: "Điện áp", "Công suất", "Kích thước"
        
        [Required]
        [StringLength(200)]
        public string AttributeValue { get; set; } = string.Empty; // VD: "5V", "10W", "10x15mm"
        
        [StringLength(20)]
        public string? Unit { get; set; } // VD: "V", "W", "mm"
        
        public int DisplayOrder { get; set; } = 0;
    }
}
