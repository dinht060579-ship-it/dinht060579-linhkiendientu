using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using ElectronicsStore.ViewModels;
using System.Security.Claims;

namespace ElectronicsStore.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if user is admin
        private bool IsUserAdmin()
        {
            if (!User.Identity!.IsAuthenticated)
                return false;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = _context.Users.Find(userId);
            return user?.IsAdmin == true;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new AdminDashboardViewModel();

            // Get dashboard stats
            model.Stats = new DashboardStats
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalCustomers = await _context.Users.Where(u => u.Role == UserRole.Customer).CountAsync(),
                TotalRevenue = await _context.Orders
                    .Where(o => o.OrderStatus == "Completed")
                    .SumAsync(o => o.TotalAmount),
                PendingOrders = await _context.Orders
                    .Where(o => o.OrderStatus == "Pending")
                    .CountAsync(),
                LowStockProducts = await _context.Products
                    .Where(p => p.StockQuantity <= p.MinStockLevel)
                    .CountAsync(),
                TodayRevenue = await _context.Orders
                    .Where(o => o.CreatedAt.Date == DateTime.Today && o.OrderStatus == "Completed")
                    .SumAsync(o => o.TotalAmount),
                TodayOrders = await _context.Orders
                    .Where(o => o.CreatedAt.Date == DateTime.Today)
                    .CountAsync(),
                MonthRevenue = await _context.Orders
                    .Where(o => o.CreatedAt.Month == DateTime.Now.Month &&
                               o.CreatedAt.Year == DateTime.Now.Year &&
                               o.OrderStatus == "Completed")
                    .SumAsync(o => o.TotalAmount),
                MonthOrders = await _context.Orders
                    .Where(o => o.CreatedAt.Month == DateTime.Now.Month && 
                               o.CreatedAt.Year == DateTime.Now.Year)
                    .CountAsync()
            };

            // Get recent orders
            var recentOrdersData = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .Select(o => new RecentOrder
                {
                    Id = o.Id,
                    CustomerName = o.User.FullName,
                    CustomerEmail = o.User.Email ?? "",
                    TotalAmount = o.TotalAmount,
                    Status = o.OrderStatus,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
                })
                .ToListAsync();

            model.RecentOrders = recentOrdersData
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .ToList();

            // Get top products
            var topProductsData = await _context.Products
                .Include(p => p.Category)
                .Select(p => new TopProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.MainImageUrl ?? "",
                    Price = p.Price,
                    SoldCount = p.SoldCount,
                    Revenue = p.SoldCount * p.Price,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();

            model.TopProducts = topProductsData
                .OrderByDescending(p => p.SoldCount)
                .Take(10)
                .ToList();

            // Get monthly revenue (last 6 months)
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var monthlyData = await _context.Orders
                .Where(o => o.CreatedAt >= sixMonthsAgo && o.OrderStatus == "Completed")
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .ToListAsync();

            model.MonthlyRevenues = monthlyData
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .Select(g => new MonthlyRevenue
                {
                    Month = $"{g.Month:00}/{g.Year}",
                    Revenue = g.Revenue,
                    OrderCount = g.OrderCount
                }).ToList();

            // Get category stats
            var categoryStatsData = await _context.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryStats
                {
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count(p => p.IsActive),
                    Revenue = c.Products.Sum(p => p.SoldCount * p.Price),
                    OrderCount = c.Products.Sum(p => p.SoldCount)
                })
                .Where(cs => cs.ProductCount > 0)
                .ToListAsync();

            model.CategoryStats = categoryStatsData
                .OrderByDescending(cs => cs.Revenue)
                .ToList();

            return View(model);
        }

        // GET: Admin/Products
        public async Task<IActionResult> Products(string? search, int? categoryId, int? brandId, 
            bool? isActive, string sortBy = "name", int page = 1, int pageSize = 20)
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || 
                                        p.SKU.Contains(search) ||
                                        p.PartNumber.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "name" => query.OrderBy(p => p.Name),
                "price" => query.OrderBy(p => p.Price),
                "stock" => query.OrderBy(p => p.StockQuantity),
                "sold" => query.OrderByDescending(p => p.SoldCount),
                "created" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new AdminProductListViewModel
            {
                Products = products,
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync(),
                Brands = await _context.Brands.Where(b => b.IsActive).ToListAsync(),
                SearchTerm = search,
                CategoryId = categoryId,
                BrandId = brandId,
                IsActive = isActive,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize
            };

            return View(model);
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Orders(string? search, string? status, 
            DateTime? fromDate, DateTime? toDate, string sortBy = "date", int page = 1, int pageSize = 20)
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.User.FullName.Contains(search) || 
                                        o.User.Email!.Contains(search) ||
                                        o.Id.ToString().Contains(search));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt.Date <= toDate.Value.Date);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "date" => query.OrderByDescending(o => o.CreatedAt),
                "amount" => query.OrderByDescending(o => o.TotalAmount),
                "customer" => query.OrderBy(o => o.User.FullName),
                "status" => query.OrderBy(o => o.OrderStatus),
                _ => query.OrderByDescending(o => o.CreatedAt)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new AdminOrderListViewModel
            {
                Orders = orders,
                SearchTerm = search,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize
            };

            return View(model);
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users(string? search, UserRole? role, bool? isActive, 
            string sortBy = "name", int page = 1, int pageSize = 20)
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Users.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.FullName.Contains(search) || 
                                        u.Email!.Contains(search) ||
                                        u.PhoneNumber!.Contains(search));
            }

            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "name" => query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "email" => query.OrderBy(u => u.Email),
                "role" => query.OrderBy(u => u.Role),
                "created" => query.OrderByDescending(u => u.CreatedAt),
                "lastlogin" => query.OrderByDescending(u => u.LastLoginAt),
                _ => query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new AdminUserListViewModel
            {
                Users = users,
                SearchTerm = search,
                Role = role,
                IsActive = isActive,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize
            };

            return View(model);
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Categories()
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            // Get product count for each category
            foreach (var category in categories)
            {
                category.ProductCount = await _context.Products
                    .CountAsync(p => p.CategoryId == category.Id && p.IsActive);
            }

            return View(categories);
        }

        // GET: Admin/Brands
        public async Task<IActionResult> Brands()
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var brands = await _context.Brands
                .OrderBy(b => b.Name)
                .ToListAsync();

            // Get product count for each brand
            foreach (var brand in brands)
            {
                brand.ProductCount = await _context.Products
                    .CountAsync(p => p.BrandId == brand.Id && p.IsActive);
            }

            return View(brands);
        }

        // GET: Admin/Reports
        public async Task<IActionResult> Reports()
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new AdminReportsViewModel
            {
                // Sales statistics
                TotalRevenue = await _context.Orders
                    .Where(o => o.OrderStatus == "Delivered")
                    .SumAsync(o => o.TotalAmount),

                MonthlyRevenue = await _context.Orders
                    .Where(o => o.OrderStatus == "Delivered" &&
                               o.CreatedAt.Month == DateTime.Now.Month &&
                               o.CreatedAt.Year == DateTime.Now.Year)
                    .SumAsync(o => o.TotalAmount),

                TotalOrders = await _context.Orders.CountAsync(),

                MonthlyOrders = await _context.Orders
                    .Where(o => o.CreatedAt.Month == DateTime.Now.Month &&
                               o.CreatedAt.Year == DateTime.Now.Year)
                    .CountAsync(),

                // Top selling products
                TopProducts = await _context.OrderItems
                    .Include(oi => oi.Product)
                    .GroupBy(oi => oi.Product)
                    .Select(g => new TopSellingProduct
                    {
                        Product = g.Key,
                        TotalQuantitySold = g.Sum(oi => oi.Quantity),
                        TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                    })
                    .OrderByDescending(x => x.TotalQuantitySold)
                    .Take(10)
                    .ToListAsync(),

                // Monthly sales data for chart
                MonthlySales = await GetMonthlySalesData()
            };

            return View(model);
        }

        // GET: Admin/Settings
        public IActionResult Settings()
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new AdminSettingsViewModel
            {
                SiteName = "Electronics Store",
                SiteDescription = "Cửa hàng linh kiện điện tử uy tín",
                ContactEmail = "admin@electronicsstore.com",
                ContactPhone = "0123456789",
                Address = "123 Đường ABC, Quận XYZ, TP.HCM",
                AllowRegistration = true,
                RequireEmailConfirmation = false,
                DefaultPageSize = 20,
                MaxFileUploadSize = 5, // MB
                EnableReviews = true,
                AutoApproveReviews = false
            };

            return View(model);
        }

        private async Task<List<MonthlySalesData>> GetMonthlySalesData()
        {
            var result = new List<MonthlySalesData>();
            var currentDate = DateTime.Now;

            for (int i = 11; i >= 0; i--)
            {
                var targetDate = currentDate.AddMonths(-i);
                var revenue = await _context.Orders
                    .Where(o => o.OrderStatus == "Delivered" &&
                               o.CreatedAt.Month == targetDate.Month &&
                               o.CreatedAt.Year == targetDate.Year)
                    .SumAsync(o => o.TotalAmount);

                result.Add(new MonthlySalesData
                {
                    Month = targetDate.ToString("MM/yyyy"),
                    Revenue = revenue
                });
            }

            return result;
        }
    }
}
