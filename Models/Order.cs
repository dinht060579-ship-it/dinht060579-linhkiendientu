using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicsStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; } = string.Empty; // Mã đơn hàng
        
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        
        // Thông tin khách hàng
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string CustomerPhone { get; set; } = string.Empty;
        
        // Địa chỉ giao hàng
        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string ShippingCity { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? ShippingPostalCode { get; set; }
        
        // Thông tin đơn hàng
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; } // Tổng tiền hàng
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } // Phí vận chuyển
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0; // Số tiền giảm giá
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Tổng thanh toán
        
        // Phương thức thanh toán
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // COD, Bank Transfer, VNPay, etc.
        
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed
        
        // Trạng thái đơn hàng
        [StringLength(50)]
        public string OrderStatus { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        
        // Ghi chú
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(500)]
        public string? AdminNotes { get; set; }
        
        // Thời gian
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ProcessedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        
        // Navigation properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    
    public class OrderItem
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty; // Lưu tên sản phẩm tại thời điểm đặt hàng
        
        [Required]
        [StringLength(50)]
        public string ProductSKU { get; set; } = string.Empty; // Lưu SKU tại thời điểm đặt hàng
        
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Giá tại thời điểm đặt hàng
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; } // Quantity * UnitPrice
    }
}
