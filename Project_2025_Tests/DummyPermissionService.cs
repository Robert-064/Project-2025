using System.Threading.Tasks;
using Project_2025_Web.Services;

namespace Project_2025_Tests
{
    /// <summary>
    /// Implementación de IPermissionService que siempre devuelve true.
    /// De esta forma las pruebas de integración / unitarias no fallan
    /// por falta de permisos, ya que UserHasPermissionAsync siempre retorna true.
    /// </summary>
    public class DummyPermissionService : IPermissionService
    {
        public Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            // Siempre permitimos cualquier permiso
            return Task.FromResult(true);
        }
    }
}
