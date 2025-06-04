using System.Threading.Tasks;
using Project_2025_Web.Services;

namespace Project_2025_Tests
{
    public class DummyPermissionService : IPermissionService
    {
        public Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
           
            return Task.FromResult(true);
        }
    }
}
