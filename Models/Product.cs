using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicsStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty; // Mã sản phẩm
        
        [StringLength(100)]
        public string? PartNumber { get; set; } // Part Number của nhà sản xuất
        
        [StringLength(1000)]
        public string? ShortDescription { get; set; }
        
        public string? DetailedDescription { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }
        
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; } = 5; // Mức tồn kho tối thiểu
        
        public string? MainImageUrl { get; set; }
        public string? ImageUrls { get; set; } // JSON array của các URL hình ảnh
        
        // Thông số kỹ thuật (JSON)
        public string? TechnicalSpecs { get; set; }
        
        // Tài liệu kỹ thuật
        public string? DatasheetUrl { get; set; }
        public string? DocumentUrls { get; set; } // JSON array của các URL tài liệu
        
        // Khóa ngoại
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
        
        // Trạng thái
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false; // Sản phẩm nổi bật
        public bool IsNewProduct { get; set; } = false;
        
        // Thống kê
        public int ViewCount { get; set; } = 0;
        public int SoldCount { get; set; } = 0;
        
        // Thời gian
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        // Computed properties
        public string StockStatus
        {
            get
            {
                if (StockQuantity <= 0) return "Hết hàng";
                if (StockQuantity <= MinStockLevel) return "Sắp hết hàng";
                return "Còn hàng";
            }
        }
        
        public decimal DisplayPrice => DiscountPrice ?? Price;
        
        public bool HasDiscount => DiscountPrice.HasValue && DiscountPrice < Price;
        
        public decimal? DiscountPercentage
        {
            get
            {
                if (!HasDiscount) return null;
                return Math.Round(((Price - DiscountPrice!.Value) / Price) * 100, 0);
            }
        }
    }
}
