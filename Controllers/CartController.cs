using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using System.Security.Claims;

namespace ElectronicsStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await GetOrCreateCartAsync();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || !product.IsActive || product.StockQuantity < quantity)
            {
                return Json(new { success = false, message = "Sản phẩm không khả dụng" });
            }

            var cart = await GetOrCreateCartAsync();
            var existingItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (existingItem.Quantity > product.StockQuantity)
                {
                    return Json(new { success = false, message = "Không đủ hàng trong kho" });
                }
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.DisplayPrice
                });
            }

            cart.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Đã thêm vào giỏ hàng",
                cartCount = cart.TotalItems
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == cartItemId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else if (quantity > cartItem.Product.StockQuantity)
            {
                return Json(new { success = false, message = "Không đủ hàng trong kho" });
            }
            else
            {
                cartItem.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        public async Task<IActionResult> GetCartCount()
        {
            var cart = await GetOrCreateCartAsync();
            return Json(new { count = cart.TotalItems });
        }

        public IActionResult Test()
        {
            return View();
        }

        private async Task<Cart> GetOrCreateCartAsync()
        {
            var userId = GetCurrentUserId();
            
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart 
                { 
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                
                _context.Carts.Add(cart);
                
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    // Nếu lỗi foreign key, trả về cart tạm thời
                    return new Cart 
                    { 
                        UserId = userId, 
                        CartItems = new List<CartItem>(),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                }
            }

            return cart;
        }

        private string GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            }
            
            // Sử dụng session cho guest users
            var sessionId = HttpContext.Session.GetString("CartSessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartSessionId", sessionId);
            }
            return $"guest_{sessionId}";
        }
    }
}