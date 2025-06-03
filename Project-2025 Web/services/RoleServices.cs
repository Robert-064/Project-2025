using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTO;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Project_2025_Web.Services
{
    public interface IRoleService
    {
        Task<Response<Role>> CreateAsync(RoleDTO dto);
        Task<Response<Role>> EditAsync(RoleDTO dto);
        Task<Response<object>> DeleteAsync(int id);
        Task<Response<RoleDTO>> GetOneAsync(int id);
        Task<Response<List<Role>>> GetListAsync();
        Task<List<Role>> GetRolesPagedAsync(int pageNumber, int pageSize);
        Task<int> GetRolesCountAsync();
    }
    public class RoleService : IRoleService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public RoleService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Response<Role>> CreateAsync(RoleDTO dto)
        {
            try
            {
                var role = _mapper.Map<Role>(dto);
                await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();
                return new Response<Role>
                {
                    IsSucess = true,
                    Message = "Rol creado con éxito",
                    Result = role
                };
            }
            catch (Exception ex)
            {
                return new Response<Role>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response<Role>> EditAsync(RoleDTO dto)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == dto.Id);
                if (role == null)
                {
                    return new Response<Role>
                    {
                        IsSucess = false,
                        Message = $"El rol con id '{dto.Id}' no existe"
                    };
                }
                _mapper.Map(dto, role);
                _context.Roles.Update(role);
                await _context.SaveChangesAsync();
                return new Response<Role>
                {
                    IsSucess = true,
                    Message = "Rol actualizado con éxito",
                    Result = role
                };
            }
            catch (Exception ex)
            {
                return new Response<Role>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response<object>> DeleteAsync(int id)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
                if (role == null)
                {
                    return new Response<object>
                    {
                        IsSucess = false,
                        Message = $"El rol con id '{id}' no existe"
                    };
                }
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return new Response<object>
                {
                    IsSucess = true,
                    Message = "Rol eliminado con éxito"
                };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response<RoleDTO>> GetOneAsync(int id)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(r => r.Id == id);
                if (role == null)
                {
                    return new Response<RoleDTO>
                    {
                        IsSucess = false,
                        Message = $"El rol con id '{id}' no existe"
                    };
                }
                var dto = _mapper.Map<RoleDTO>(role);
                return new Response<RoleDTO>
                {
                    IsSucess = true,
                    Message = "Rol obtenido con éxito",
                    Result = dto
                };
            }
            catch (Exception ex)
            {
                return new Response<RoleDTO>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response<List<Role>>> GetListAsync()
        {
            try
            {
                var roles = await _context.Roles
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .ToListAsync();
                return new Response<List<Role>>
                {
                    IsSucess = true,
                    Message = "Lista de roles obtenida con éxito",
                    Result = roles
                };
            }
            catch (Exception ex)
            {
                return new Response<List<Role>>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<List<Role>> GetRolesPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Roles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetRolesCountAsync()
        {
            return await _context.Roles.CountAsync();
        }
    }
}