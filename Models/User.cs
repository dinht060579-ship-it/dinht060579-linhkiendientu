using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public enum UserRole
    {
        Customer = 0,
        Admin = 1,
        SuperAdmin = 2
    }
    public class User : IdentityUser
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        
        [StringLength(100)]
        public string? LastName { get; set; }
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        [StringLength(100)]
        public string? City { get; set; }
        
        [StringLength(20)]
        public string? PostalCode { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }

        // Role and permissions
        public UserRole Role { get; set; } = UserRole.Customer;
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public Cart? Cart { get; set; }
        
        // Computed properties
        public string FullName => $"{FirstName} {LastName}".Trim();
        public bool IsAdmin => Role == UserRole.Admin || Role == UserRole.SuperAdmin;
        public string RoleDisplayName => Role switch
        {
            UserRole.Customer => "Khách hàng",
            UserRole.Admin => "Quản trị viên",
            UserRole.SuperAdmin => "Quản trị viên cấp cao",
            _ => "Không xác định"
        };
    }
}
