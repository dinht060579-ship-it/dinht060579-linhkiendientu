using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;

namespace ElectronicsStore
{
    /// <summary>
    /// Chương trình độc lập để cập nhật các ngày trong hệ thống
    /// Chạy lệnh: dotnet run --project ElectronicsStore RunUpdateDates
    /// </summary>
    public class RunUpdateDates
    {
        public static async Task Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "RunUpdateDates")
            {
                Console.WriteLine("=== CẬP NHẬT NGÀY THÁNG TRONG HỆ THỐNG ===");
                Console.WriteLine("Cập nhật tất cả các ngày từ 1 tháng 9 năm 2025 đến 21 tháng 9 năm 2025");
                Console.WriteLine();

                // Tạo connection string
                var connectionString = "Data Source=ElectronicsStore.db";
                
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connectionString)
                    .Options;

                using var context = new ApplicationDbContext(options);
                
                try
                {
                    // Kiểm tra kết nối database
                    await context.Database.EnsureCreatedAsync();
                    
                    // Chạy script cập nhật
                    await UpdateDatesScript.UpdateAllDatesAsync(context);
                    
                    Console.WriteLine();
                    Console.WriteLine("✅ Cập nhật thành công!");
                    Console.WriteLine("Tất cả các ngày đã được cập nhật từ 1/9/2025 đến 21/9/2025");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi khi cập nhật: {ex.Message}");
                    Console.WriteLine($"Chi tiết: {ex.StackTrace}");
                }
                
                Console.WriteLine();
                Console.WriteLine("Nhấn phím bất kỳ để thoát...");
                Console.ReadKey();
            }
        }
    }
}