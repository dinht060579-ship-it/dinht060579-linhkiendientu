using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Models;
using ElectronicsStore.Data;

namespace ElectronicsStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Get featured products
        var featuredProducts = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive && p.IsFeatured)
            .Take(8)
            .ToListAsync();

        // Get new products
        var newProducts = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive && p.IsNewProduct)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();

        // Get best selling products
        var bestSellingProducts = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.SoldCount)
            .Take(8)
            .ToListAsync();

        // Get categories
        var categories = await _context.Categories
            .Where(c => c.IsActive && c.ParentCategoryId == null)
            .Take(8)
            .ToListAsync();

        // Get brands
        var brands = await _context.Brands
            .Where(b => b.IsActive)
            .Take(10)
            .ToListAsync();

        ViewBag.FeaturedProducts = featuredProducts;
        ViewBag.NewProducts = newProducts;
        ViewBag.BestSellingProducts = bestSellingProducts;
        ViewBag.Categories = categories;
        ViewBag.Brands = brands;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
