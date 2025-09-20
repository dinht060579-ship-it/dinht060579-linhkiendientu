using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;

namespace ElectronicsStore.Controllers
{
    public class UpdateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UpdateController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Update/Dates
        public async Task<IActionResult> Dates()
        {
            try
            {
                await UpdateAllDatesAsync();
                ViewBag.Message = "✅ Cập nhật thành công! Tất cả các ngày đã được cập nhật từ 1/9/2025 đến 21/9/2025";
                ViewBag.Success = true;
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"❌ Lỗi khi cập nhật: {ex.Message}";
                ViewBag.Success = false;
            }

            return View();
        }

        private async Task UpdateAllDatesAsync()
        {
            // Cập nhật ngày tạo người dùng
            await UpdateUserDates();

            // Cập nhật ngày tạo sản phẩm
            await UpdateProductDates();

            // Cập nhật ngày đơn hàng
            await UpdateOrderDates();

            // Cập nhật ngày đánh giá sản phẩm
            await UpdateReviewDates();

            await _context.SaveChangesAsync();
        }

        private async Task UpdateUserDates()
        {
            var users = await _context.Users.ToListAsync();
            var random = new Random();

            foreach (var user in users)
            {
                if (user.Email == "admin@store.com")
                {
                    // Admin được tạo trước
                    user.CreatedAt = new DateTime(2025, 8, 15);
                    user.LastLoginAt = new DateTime(2025, 9, 21, 10, 30, 0);
                }
                else
                {
                    // Khách hàng được tạo trong khoảng thời gian
                    var createdDate = new DateTime(2025, 9, 1).AddDays(random.Next(0, 20));
                    user.CreatedAt = createdDate;
                    user.LastLoginAt = createdDate.AddDays(random.Next(0, (new DateTime(2025, 9, 21) - createdDate).Days + 1));
                }
            }
        }

        private async Task UpdateProductDates()
        {
            var products = await _context.Products.ToListAsync();
            var random = new Random();

            foreach (var product in products)
            {
                // Sản phẩm được tạo trước khoảng thời gian đơn hàng
                product.CreatedAt = new DateTime(2025, 8, random.Next(1, 31));
                product.UpdatedAt = new DateTime(2025, 9, random.Next(1, 21));
            }
        }

        private async Task UpdateOrderDates()
        {
            // Xóa tất cả đơn hàng cũ
            var existingOrders = await _context.Orders.Include(o => o.OrderItems).ToListAsync();
            _context.Orders.RemoveRange(existingOrders);
            await _context.SaveChangesAsync();

            // Tạo lại đơn hàng với ngày mới
            await CreateOrdersForDateRange();
        }

        private async Task CreateOrdersForDateRange()
        {
            var customers = await _context.Users.Where(u => u.Role == UserRole.Customer).ToListAsync();
            var products = await _context.Products.Where(p => p.IsActive).ToListAsync();

            if (!customers.Any() || !products.Any()) return;

            var random = new Random();
            var orders = new List<Order>();

            // Tạo đơn hàng từ 1/9/2025 đến 21/9/2025
            var startDate = new DateTime(2025, 9, 1);
            var endDate = new DateTime(2025, 9, 21);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Nhiều đơn hàng hơn vào cuối tuần
                var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                var orderCount = isWeekend ? random.Next(2, 6) : random.Next(3, 10);

                for (int i = 0; i < orderCount; i++)
                {
                    var customer = customers[random.Next(customers.Count)];
                    var orderTime = date.AddHours(random.Next(8, 22)).AddMinutes(random.Next(0, 60));

                    var order = new Order
                    {
                        OrderNumber = $"DH{orderTime:yyyyMMdd}{random.Next(1000, 9999)}",
                        UserId = customer.Id,
                        User = customer,
                        CustomerName = customer.FullName,
                        CustomerEmail = customer.Email ?? "",
                        CustomerPhone = customer.PhoneNumber ?? "",
                        CreatedAt = orderTime,
                        OrderStatus = GetOrderStatus(random, orderTime),
                        PaymentStatus = "Paid",
                        ShippingAddress = customer.Address ?? "Địa chỉ giao hàng",
                        ShippingCity = customer.City ?? "TP.HCM",
                        ShippingPostalCode = customer.PostalCode ?? "70000",
                        PaymentMethod = GetPaymentMethod(random),
                        OrderItems = new List<OrderItem>()
                    };

                    // Thêm sản phẩm vào đơn hàng
                    var itemCount = random.Next(1, 5);
                    var selectedProducts = products.OrderBy(x => random.Next()).Take(itemCount).ToList();
                    decimal totalAmount = 0;

                    foreach (var product in selectedProducts)
                    {
                        var quantity = random.Next(1, 4);
                        var price = product.DiscountPrice ?? product.Price;

                        var orderItem = new OrderItem
                        {
                            Order = order,
                            ProductId = product.Id,
                            Product = product,
                            ProductName = product.Name,
                            ProductSKU = product.SKU,
                            Quantity = quantity,
                            UnitPrice = price,
                            TotalPrice = price * quantity
                        };

                        order.OrderItems.Add(orderItem);
                        totalAmount += orderItem.TotalPrice;
                    }

                    order.SubTotal = totalAmount;
                    order.TotalAmount = totalAmount;

                    // Phí ship cho đơn hàng dưới 500k
                    if (totalAmount < 500000)
                    {
                        order.ShippingFee = 30000;
                        order.TotalAmount += order.ShippingFee;
                    }

                    orders.Add(order);
                }
            }

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateReviewDates()
        {
            var reviews = await _context.ProductReviews.ToListAsync();
            var random = new Random();

            foreach (var review in reviews)
            {
                // Đánh giá được tạo trong khoảng thời gian
                review.CreatedAt = new DateTime(2025, 9, random.Next(1, 22));
            }
        }

        private static string GetOrderStatus(Random random, DateTime orderDate)
        {
            var currentDate = new DateTime(2025, 9, 21);
            var daysSinceOrder = (currentDate - orderDate).Days;
            
            if (daysSinceOrder > 15)
            {
                return random.NextDouble() < 0.9 ? "Completed" : "Cancelled";
            }
            else if (daysSinceOrder > 7)
            {
                return random.NextDouble() < 0.8 ? "Completed" : 
                       random.NextDouble() < 0.9 ? "Shipped" : "Cancelled";
            }
            else if (daysSinceOrder > 3)
            {
                return random.NextDouble() < 0.6 ? "Completed" :
                       random.NextDouble() < 0.8 ? "Shipped" :
                       random.NextDouble() < 0.9 ? "Processing" : "Pending";
            }
            else
            {
                return random.NextDouble() < 0.3 ? "Completed" :
                       random.NextDouble() < 0.5 ? "Shipped" :
                       random.NextDouble() < 0.7 ? "Processing" : "Pending";
            }
        }

        private static string GetPaymentMethod(Random random)
        {
            var methods = new[] { "COD", "Bank Transfer", "Credit Card", "E-Wallet" };
            return methods[random.Next(methods.Length)];
        }
    }
}