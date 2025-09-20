using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;

namespace ElectronicsStore.Data
{
    public static class UpdateAllImages
    {
        public static async Task UpdateAllProductImages(ApplicationDbContext context)
        {
            Console.WriteLine("Bắt đầu cập nhật tất cả hình ảnh sản phẩm...");
            
            var products = await context.Products.ToListAsync();
            int updatedCount = 0;
            
            foreach (var product in products)
            {
                // Tạo đường dẫn hình ảnh dựa trên SKU
                var imagePath = $"/images/products/{product.SKU}.jpg";
                
                // Kiểm tra xem file có tồn tại không
                var physicalPath = Path.Combine("wwwroot", "images", "products", $"{product.SKU}.jpg");
                
                if (File.Exists(physicalPath))
                {
                    product.MainImageUrl = imagePath;
                    updatedCount++;
                    Console.WriteLine($"✓ Cập nhật hình ảnh cho {product.Name}: {imagePath}");
                }
                else
                {
                    // Sử dụng hình ảnh placeholder từ Unsplash
                    var placeholderUrl = GetPlaceholderImage(product.CategoryId, product.Name);
                    product.MainImageUrl = placeholderUrl;
                    updatedCount++;
                    Console.WriteLine($"✓ Sử dụng placeholder cho {product.Name}: {placeholderUrl}");
                }
            }
            
            await context.SaveChangesAsync();
            Console.WriteLine($"Hoàn thành! Đã cập nhật {updatedCount}/{products.Count} sản phẩm.");
        }
        
        private static string GetPlaceholderImage(int categoryId, string productName)
        {
            // Trả về hình ảnh placeholder phù hợp với từng danh mục
            return categoryId switch
            {
                1 => "https://images.unsplash.com/photo-1553406830-ef2513450d76?w=400&h=300&fit=crop", // Vi điều khiển
                2 => "https://images.unsplash.com/photo-1581092160562-40aa08e78837?w=400&h=300&fit=crop", // Cảm biến
                3 => "https://images.unsplash.com/photo-1581092921461-eab62e97a780?w=400&h=300&fit=crop", // Linh kiện thụ động
                4 => "https://images.unsplash.com/photo-1581092795442-8d6d0b4e6e0a?w=400&h=300&fit=crop", // Bán dẫn
                5 => "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&h=300&fit=crop", // Module giao tiếp
                6 => "https://images.unsplash.com/photo-1581092921461-eab62e97a780?w=400&h=300&fit=crop", // Nguồn điện
                7 => "https://images.unsplash.com/photo-1581092795442-8d6d0b4e6e0a?w=400&h=300&fit=crop", // Màn hình
                8 => "https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?w=400&h=300&fit=crop", // Động cơ & Servo
                _ => "https://images.unsplash.com/photo-1553406830-ef2513450d76?w=400&h=300&fit=crop"  // Mặc định
            };
        }
    }
}