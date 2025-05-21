using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTO;

namespace Project_2025_Web.Services
{
    public class RoleService : IRoleService
    {
        private readonly DataContext _context;

        public RoleService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<List<RoleDTO>>> GetListAsync()
        {
            var roles = await _context.Roles
                .Select(r => new RoleDTO { Id = r.Id, Name = r.Name, Description = r.Description })
                .ToListAsync();

            return new Response<List<RoleDTO>>
            {
                IsSucess = true,
                Result = roles
            };
        }

        public async Task<Response<RoleDTO>> GetOneAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return new Response<RoleDTO>
                {
                    IsSucess = false,
                    Message = "Rol no encontrado"
                };
            }

            return new Response<RoleDTO>
            {
                IsSucess = true,
                Result = new RoleDTO { Id = role.Id, Name = role.Name, Description = role.Description }
            };
        }

        public async Task<Response<RoleDTO>> CreateAsync(RoleDTO dto)
        {
            var entity = new Role { Name = dto.Name, Description = dto.Description };
            _context.Roles.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return new Response<RoleDTO>
            {
                IsSucess = true,
                Result = dto
            };
        }

        public async Task<Response<RoleDTO>> EditAsync(RoleDTO dto)
        {
            var entity = await _context.Roles.FindAsync(dto.Id);
            if (entity == null)
            {
                return new Response<RoleDTO>
                {
                    IsSucess = false,
                    Message = "Rol no encontrado"
                };
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            _context.Roles.Update(entity);
            await _context.SaveChangesAsync();

            return new Response<RoleDTO>
            {
                IsSucess = true,
                Result = dto
            };
        }

        public async Task<Response<object>> DeleteAsync(int id)
        {
            var entity = await _context.Roles.FindAsync(id);
            if (entity == null)
            {
                return new Response<object>
                {
                    IsSucess = false,
                    Message = "Rol no encontrado"
                };
            }

            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync();

            return new Response<object>
            {
                IsSucess = true
            };
        }
    }
}

