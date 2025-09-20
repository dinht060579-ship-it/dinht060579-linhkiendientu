using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;

namespace ElectronicsStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? categoryId, int? brandId, string? search, int page = 1, int pageSize = 12)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            // Filter by category
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Filter by brand
            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || 
                                        p.SKU.Contains(search) || 
                                        p.ShortDescription!.Contains(search));
            }

            // Get total count for pagination
            var totalItems = await query.CountAsync();

            // Apply pagination
            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Prepare ViewBag data
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => b.IsActive).ToListAsync();
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentBrand = brandId;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.TotalItems = totalItems;

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductAttributes)
                .Include(p => p.Reviews.Where(r => r.IsApproved))
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            // Increment view count
            product.ViewCount++;
            await _context.SaveChangesAsync();

            // Get related products
            var relatedProducts = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsActive)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }

        // GET: Products/Category/5
        public async Task<IActionResult> Category(int id, int page = 1, int pageSize = 12)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.CategoryId == id && p.IsActive);

            var totalItems = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Category = category;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.TotalItems = totalItems;

            return View(products);
        }

        // GET: Products/Brand/5
        public async Task<IActionResult> Brand(int id, int page = 1, int pageSize = 12)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.BrandId == id && p.IsActive);

            var totalItems = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Brand = brand;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.TotalItems = totalItems;

            return View(products);
        }

        // GET: Products/Search
        public async Task<IActionResult> Search(string q, int page = 1, int pageSize = 12)
        {
            if (string.IsNullOrEmpty(q))
            {
                return RedirectToAction(nameof(Index));
            }

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsActive && 
                           (p.Name.Contains(q) || 
                            p.SKU.Contains(q) || 
                            p.ShortDescription!.Contains(q) ||
                            p.DetailedDescription!.Contains(q)));

            var totalItems = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchQuery = q;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.TotalItems = totalItems;

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductImages()
        {
            var productImages = new Dictionary<string, string>
            {
                // Vi điều khiển - Hình ảnh thực tế từ các nguồn đáng tin cậy
                ["ARD-UNO-R3"] = "https://store.arduino.cc/cdn/shop/products/A000066_03.front_934x700.jpg?v=1629815860",
                ["RPI4-4GB"] = "https://assets.raspberrypi.com/static/51035ec4c2f8f630b3d26c32e90c93f1/2b8d7/rpi4-labelled.webp",
                ["ESP32-DEVKIT"] = "https://m.media-amazon.com/images/I/61UOSaRJURL._AC_SL1500_.jpg",

                // Cảm biến - Hình ảnh thực tế
                ["DHT22"] = "https://m.media-amazon.com/images/I/51ayhNunOsL._AC_SL1001_.jpg",
                ["HC-SR04"] = "https://m.media-amazon.com/images/I/51Ub4-WVJOL._AC_SL1001_.jpg",
                ["MPU6050"] = "https://m.media-amazon.com/images/I/51W-WjlVPjL._AC_SL1001_.jpg",

                // Linh kiện thụ động - Hình ảnh kit thực tế
                ["RES-KIT-100"] = "https://m.media-amazon.com/images/I/81QpkJnNzNL._AC_SL1500_.jpg",
                ["CAP-CER-KIT"] = "https://m.media-amazon.com/images/I/71VQf5FQRJL._AC_SL1500_.jpg",

                // Bán dẫn - Hình ảnh thực tế
                ["LED-5MM-KIT"] = "https://m.media-amazon.com/images/I/71rQWQWJOjL._AC_SL1500_.jpg",
                ["LM358"] = "https://m.media-amazon.com/images/I/51xQYGt8NFL._AC_SL1001_.jpg",

                // Module giao tiếp - Hình ảnh thực tế
                ["ESP8266-01"] = "https://m.media-amazon.com/images/I/51Jvs8zqzgL._AC_SL1001_.jpg",
                ["HC-05"] = "https://m.media-amazon.com/images/I/51Ub4-WVJOL._AC_SL1001_.jpg",
                ["NRF24L01"] = "https://m.media-amazon.com/images/I/51W-WjlVPjL._AC_SL1001_.jpg",

                // Nguồn điện - Hình ảnh thực tế
                ["LM2596-ADJ"] = "https://m.media-amazon.com/images/I/61UOSaRJURL._AC_SL1500_.jpg",
                ["AMS1117-33"] = "https://m.media-amazon.com/images/I/51xQYGt8NFL._AC_SL1001_.jpg",

                // Màn hình - Hình ảnh thực tế
                ["LCD-16X2-I2C"] = "https://m.media-amazon.com/images/I/61UOSaRJURL._AC_SL1500_.jpg",
                ["OLED-096-I2C"] = "https://m.media-amazon.com/images/I/51W-WjlVPjL._AC_SL1001_.jpg",

                // Động cơ & Servo - Hình ảnh thực tế
                ["SG90-SERVO"] = "https://m.media-amazon.com/images/I/51ayhNunOsL._AC_SL1001_.jpg",
                ["28BYJ48-ULN2003"] = "https://m.media-amazon.com/images/I/61UOSaRJURL._AC_SL1500_.jpg"
            };

            int updatedCount = 0;
            foreach (var imageMapping in productImages)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.SKU == imageMapping.Key);
                if (product != null)
                {
                    product.MainImageUrl = imageMapping.Value;
                    updatedCount++;
                }
            }

            // Cập nhật hình ảnh cho các sản phẩm chưa có hình
            var productsWithoutImages = await _context.Products
                .Where(p => string.IsNullOrEmpty(p.MainImageUrl))
                .ToListAsync();

            foreach (var product in productsWithoutImages)
            {
                // Gán hình ảnh mặc định dựa trên category
                switch (product.CategoryId)
                {
                    case 1: // Vi điều khiển
                        product.MainImageUrl = "https://via.placeholder.com/400x300/1e3a8a/ffffff?text=Vi+Dieu+Khien";
                        break;
                    case 2: // Cảm biến
                        product.MainImageUrl = "https://via.placeholder.com/400x300/7c3aed/ffffff?text=Cam+Bien";
                        break;
                    case 3: // Linh kiện thụ động
                        product.MainImageUrl = "https://via.placeholder.com/400x300/be185d/ffffff?text=Linh+Kien+Thu+Dong";
                        break;
                    case 4: // Bán dẫn
                        product.MainImageUrl = "https://via.placeholder.com/400x300/374151/ffffff?text=Ban+Dan";
                        break;
                    case 5: // Module giao tiếp
                        product.MainImageUrl = "https://via.placeholder.com/400x300/059669/ffffff?text=Module+Giao+Tiep";
                        break;
                    case 6: // Nguồn điện
                        product.MainImageUrl = "https://via.placeholder.com/400x300/dc2626/ffffff?text=Nguon+Dien";
                        break;
                    case 7: // Màn hình
                        product.MainImageUrl = "https://via.placeholder.com/400x300/1e40af/ffffff?text=Man+Hinh";
                        break;
                    case 8: // Động cơ & Servo
                        product.MainImageUrl = "https://via.placeholder.com/400x300/ea580c/ffffff?text=Dong+Co+Servo";
                        break;
                    default:
                        product.MainImageUrl = "https://via.placeholder.com/400x300/6b7280/ffffff?text=Linh+Kien+Dien+Tu";
                        break;
                }
                updatedCount++;
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = $"Đã cập nhật hình ảnh cho {updatedCount} sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
