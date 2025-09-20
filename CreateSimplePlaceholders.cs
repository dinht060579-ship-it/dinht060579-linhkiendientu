using System.Net.Http;

namespace ElectronicsStore.Data
{
    public static class CreateSimplePlaceholders
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        public static async Task CreateMissingImages()
        {
            var imagesPath = Path.Combine("wwwroot", "images", "products");
            Directory.CreateDirectory(imagesPath);
            
            // Danh sách các sản phẩm cần hình ảnh
            var productSKUs = new[]
            {
                "ARD-UNO-R3", "RPI4-4GB", "ESP32-DEVKIT", "DHT22", "HC-SR04", "MPU6050",
                "RES-KIT-100", "CAP-CER-KIT", "LED-5MM-KIT", "LM358", "ESP8266-01", 
                "HC-05", "NRF24L01", "LM2596-ADJ", "AMS1117-33", "LCD-16X2-I2C", 
                "OLED-096-I2C", "SG90-SERVO", "28BYJ48-ULN2003"
            };
            
            // URL hình ảnh placeholder từ Unsplash
            var placeholderUrls = new[]
            {
                "https://images.unsplash.com/photo-1553406830-ef2513450d76?w=400&h=300&fit=crop",
                "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400&h=300&fit=crop",
                "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=400&h=300&fit=crop",
                "https://images.unsplash.com/photo-1581092160562-40aa08e78837?w=400&h=300&fit=crop",
                "https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?w=400&h=300&fit=crop",
                "https://images.unsplash.com/photo-1581092795360-fd1ca04f0952?w=400&h=300&fit=crop",
                "https://images.unsplash.com/photo-1581092921461-eab62e97a780?w=400&h=300&fit=crop",
                "https://images.unsplash.com/photo-1581092918484-8313a22c5735?w=400&h=300&fit=crop",
                "https://images.unsplash.com/photo-1581092795442-8d6d0b4e6e0a?w=400&h=300&fit=crop"
            };
            
            Console.WriteLine("Tạo hình ảnh placeholder cho các sản phẩm...");
            
            for (int i = 0; i < productSKUs.Length; i++)
            {
                var fileName = $"{productSKUs[i]}.jpg";
                var filePath = Path.Combine(imagesPath, fileName);
                
                if (!File.Exists(filePath))
                {
                    try
                    {
                        var urlIndex = i % placeholderUrls.Length;
                        var imageBytes = await httpClient.GetByteArrayAsync(placeholderUrls[urlIndex]);
                        await File.WriteAllBytesAsync(filePath, imageBytes);
                        Console.WriteLine($"✓ Đã tạo: {fileName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ Lỗi khi tạo {fileName}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"- Đã tồn tại: {fileName}");
                }
            }
            
            Console.WriteLine("Hoàn thành tạo placeholder!");
        }
    }
}