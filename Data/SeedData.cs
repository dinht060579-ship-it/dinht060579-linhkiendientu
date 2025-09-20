using ElectronicsStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (context.Products.Any())
            {
                return; // Database has been seeded
            }

            // Seed Brands
            var brands = new List<Brand>
            {
                new Brand { Name = "Arduino", Description = "Nền tảng phần cứng mã nguồn mở", Website = "https://arduino.cc" },
                new Brand { Name = "Raspberry Pi", Description = "Máy tính nhỏ gọn", Website = "https://raspberrypi.org" },
                new Brand { Name = "STMicroelectronics", Description = "Nhà sản xuất vi điều khiển và cảm biến", Website = "https://st.com" },
                new Brand { Name = "Texas Instruments", Description = "Nhà sản xuất IC analog và digital", Website = "https://ti.com" },
                new Brand { Name = "Atmel", Description = "Vi điều khiển AVR và ARM", Website = "https://microchip.com" },
                new Brand { Name = "Espressif", Description = "Vi điều khiển WiFi và Bluetooth", Website = "https://espressif.com" },
                new Brand { Name = "Adafruit", Description = "Module và kit phát triển", Website = "https://adafruit.com" },
                new Brand { Name = "SparkFun", Description = "Linh kiện điện tử và module", Website = "https://sparkfun.com" },
                new Brand { Name = "Vishay", Description = "Điện trở, tụ điện, diode", Website = "https://vishay.com" },
                new Brand { Name = "Infineon", Description = "Bán dẫn công suất và cảm biến", Website = "https://infineon.com" }
            };

            context.Brands.AddRange(brands);
            await context.SaveChangesAsync();

            // Seed Categories
            var categories = new List<Category>
            {
                new Category { Name = "Vi điều khiển", Description = "Các loại vi điều khiển và board phát triển" },
                new Category { Name = "Cảm biến", Description = "Cảm biến nhiệt độ, độ ẩm, ánh sáng, chuyển động" },
                new Category { Name = "Linh kiện thụ động", Description = "Điện trở, tụ điện, cuộn cảm" },
                new Category { Name = "Bán dẫn", Description = "IC, transistor, diode, LED" },
                new Category { Name = "Module giao tiếp", Description = "WiFi, Bluetooth, UART, SPI, I2C" },
                new Category { Name = "Nguồn điện", Description = "Adapter, pin, module nguồn" },
                new Category { Name = "Màn hình", Description = "LCD, OLED, LED matrix" },
                new Category { Name = "Động cơ & Servo", Description = "Servo motor, stepper motor, DC motor" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // Get saved entities with IDs
            var arduinoBrand = brands.First(b => b.Name == "Arduino");
            var raspberryBrand = brands.First(b => b.Name == "Raspberry Pi");
            var stBrand = brands.First(b => b.Name == "STMicroelectronics");
            var tiBrand = brands.First(b => b.Name == "Texas Instruments");
            var atmelBrand = brands.First(b => b.Name == "Atmel");
            var espressifBrand = brands.First(b => b.Name == "Espressif");
            var adafruitBrand = brands.First(b => b.Name == "Adafruit");
            var sparkfunBrand = brands.First(b => b.Name == "SparkFun");
            var vishayBrand = brands.First(b => b.Name == "Vishay");
            var infineonBrand = brands.First(b => b.Name == "Infineon");

            var microcontrollerCat = categories.First(c => c.Name == "Vi điều khiển");
            var sensorCat = categories.First(c => c.Name == "Cảm biến");
            var passiveCat = categories.First(c => c.Name == "Linh kiện thụ động");
            var semiconductorCat = categories.First(c => c.Name == "Bán dẫn");
            var communicationCat = categories.First(c => c.Name == "Module giao tiếp");
            var powerCat = categories.First(c => c.Name == "Nguồn điện");
            var displayCat = categories.First(c => c.Name == "Màn hình");
            var motorCat = categories.First(c => c.Name == "Động cơ & Servo");

            // Seed Products with real electronic components
            var products = new List<Product>();
            var random = new Random();
            var productCreatedDate = new DateTime(2025, 8, 15); // Sản phẩm được tạo trước khi có đơn hàng
            
            // Vi điều khiển
            products.AddRange(new[]
            {
                new Product
                {
                    Name = "Arduino Uno R3",
                    SKU = "ARD-UNO-R3",
                    PartNumber = "A000066",
                    ShortDescription = "Board vi điều khiển phổ biến nhất cho người mới bắt đầu",
                    DetailedDescription = "Arduino Uno R3 là board vi điều khiển dựa trên ATmega328P. Có 14 chân digital I/O, 6 chân analog input, crystal 16MHz, kết nối USB, jack nguồn, ICSP header và nút reset.",
                    Price = 350000,
                    DiscountPrice = 320000,
                    StockQuantity = 50,
                    CategoryId = microcontrollerCat.Id,
                    BrandId = arduinoBrand.Id,
                    IsNewProduct = true,
                    IsFeatured = true,
                    MainImageUrl = "/images/products/ARD-UNO-R3.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Vi điều khiển"": ""ATmega328P"",
                        ""Điện áp hoạt động"": ""5V"",
                        ""Điện áp đầu vào"": ""7-12V"",
                        ""Chân Digital I/O"": ""14 (6 có PWM)"",
                        ""Chân Analog Input"": ""6"",
                        ""Bộ nhớ Flash"": ""32KB"",
                        ""SRAM"": ""2KB"",
                        ""EEPROM"": ""1KB"",
                        ""Tốc độ xung nhịp"": ""16MHz""
                    }"
                },
                new Product
                {
                    Name = "Raspberry Pi 4 Model B 4GB",
                    SKU = "RPI4-4GB",
                    PartNumber = "RPI4-MODBP-4GB",
                    ShortDescription = "Máy tính nhỏ gọn với hiệu năng cao",
                    DetailedDescription = "Raspberry Pi 4 Model B với 4GB RAM, CPU ARM Cortex-A72 quad-core 64-bit, dual-band WiFi, Bluetooth 5.0, Gigabit Ethernet, 2 cổng USB 3.0 và 2 cổng USB 2.0.",
                    Price = 1850000,
                    StockQuantity = 25,
                    CategoryId = microcontrollerCat.Id,
                    BrandId = raspberryBrand.Id,
                    IsFeatured = true,
                    MainImageUrl = "/images/products/RPI4-4GB.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""CPU"": ""ARM Cortex-A72 quad-core 1.5GHz"",
                        ""RAM"": ""4GB LPDDR4"",
                        ""WiFi"": ""802.11ac dual-band"",
                        ""Bluetooth"": ""5.0"",
                        ""Ethernet"": ""Gigabit"",
                        ""USB"": ""2x USB 3.0, 2x USB 2.0"",
                        ""HDMI"": ""2x micro HDMI (4K 60fps)"",
                        ""GPIO"": ""40 pin""
                    }"
                },
                new Product
                {
                    Name = "ESP32 DevKit V1",
                    SKU = "ESP32-DEVKIT",
                    PartNumber = "ESP32-WROOM-32",
                    ShortDescription = "Vi điều khiển tích hợp WiFi và Bluetooth",
                    DetailedDescription = "ESP32 là vi điều khiển dual-core với WiFi và Bluetooth tích hợp. Phù hợp cho các dự án IoT với hiệu năng cao và tiêu thụ điện năng thấp.",
                    Price = 180000,
                    DiscountPrice = 165000,
                    StockQuantity = 80,
                    CategoryId = microcontrollerCat.Id,
                    BrandId = espressifBrand.Id,
                    IsNewProduct = true,
                    MainImageUrl = "/images/products/ESP32-DEVKIT.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""CPU"": ""Dual-core Tensilica LX6"",
                        ""Tốc độ"": ""240MHz"",
                        ""WiFi"": ""802.11 b/g/n"",
                        ""Bluetooth"": ""4.2 BR/EDR và BLE"",
                        ""Flash"": ""4MB"",
                        ""SRAM"": ""520KB"",
                        ""GPIO"": ""30 chân"",
                        ""ADC"": ""12-bit"",
                        ""PWM"": ""16 kênh""
                    }"
                }
            });

            // Cảm biến
            products.AddRange(new[]
            {
                new Product
                {
                    Name = "DHT22 - Cảm biến nhiệt độ và độ ẩm",
                    SKU = "DHT22",
                    PartNumber = "AM2302",
                    ShortDescription = "Cảm biến nhiệt độ và độ ẩm chính xác cao",
                    DetailedDescription = "DHT22 là cảm biến nhiệt độ và độ ẩm với độ chính xác cao, giao tiếp 1-wire đơn giản. Phù hợp cho các dự án giám sát môi trường.",
                    Price = 85000,
                    StockQuantity = 100,
                    CategoryId = sensorCat.Id,
                    BrandId = adafruitBrand.Id,
                    MainImageUrl = "/images/products/DHT22.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Dải nhiệt độ"": ""-40°C đến +80°C"",
                        ""Độ chính xác nhiệt độ"": ""±0.5°C"",
                        ""Dải độ ẩm"": ""0-100% RH"",
                        ""Độ chính xác độ ẩm"": ""±2% RH"",
                        ""Điện áp"": ""3.3-6V"",
                        ""Giao tiếp"": ""1-wire""
                    }"
                },
                new Product
                {
                    Name = "HC-SR04 - Cảm biến siêu âm",
                    SKU = "HC-SR04",
                    PartNumber = "HC-SR04",
                    ShortDescription = "Cảm biến đo khoảng cách bằng siêu âm",
                    DetailedDescription = "HC-SR04 là cảm biến siêu âm đo khoảng cách từ 2cm đến 4m với độ chính xác cao. Dễ sử dụng với Arduino và các vi điều khiển khác.",
                    Price = 45000,
                    StockQuantity = 150,
                    CategoryId = sensorCat.Id,
                    BrandId = sparkfunBrand.Id,
                    MainImageUrl = "/images/products/HC-SR04.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Dải đo"": ""2cm - 400cm"",
                        ""Độ chính xác"": ""±3mm"",
                        ""Góc đo"": ""15°"",
                        ""Điện áp"": ""5V"",
                        ""Tần số"": ""40kHz"",
                        ""Trigger Input"": ""10µS TTL pulse""
                    }"
                },
                new Product
                {
                    Name = "MPU6050 - Cảm biến gia tốc và con quay hồi chuyển",
                    SKU = "MPU6050",
                    PartNumber = "MPU-6050",
                    ShortDescription = "6-axis motion tracking device",
                    DetailedDescription = "MPU6050 kết hợp 3-axis gyroscope và 3-axis accelerometer với Digital Motion Processor (DMP). Giao tiếp I2C, phù hợp cho các dự án cân bằng và điều hướng.",
                    Price = 65000,
                    StockQuantity = 75,
                    CategoryId = sensorCat.Id,
                    BrandId = tiBrand.Id,
                    MainImageUrl = "/images/products/MPU6050.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Gyroscope"": ""±250, ±500, ±1000, ±2000°/sec"",
                        ""Accelerometer"": ""±2g, ±4g, ±8g, ±16g"",
                        ""ADC"": ""16-bit"",
                        ""Điện áp"": ""2.375V-3.46V"",
                        ""Giao tiếp"": ""I2C"",
                        ""Nhiệt độ hoạt động"": ""-40°C to +85°C""
                    }"
                }
            });

            // Linh kiện thụ động
            products.AddRange(new[]
            {
                new Product
                {
                    Name = "Điện trở 1/4W - Bộ 100 giá trị",
                    SKU = "RES-KIT-100",
                    PartNumber = "CFR-25JB-52",
                    ShortDescription = "Bộ kit 100 giá trị điện trở 1/4W phổ biến",
                    DetailedDescription = "Bộ kit gồm 100 giá trị điện trở carbon film 1/4W từ 1Ω đến 1MΩ, mỗi giá trị 10 chiếc. Sai số ±5%, phù hợp cho thực hành và dự án điện tử.",
                    Price = 120000,
                    DiscountPrice = 100000,
                    StockQuantity = 30,
                    CategoryId = passiveCat.Id,
                    BrandId = vishayBrand.Id,
                    MainImageUrl = "/images/products/RES-KIT-100.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Công suất"": ""1/4W (0.25W)"",
                        ""Sai số"": ""±5%"",
                        ""Loại"": ""Carbon Film"",
                        ""Dải giá trị"": ""1Ω - 1MΩ"",
                        ""Nhiệt độ"": ""-55°C to +155°C"",
                        ""Số lượng"": ""1000 chiếc (100 giá trị x 10)""
                    }"
                },
                new Product
                {
                    Name = "Tụ điện Ceramic 50V - Bộ kit",
                    SKU = "CAP-CER-KIT",
                    PartNumber = "CC0805",
                    ShortDescription = "Bộ kit tụ điện ceramic đa dung lượng",
                    DetailedDescription = "Bộ kit gồm các tụ điện ceramic 50V với dung lượng từ 1pF đến 1µF. Kích thước nhỏ gọn, phù hợp cho mạch tần số cao và lọc nhiễu.",
                    Price = 95000,
                    StockQuantity = 40,
                    CategoryId = passiveCat.Id,
                    BrandId = vishayBrand.Id,
                    MainImageUrl = "/images/products/CAP-CER-KIT.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Điện áp"": ""50V"",
                        ""Dải dung lượng"": ""1pF - 1µF"",
                        ""Loại"": ""Ceramic (X7R, C0G)"",
                        ""Sai số"": ""±10%, ±20%"",
                        ""Nhiệt độ"": ""-55°C to +125°C"",
                        ""Package"": ""0805, Through-hole""
                    }"
                }
            });

            // Bán dẫn
            products.AddRange(new[]
            {
                new Product
                {
                    Name = "LED 5mm - Bộ kit đa màu",
                    SKU = "LED-5MM-KIT",
                    PartNumber = "LED-5MM-MIX",
                    ShortDescription = "Bộ kit LED 5mm đa màu sáng",
                    DetailedDescription = "Bộ kit gồm 100 LED 5mm các màu: đỏ, xanh lá, xanh dương, vàng, trắng. Độ sáng cao, tuổi thọ lâu dài, phù hợp cho các dự án hiển thị và trang trí.",
                    Price = 75000,
                    StockQuantity = 60,
                    CategoryId = semiconductorCat.Id,
                    BrandId = vishayBrand.Id,
                    MainImageUrl = "/images/products/LED-5MM-KIT.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Kích thước"": ""5mm"",
                        ""Điện áp thuận"": ""1.8V-3.3V"",
                        ""Dòng điện"": ""20mA"",
                        ""Góc chiếu"": ""30°-60°"",
                        ""Tuổi thọ"": ""50,000 giờ"",
                        ""Màu sắc"": ""Đỏ, Xanh lá, Xanh dương, Vàng, Trắng""
                    }"
                },
                new Product
                {
                    Name = "LM358 - Op-Amp kép",
                    SKU = "LM358",
                    PartNumber = "LM358N",
                    ShortDescription = "IC khuếch đại thuật toán kép",
                    DetailedDescription = "LM358 là IC chứa 2 op-amp với nguồn đơn, tiêu thụ điện năng thấp. Phù hợp cho các mạch khuếch đại, so sánh và lọc tín hiệu.",
                    Price = 12000,
                    StockQuantity = 200,
                    CategoryId = semiconductorCat.Id,
                    BrandId = tiBrand.Id,
                    MainImageUrl = "/images/products/LM358.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Số kênh"": ""2"",
                        ""Điện áp nguồn"": ""3V-32V"",
                        ""Gain-Bandwidth"": ""1MHz"",
                        ""Slew Rate"": ""0.3V/µs"",
                        ""Input Offset Voltage"": ""±2mV"",
                        ""Package"": ""DIP-8""
                    }"
                }
            });

            // Module giao tiếp
            products.AddRange(new[]
            {
                new Product
                {
                    Name = "ESP8266 WiFi Module",
                    SKU = "ESP8266-01",
                    PartNumber = "ESP8266-01",
                    ShortDescription = "Module WiFi giá rẻ cho IoT",
                    DetailedDescription = "ESP8266-01 là module WiFi nhỏ gọn với vi điều khiển tích hợp. Hỗ trợ 802.11 b/g/n, có thể lập trình độc lập hoặc dùng như WiFi shield.",
                    Price = 55000,
                    StockQuantity = 120,
                    CategoryId = communicationCat.Id,
                    BrandId = espressifBrand.Id,
                    MainImageUrl = "/images/products/ESP8266-01.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""CPU"": ""32-bit RISC"",
                        ""Tốc độ"": ""80MHz"",
                        ""WiFi"": ""802.11 b/g/n"",
                        ""Flash"": ""1MB"",
                        ""GPIO"": ""2 chân"",
                        ""Điện áp"": ""3.3V"",
                        ""Dòng tiêu thụ"": ""70mA""
                    }"
                },
                new Product
                {
                    Name = "HC-05 Bluetooth Module",
                    SKU = "HC-05",
                    PartNumber = "HC-05",
                    ShortDescription = "Module Bluetooth cho giao tiếp không dây",
                    DetailedDescription = "HC-05 là module Bluetooth 2.0 với giao tiếp UART. Dễ sử dụng, có thể cấu hình làm master hoặc slave. Tầm hoạt động 10m.",
                    Price = 85000,
                    StockQuantity = 90,
                    CategoryId = communicationCat.Id,
                    BrandId = sparkfunBrand.Id,
                    MainImageUrl = "/images/products/HC-05.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Bluetooth"": ""2.0+EDR"",
                        ""Tầm hoạt động"": ""10m"",
                        ""Tốc độ"": ""2.1Mbps/160kbps"",
                        ""Điện áp"": ""3.3V-6V"",
                        ""Dòng tiêu thụ"": ""40mA"",
                        ""Giao tiếp"": ""UART""
                    }"
                },
                new Product
                {
                    Name = "NRF24L01+ Wireless Module",
                    SKU = "NRF24L01",
                    PartNumber = "NRF24L01+",
                    ShortDescription = "Module truyền thông không dây 2.4GHz",
                    DetailedDescription = "NRF24L01+ là module RF 2.4GHz với tốc độ cao và tiêu thụ điện năng thấp. Hỗ trợ nhiều kênh, auto-acknowledgment và auto-retransmit.",
                    Price = 35000,
                    StockQuantity = 80,
                    CategoryId = communicationCat.Id,
                    BrandId = adafruitBrand.Id,
                    MainImageUrl = "/images/products/NRF24L01.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Tần số"": ""2.4GHz ISM"",
                        ""Tốc độ"": ""250kbps, 1Mbps, 2Mbps"",
                        ""Tầm hoạt động"": ""100m (outdoor)"",
                        ""Điện áp"": ""1.9V-3.6V"",
                        ""Dòng tiêu thụ"": ""13.5mA (TX), 14mA (RX)"",
                        ""Kênh"": ""125 kênh""
                    }"
                }
            });

            // Nguồn điện
            products.AddRange(new[]
            {
                new Product
                {
                    Name = "LM2596 DC-DC Buck Converter",
                    SKU = "LM2596-ADJ",
                    PartNumber = "LM2596S-ADJ",
                    ShortDescription = "Module nguồn DC-DC điều chỉnh được",
                    DetailedDescription = "LM2596 là module nguồn switching giảm áp với hiệu suất cao 92%. Điện áp đầu ra điều chỉnh được từ 1.25V-35V, dòng tối đa 3A.",
                    Price = 45000,
                    StockQuantity = 70,
                    CategoryId = powerCat.Id,
                    BrandId = tiBrand.Id,
                    MainImageUrl = "/images/products/LM2596-ADJ.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Điện áp vào"": ""4V-40V"",
                        ""Điện áp ra"": ""1.25V-35V"",
                        ""Dòng tối đa"": ""3A"",
                        ""Hiệu suất"": ""92%"",
                        ""Tần số switching"": ""150kHz"",
                        ""Bảo vệ"": ""Quá dòng, quá nhiệt""
                    }"
                },
                new Product
                {
                    Name = "AMS1117-3.3V Voltage Regulator",
                    SKU = "AMS1117-33",
                    PartNumber = "AMS1117-3.3",
                    ShortDescription = "IC ổn áp 3.3V tuyến tính",
                    DetailedDescription = "AMS1117-3.3V là IC ổn áp tuyến tính với điện áp ra cố định 3.3V. Dòng tối đa 1A, có bảo vệ quá dòng và quá nhiệt.",
                    Price = 8000,
                    StockQuantity = 150,
                    CategoryId = powerCat.Id,
                    BrandId = tiBrand.Id,
                    MainImageUrl = "/images/products/AMS1117-33.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Điện áp vào"": ""4.75V-15V"",
                        ""Điện áp ra"": ""3.3V ±2%"",
                        ""Dòng tối đa"": ""1A"",
                        ""Dropout Voltage"": ""1.3V"",
                        ""Package"": ""SOT-223, TO-252"",
                        ""Nhiệt độ"": ""-40°C to +125°C""
                    }"
                }
            });

            // Màn hình
            products.AddRange(new[]
            {
                new Product
                {
                    Name = "LCD 16x2 với I2C",
                    SKU = "LCD-16X2-I2C",
                    PartNumber = "HD44780-I2C",
                    ShortDescription = "Màn hình LCD 16x2 với module I2C",
                    DetailedDescription = "Màn hình LCD 16x2 ký tự với module I2C tích hợp, giảm số chân kết nối từ 6 xuống 2 (SDA, SCL). Dễ sử dụng với Arduino và các vi điều khiển khác.",
                    Price = 95000,
                    StockQuantity = 45,
                    CategoryId = displayCat.Id,
                    BrandId = adafruitBrand.Id,
                    MainImageUrl = "/images/products/LCD-16X2-I2C.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Kích thước"": ""16 ký tự x 2 dòng"",
                        ""Điện áp"": ""5V"",
                        ""Giao tiếp"": ""I2C"",
                        ""Địa chỉ I2C"": ""0x27 (mặc định)"",
                        ""Backlight"": ""LED xanh"",
                        ""Kích thước"": ""80x36mm""
                    }"
                },
                new Product
                {
                    Name = "OLED 0.96 inch 128x64 I2C",
                    SKU = "OLED-096-I2C",
                    PartNumber = "SSD1306",
                    ShortDescription = "Màn hình OLED nhỏ gọn độ phân giải cao",
                    DetailedDescription = "Màn hình OLED 0.96 inch với độ phân giải 128x64 pixel, giao tiếp I2C. Độ tương phản cao, góc nhìn rộng, không cần backlight.",
                    Price = 125000,
                    StockQuantity = 60,
                    CategoryId = displayCat.Id,
                    BrandId = adafruitBrand.Id,
                    MainImageUrl = "/images/products/OLED-096-I2C.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Kích thước"": ""0.96 inch"",
                        ""Độ phân giải"": ""128x64 pixel"",
                        ""Driver IC"": ""SSD1306"",
                        ""Giao tiếp"": ""I2C"",
                        ""Điện áp"": ""3.3V-5V"",
                        ""Màu sắc"": ""Xanh dương/Trắng""
                    }"
                }
            });

            // Động cơ & Servo
            products.AddRange(new[]
            {
                new Product
                {
                    Name = "SG90 Micro Servo Motor",
                    SKU = "SG90-SERVO",
                    PartNumber = "SG90",
                    ShortDescription = "Servo motor nhỏ gọn cho robot và mô hình",
                    DetailedDescription = "SG90 là servo motor nhỏ gọn với góc quay 180°, moment xoắn 1.8kg.cm. Phù hợp cho robot, mô hình máy bay và các dự án automation nhỏ.",
                    Price = 65000,
                    StockQuantity = 100,
                    CategoryId = motorCat.Id,
                    BrandId = sparkfunBrand.Id,
                    MainImageUrl = "/images/products/SG90-SERVO.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Góc quay"": ""180° (±90°)"",
                        ""Moment xoắn"": ""1.8kg.cm (4.8V)"",
                        ""Tốc độ"": ""0.1s/60° (4.8V)"",
                        ""Điện áp"": ""4.8V-6V"",
                        ""Dòng tiêu thụ"": ""100-200mA"",
                        ""Kích thước"": ""22.2x11.8x31mm""
                    }"
                },
                new Product
                {
                    Name = "28BYJ-48 Stepper Motor với Driver",
                    SKU = "28BYJ48-ULN2003",
                    PartNumber = "28BYJ-48",
                    ShortDescription = "Động cơ bước với driver ULN2003",
                    DetailedDescription = "28BYJ-48 là động cơ bước 5V với driver ULN2003. Góc bước 5.625°, giảm tốc 1:64. Phù hợp cho các dự án cần điều khiển vị trí chính xác.",
                    Price = 85000,
                    StockQuantity = 55,
                    CategoryId = motorCat.Id,
                    BrandId = adafruitBrand.Id,
                    MainImageUrl = "/images/products/28BYJ48-ULN2003.jpg",
                    CreatedAt = productCreatedDate.AddDays(random.Next(0, 10)),
                    TechnicalSpecs = @"{
                        ""Loại"": ""Unipolar stepper motor"",
                        ""Góc bước"": ""5.625°"",
                        ""Tỷ lệ giảm tốc"": ""1:64"",
                        ""Điện áp"": ""5V"",
                        ""Dòng tiêu thụ"": ""200mA"",
                        ""Moment xoắn"": ""34.3mN.m""
                    }"
                }
            });

            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Thêm ProductAttributes cho một số sản phẩm
            await AddProductAttributes(context);
        }

        private static async Task AddProductAttributes(ApplicationDbContext context)
        {
            var arduinoUno = await context.Products.FirstOrDefaultAsync(p => p.SKU == "ARD-UNO-R3");
            if (arduinoUno != null)
            {
                var attributes = new List<ProductAttribute>
                {
                    new ProductAttribute { ProductId = arduinoUno.Id, AttributeName = "Vi điều khiển", AttributeValue = "ATmega328P", DisplayOrder = 1 },
                    new ProductAttribute { ProductId = arduinoUno.Id, AttributeName = "Điện áp hoạt động", AttributeValue = "5", Unit = "V", DisplayOrder = 2 },
                    new ProductAttribute { ProductId = arduinoUno.Id, AttributeName = "Chân Digital I/O", AttributeValue = "14", DisplayOrder = 3 },
                    new ProductAttribute { ProductId = arduinoUno.Id, AttributeName = "Chân Analog Input", AttributeValue = "6", DisplayOrder = 4 },
                    new ProductAttribute { ProductId = arduinoUno.Id, AttributeName = "Bộ nhớ Flash", AttributeValue = "32", Unit = "KB", DisplayOrder = 5 },
                    new ProductAttribute { ProductId = arduinoUno.Id, AttributeName = "Tốc độ xung nhịp", AttributeValue = "16", Unit = "MHz", DisplayOrder = 6 }
                };
                context.ProductAttributes.AddRange(attributes);
            }

            var esp32 = await context.Products.FirstOrDefaultAsync(p => p.SKU == "ESP32-DEVKIT");
            if (esp32 != null)
            {
                var attributes = new List<ProductAttribute>
                {
                    new ProductAttribute { ProductId = esp32.Id, AttributeName = "CPU", AttributeValue = "Dual-core Tensilica LX6", DisplayOrder = 1 },
                    new ProductAttribute { ProductId = esp32.Id, AttributeName = "Tốc độ", AttributeValue = "240", Unit = "MHz", DisplayOrder = 2 },
                    new ProductAttribute { ProductId = esp32.Id, AttributeName = "WiFi", AttributeValue = "802.11 b/g/n", DisplayOrder = 3 },
                    new ProductAttribute { ProductId = esp32.Id, AttributeName = "Bluetooth", AttributeValue = "4.2 BR/EDR và BLE", DisplayOrder = 4 },
                    new ProductAttribute { ProductId = esp32.Id, AttributeName = "Flash", AttributeValue = "4", Unit = "MB", DisplayOrder = 5 },
                    new ProductAttribute { ProductId = esp32.Id, AttributeName = "GPIO", AttributeValue = "30", Unit = "chân", DisplayOrder = 6 }
                };
                context.ProductAttributes.AddRange(attributes);
            }

            await context.SaveChangesAsync();
        }
    }
}
