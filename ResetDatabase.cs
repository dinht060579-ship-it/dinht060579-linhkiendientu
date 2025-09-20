using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;

namespace ElectronicsStore
{
    public static class DatabaseReset
    {
        public static async Task ResetDatabaseAsync(ApplicationDbContext context)
        {
            // Xóa database cũ
            await context.Database.EnsureDeletedAsync();
            
            // Tạo database mới
            await context.Database.EnsureCreatedAsync();
            
            // Seed dữ liệu mới
            await SeedData.Initialize(context);
            await DataSeeder.SeedAsync(context);
            await DataSeeder.SeedSampleCustomers(context);
            
            Console.WriteLine("Database đã được reset thành công với dữ liệu từ 1/9/2025 đến 21/9/2025!");
        }
    }
}