using System.ComponentModel.DataAnnotations;
using ElectronicsStore.Models;

namespace ElectronicsStore.ViewModels
{
    public class AdminDashboardViewModel
    {
        public DashboardStats Stats { get; set; } = new();
        public List<RecentOrder> RecentOrders { get; set; } = new();
        public List<TopProduct> TopProducts { get; set; } = new();
        public List<MonthlyRevenue> MonthlyRevenues { get; set; } = new();
        public List<CategoryStats> CategoryStats { get; set; } = new();
    }

    public class DashboardStats
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockProducts { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public decimal MonthRevenue { get; set; }
        public int MonthOrders { get; set; }
    }

    public class RecentOrder
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
    }

    public class TopProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int SoldCount { get; set; }
        public decimal Revenue { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class MonthlyRevenue
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class CategoryStats
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class AdminProductListViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public bool? IsActive { get; set; }
        public string SortBy { get; set; } = "name";
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 20;
    }

    public class AdminOrderListViewModel
    {
        public List<Order> Orders { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SortBy { get; set; } = "date";
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 20;
    }

    public class AdminUserListViewModel
    {
        public List<User> Users { get; set; } = new();
        public string? SearchTerm { get; set; }
        public UserRole? Role { get; set; }
        public bool? IsActive { get; set; }
        public string SortBy { get; set; } = "name";
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 20;
    }

    public class CreateAdminUserViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ là bắt buộc")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
        [Display(Name = "Họ")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        [Display(Name = "Tên")]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [Display(Name = "Vai trò")]
        public UserRole Role { get; set; } = UserRole.Admin;
    }

    public class AdminReportsViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int MonthlyOrders { get; set; }
        public List<TopSellingProduct> TopProducts { get; set; } = new();
        public List<MonthlySalesData> MonthlySales { get; set; } = new();
    }

    public class TopSellingProduct
    {
        public Product Product { get; set; } = new();
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlySalesData
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class AdminSettingsViewModel
    {
        [Display(Name = "Tên website")]
        public string SiteName { get; set; } = string.Empty;

        [Display(Name = "Mô tả website")]
        public string SiteDescription { get; set; } = string.Empty;

        [Display(Name = "Email liên hệ")]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string ContactPhone { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Cho phép đăng ký")]
        public bool AllowRegistration { get; set; }

        [Display(Name = "Yêu cầu xác nhận email")]
        public bool RequireEmailConfirmation { get; set; }

        [Display(Name = "Số sản phẩm mỗi trang")]
        public int DefaultPageSize { get; set; }

        [Display(Name = "Kích thước file tối đa (MB)")]
        public int MaxFileUploadSize { get; set; }

        [Display(Name = "Cho phép đánh giá")]
        public bool EnableReviews { get; set; }

        [Display(Name = "Tự động duyệt đánh giá")]
        public bool AutoApproveReviews { get; set; }
    }
}
