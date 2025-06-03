using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTO;
using Project_2025_Web.DTOs;
using Project_2025_Web.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
public interface IUserService
{
    Task<bool> RegisterAsync(User user, string password);
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User?> GetByUsernameAsync(string username);
    Task<Response<string>> RegisterUserAsync(UserRegisterDTO dto);
    Task<Response<UserTokenDTO>> RegisterApiUserAsync(UserRegisterDTO dto);
    bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
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
            RoleId = 2
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
    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key; // Clave aleatoria
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
            if (computedHash[i] != storedHash[i]) return false;
        return true;
    }

    public async Task<Response<UserTokenDTO>> RegisterApiUserAsync(UserRegisterDTO dto)
    {
        try
        {
            // Verificar si el usuario ya existe
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return new Response<UserTokenDTO>
                {
                    IsSucess = false,
                    Message = "El nombre de usuario ya está en uso.",
                    Errors = new List<string> { "Nombre de usuario duplicado." }
                };
            }
            // Crear hash de la contraseña
            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            // Crear nuevo usuario
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = dto.RoleId
            };
            // Guardar usuario en la base de datos
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // Generar token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("TuClaveSecretaSuperseguraDeAlMenos256Bits"); // Debe coincidir con la configuración
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, (await _context.Roles.FindAsync(user.RoleId))?.Name ?? "")
            }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Response<UserTokenDTO>
            {
                IsSucess = true,
                Message = "Usuario registrado correctamente",
                Result = new UserTokenDTO
                {
                    Token = tokenHandler.WriteToken(token),
                    Expiration = tokenDescriptor.Expires ?? DateTime.UtcNow.AddDays(7)
                }
            };
        }
        catch (Exception ex)
        {
            return new Response<UserTokenDTO>
            {
                IsSucess = false,
                Message = "Error al registrar el usuario",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}

