using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public class Cart
    {
        public int Id { get; set; }
        
        public string? UserId { get; set; }
        // public User? User { get; set; } // Bỏ navigation property để tránh foreign key constraint
        
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        // Computed properties
        public decimal TotalAmount => CartItems.Sum(item => item.TotalPrice);
        public int TotalItems => CartItems.Sum(item => item.Quantity);
    }
    
    public class CartItem
    {
        public int Id { get; set; }
        
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Giá tại thời điểm thêm vào giỏ
        
        public DateTime AddedAt { get; set; } = DateTime.Now;
        
        // Computed properties
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
