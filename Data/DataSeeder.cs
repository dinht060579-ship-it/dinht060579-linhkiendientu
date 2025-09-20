using ElectronicsStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Admin User
            await SeedAdminUser(context);

            // Seed Sample Orders with realistic dates
            await SeedSampleOrders(context);

            // Update product sold counts and view counts
            await UpdateProductStats(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedAdminUser(ApplicationDbContext context)
        {
            // Check if admin user already exists
            var adminExists = await context.Users.AnyAsync(u => u.Email == "admin@store.com");
            if (adminExists) return;

            var adminUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin@store.com",
                Email = "admin@store.com",
                EmailConfirmed = true,
                FirstName = "Quản trị",
                LastName = "Viên",
                PhoneNumber = "0987654321",
                Address = "123 Đường ABC, Quận XYZ, TP.HCM",
                City = "Hồ Chí Minh",
                PostalCode = "70000",
                Role = UserRole.SuperAdmin,
                IsActive = true,
                CreatedAt = new DateTime(2025, 8, 15),
                LastLoginAt = new DateTime(2025, 9, 21, 10, 30, 0),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            };

            context.Users.Add(adminUser);
            
            // Create demo user
            var userExists = await context.Users.AnyAsync(u => u.Email == "user@store.com");
            if (!userExists)
            {
                var demoUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "user@store.com",
                    Email = "user@store.com",
                    EmailConfirmed = true,
                    FirstName = "Người dùng",
                    LastName = "Demo",
                    PhoneNumber = "0123456789",
                    Address = "456 Đường DEF, Quận ABC, TP.HCM",
                    City = "Hồ Chí Minh",
                    PostalCode = "70000",
                    Role = UserRole.Customer,
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 8, 20),
                    LastLoginAt = new DateTime(2025, 9, 21, 14, 15, 0),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!")
                };
                context.Users.Add(demoUser);
            }
            
            await context.SaveChangesAsync();
        }

        private static async Task SeedSampleOrders(ApplicationDbContext context)
        {
            // Check if sample orders already exist
            var ordersExist = await context.Orders.AnyAsync();
            if (ordersExist) return;

            var customers = await context.Users.Where(u => u.Role == UserRole.Customer).ToListAsync();
            var products = await context.Products.Where(p => p.IsActive).ToListAsync();

            if (!customers.Any() || !products.Any()) return;

            var random = new Random();
            var orders = new List<Order>();

            // Create orders from September 1, 2025 to September 21, 2025
            var startDate = new DateTime(2025, 9, 1);
            var endDate = new DateTime(2025, 9, 21);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // More orders on weekdays, fewer on weekends
                var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                var orderCount = isWeekend ? random.Next(0, 3) : random.Next(1, 8);

                // Consistent order distribution throughout the period
                var recentMultiplier = 1.0;
                orderCount = (int)(orderCount * recentMultiplier);

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
                        OrderStatus = GetRandomOrderStatus(random, orderTime),
                        PaymentStatus = "Paid",
                        ShippingAddress = customer.Address ?? "Địa chỉ giao hàng",
                        ShippingCity = customer.City ?? "TP.HCM",
                        ShippingPostalCode = customer.PostalCode ?? "70000",
                        PaymentMethod = GetRandomPaymentMethod(random),
                        OrderItems = new List<OrderItem>()
                    };

                    // Add 1-5 items to each order
                    var itemCount = random.Next(1, 6);
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

                    // Add shipping fee for orders under 500k
                    if (totalAmount < 500000)
                    {
                        order.ShippingFee = 30000;
                        order.TotalAmount += order.ShippingFee;
                    }

                    orders.Add(order);
                }
            }

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }

        private static async Task UpdateProductStats(ApplicationDbContext context)
        {
            var products = await context.Products.Include(p => p.OrderItems).ToListAsync();
            var random = new Random();

            foreach (var product in products)
            {
                // Calculate sold count from actual orders
                var soldFromOrders = product.OrderItems
                    .Where(oi => oi.Order != null && oi.Order.OrderStatus == "Completed")
                    .Sum(oi => oi.Quantity);

                // Add some additional sold count for realism
                product.SoldCount = soldFromOrders + random.Next(0, 20);

                // Generate realistic view counts (10-50 times the sold count)
                product.ViewCount = product.SoldCount * random.Next(10, 50) + random.Next(100, 1000);

                // Update stock based on sold count
                var originalStock = product.StockQuantity + product.SoldCount;
                product.StockQuantity = Math.Max(0, originalStock - product.SoldCount);

                // Set some products as featured or new
                if (random.NextDouble() < 0.2) // 20% chance
                {
                    product.IsFeatured = true;
                }

                if (product.CreatedAt > new DateTime(2025, 8, 1) && random.NextDouble() < 0.3) // 30% chance for recent products
                {
                    product.IsNewProduct = true;
                }
            }

            await context.SaveChangesAsync();
        }

        private static string GetRandomOrderStatus(Random random, DateTime orderDate)
        {
            var currentDate = new DateTime(2025, 9, 21);
            var daysSinceOrder = (currentDate - orderDate).Days;
            
            // Orders are more likely to be completed as they get older
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

        private static string GetRandomPaymentMethod(Random random)
        {
            var methods = new[] { "COD", "Bank Transfer", "Credit Card", "E-Wallet" };
            return methods[random.Next(methods.Length)];
        }

        public static async Task SeedSampleCustomers(ApplicationDbContext context)
        {
            // Check if sample customers already exist
            var customerCount = await context.Users.CountAsync(u => u.Role == UserRole.Customer);
            if (customerCount >= 50) return;

            var random = new Random();
            var customers = new List<User>();

            var firstNames = new[] { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng", "Bùi", "Đỗ", "Hồ", "Ngô", "Dương" };
            var lastNames = new[] { "Văn Anh", "Thị Bình", "Minh Châu", "Hoàng Dũng", "Thị Hoa", "Văn Khoa", "Thị Lan", "Minh Long", "Thị Mai", "Văn Nam", "Thị Oanh", "Minh Phúc", "Thị Quỳnh", "Văn Sơn", "Thị Tâm" };
            var cities = new[] { "Hồ Chí Minh", "Hà Nội", "Đà Nẵng", "Cần Thơ", "Hải Phòng", "Biên Hòa", "Nha Trang", "Huế", "Vũng Tàu", "Buôn Ma Thuột" };

            for (int i = 0; i < 50; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                var email = $"customer{i + 1}@example.com";
                var city = cities[random.Next(cities.Length)];

                var customer = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = $"09{random.Next(10000000, 99999999)}",
                    Address = $"Số {random.Next(1, 999)} đường {random.Next(1, 50)}, Quận {random.Next(1, 13)}",
                    City = city,
                    PostalCode = random.Next(10000, 99999).ToString(),
                    Role = UserRole.Customer,
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 9, 1).AddDays(-random.Next(1, 60)),
                    LastLoginAt = new DateTime(2025, 9, 21).AddDays(-random.Next(0, 20)),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123@")
                };

                customers.Add(customer);
            }

            context.Users.AddRange(customers);
            await context.SaveChangesAsync();
        }
    }
}
