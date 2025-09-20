using ElectronicsStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.Data
{
    public static class UpdateProductImages
    {
        public static async Task UpdateImages(ApplicationDbContext context)
        {
            var productImages = new Dictionary<string, string>
            {
                // Vi điều khiển - Hình ảnh local
                ["ARD-UNO-R3"] = "/images/products/ARD-UNO-R3.jpg",
                ["RPI4-4GB"] = "/images/products/RPI4-4GB.jpg",
                ["ESP32-DEVKIT"] = "/images/products/ESP32-DEVKIT.jpg",

                // Cảm biến - Hình ảnh local
                ["DHT22"] = "/images/products/DHT22.jpg",
                ["HC-SR04"] = "/images/products/HC-SR04.jpg",
                ["MPU6050"] = "/images/products/MPU6050.jpg",

                // Linh kiện thụ động - Hình ảnh local
                ["RES-KIT-100"] = "/images/products/RES-KIT-100.jpg",
                ["CAP-CER-KIT"] = "/images/products/CAP-CER-KIT.jpg",

                // Bán dẫn - Hình ảnh local
                ["LED-5MM-KIT"] = "/images/products/LED-5MM-KIT.jpg",
                ["LM358"] = "/images/products/LM358.jpg",

                // Module giao tiếp - Hình ảnh local
                ["ESP8266-01"] = "/images/products/ESP8266-01.jpg",
                ["HC-05"] = "/images/products/HC-05.jpg",
                ["NRF24L01"] = "/images/products/NRF24L01.jpg",

                // Nguồn điện - Hình ảnh local
                ["LM2596-ADJ"] = "/images/products/LM2596-ADJ.jpg",
                ["AMS1117-33"] = "/images/products/AMS1117-33.jpg",

                // Màn hình - Hình ảnh local
                ["LCD-16X2-I2C"] = "/images/products/LCD-16X2-I2C.jpg",
                ["OLED-096-I2C"] = "/images/products/OLED-096-I2C.jpg",

                // Động cơ & Servo - Hình ảnh local
                ["SG90-SERVO"] = "/images/products/SG90-SERVO.jpg",
                ["28BYJ48-ULN2003"] = "/images/products/28BYJ48-ULN2003.jpg"
            };

            foreach (var imageMapping in productImages)
            {
                var product = await context.Products.FirstOrDefaultAsync(p => p.SKU == imageMapping.Key);
                if (product != null)
                {
                    product.MainImageUrl = imageMapping.Value;
                    Console.WriteLine($"Đã cập nhật hình ảnh cho {product.Name}: {imageMapping.Value}");
                }
                else
                {
                    Console.WriteLine($"Không tìm thấy sản phẩm với SKU: {imageMapping.Key}");
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
