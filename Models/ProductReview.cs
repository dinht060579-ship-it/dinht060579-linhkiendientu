using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Range(1, 5)]
        public int Rating { get; set; } // 1-5 sao
        
        [StringLength(100)]
        public string? Title { get; set; }
        
        [StringLength(1000)]
        public string? Comment { get; set; }
        
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
