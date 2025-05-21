using Project_2025_Web.DTO;
using Project_2025_Web.Data.Entities;

namespace Project_2025_Web.Services
{
    public interface IRoleService
    {
        Task<Response<List<RoleDTO>>> GetListAsync();
        Task<Response<RoleDTO>> GetOneAsync(int id);
        Task<Response<RoleDTO>> CreateAsync(RoleDTO dto);
        Task<Response<RoleDTO>> EditAsync(RoleDTO dto);
        Task<Response<object>> DeleteAsync(int id);
    }
}
