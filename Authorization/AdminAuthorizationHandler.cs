using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Data;
using ElectronicsStore.Models;
using System.Security.Claims;

namespace ElectronicsStore.Authorization
{
    public class AdminRequirement : IAuthorizationRequirement
    {
    }

    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly ApplicationDbContext _context;

        public AdminAuthorizationHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            var user = await _context.Users.FindAsync(userId);

            if (user != null && user.IsAdmin && user.IsActive)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }

    public class SuperAdminRequirement : IAuthorizationRequirement
    {
    }

    public class SuperAdminAuthorizationHandler : AuthorizationHandler<SuperAdminRequirement>
    {
        private readonly ApplicationDbContext _context;

        public SuperAdminAuthorizationHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SuperAdminRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            var user = await _context.Users.FindAsync(userId);

            if (user != null && user.Role == UserRole.SuperAdmin && user.IsActive)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
