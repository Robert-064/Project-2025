using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Project_2025_Web.Data.Entities;  
using Project_2025_Web.Data;           

namespace Project_2025_Web.Services
{
    // Interfaz del servicio de permisos
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(int userId, string permissionName);
    }

    // Implementación del servicio
    public class PermissionService : IPermissionService
    {
        private readonly DataContext _context;

        public PermissionService(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Role == null)
                return false;

            return user.Role.RolePermissions.Any(rp => rp.Permission.Name == permissionName);
        }
    }

    // Atributo para usar en los controladores o acciones
    public class AuthorizePermissionAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _permission;

        public AuthorizePermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
                return;
            }

            var service = context.HttpContext.RequestServices.GetService(typeof(IPermissionService)) as IPermissionService;
            if (service == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            bool hasPermission = await service.UserHasPermissionAsync(userId, _permission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}

