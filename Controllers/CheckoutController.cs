using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using System.Security.Claims;

namespace ElectronicsStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await GetCartAsync();
            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                Cart = cart,
                ShippingAddress = new Address(),
                PaymentMethod = "COD"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var cart = await GetCartAsync();
            if (cart == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            // Kiểm tra tồn kho
            foreach (var item in cart.CartItems)
            {
                if (item.Product.StockQuantity < item.Quantity)
                {
                    return Json(new { success = false, message = $"Sản phẩm {item.Product.Name} không đủ hàng" });
                }
            }

            // Tạo đơn hàng
            var order = new Order
            {
                UserId = GetCurrentUserId(),
                OrderNumber = GenerateOrderNumber(),
                CreatedAt = DateTime.Now,
                OrderStatus = "Pending",
                PaymentStatus = "Pending",
                SubTotal = cart.TotalAmount,
                ShippingFee = 0,
                TotalAmount = cart.TotalAmount,
                ShippingAddress = $"{model.ShippingAddress.Street}, {model.ShippingAddress.City}",
                ShippingCity = model.ShippingAddress.City,
                PaymentMethod = model.PaymentMethod,
                CustomerName = model.ShippingAddress.FullName,
                CustomerPhone = model.ShippingAddress.Phone,
                CustomerEmail = model.ShippingAddress.Email
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Tạo chi tiết đơn hàng
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    ProductSKU = cartItem.Product.SKU,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    TotalPrice = cartItem.TotalPrice
                };
                _context.OrderItems.Add(orderItem);

                // Giảm tồn kho
                cartItem.Product.StockQuantity -= cartItem.Quantity;
            }

            // Xóa giỏ hàng
            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return Json(new { success = true, orderId = order.Id });
        }

        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private async Task<Cart?> GetCartAsync()
        {
            var userId = GetCurrentUserId();
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        private string GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            }
            
            var sessionId = HttpContext.Session.GetString("CartSessionId");
            return string.IsNullOrEmpty(sessionId) ? "" : $"guest_{sessionId}";
        }
        
        private string GenerateOrderNumber()
        {
            return $"ORD{DateTime.Now:yyyyMMddHHmmss}";
        }
    }

    public class CheckoutViewModel
    {
        public Cart Cart { get; set; } = null!;
        public Address ShippingAddress { get; set; } = null!;
        public string PaymentMethod { get; set; } = "COD";
    }

    public class Address
    {
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
    }
}