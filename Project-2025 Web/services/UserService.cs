using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTOs;
using Project_2025_Web.Services;

public interface IUserService
{
    Task<bool> RegisterAsync(User user, string password);
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User?> GetByUsernameAsync(string username);
    Task<Response<string>> RegisterUserAsync(UserRegisterDTO dto);
}

public class UserService : IUserService
{
    private readonly DataContext _context;
    public UserService(DataContext context)
    {
        _context = context;
    }

    public async Task<Response<string>> RegisterUserAsync(UserRegisterDTO dto)
    {
        var response = new Response<string>();

        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
        {
            response.IsSucess = false;
            response.Message = "El nombre de usuario ya está en uso.";
            response.Errors = new List<string> { "Nombre de usuario duplicado." };
            return response;
        }

        CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            RoleId = dto.RoleId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        response.IsSucess = true;
        response.Message = "Usuario registrado correctamente.";
        response.Result = user.Username;

        return response;
    }
    public async Task<bool> RegisterAsync(User user, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            return false; // Usuario ya existe

        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return null;

        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            return null;

        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);
    }

    // Métodos privados para hash de contraseña
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key; // Clave aleatoria
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
            if (computedHash[i] != storedHash[i]) return false;
        return true;
    }
}

