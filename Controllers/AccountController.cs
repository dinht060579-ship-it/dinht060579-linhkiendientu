using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using ElectronicsStore.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ElectronicsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && VerifyPassword(model.Password, user.PasswordHash ?? ""))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    user.LastLoginAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Redirect admin users to admin panel
                    if (user.IsAdmin)
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FullName.Split(' ').FirstOrDefault() ?? "",
                    LastName = model.FullName.Split(' ').Skip(1).FirstOrDefault() ?? "",
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Auto login after registration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, "Customer")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                TempData["SuccessMessage"] = "Đăng ký thành công! Chào mừng bạn đến với cửa hàng linh kiện điện tử.";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return View(model);
        }

        // POST: Account/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var user = await _context.Users.FindAsync(userId);

                if (user != null)
                {
                    var nameParts = model.FullName.Split(' ');
                    user.FirstName = nameParts.FirstOrDefault() ?? "";
                    user.LastName = nameParts.Skip(1).FirstOrDefault() ?? "";
                    user.PhoneNumber = model.PhoneNumber;
                    user.Address = model.Address;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                }
            }

            return View(model);
        }

        // GET: Account/ChangePassword
        public IActionResult ChangePassword()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var user = await _context.Users.FindAsync(userId);

                if (user != null)
                {
                    // Verify current password
                    if (BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
                    {
                        // Hash new password
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng");
                    }
                }
            }

            return View(model);
        }

        private string HashPassword(string password)
        {
            // Simple hash for demo - in production use BCrypt or similar
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        
        // GET: Account/AccessDenied
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
