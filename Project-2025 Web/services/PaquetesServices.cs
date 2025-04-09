using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Project_2025_Web.Services
{
    public interface IPlanService
    {
        Task<Response<Plan>> CreateAsync(PlanDTO dto);
        Task<Response<Plan>> EditeAsync(PlanDTO dto);
        Task<Response<Plan>> DeleteAsync(int id);
        Task<Response<PlanDTO>> GetOne(int id);
        Task<Response<List<Plan>>> GetListAsync();
        Task<List<PlanDTO>> GetAllPaquetesAsync();
        Task<PlanDTO?> GetPaqueteByIdAsync(int id);


    }

    public class PlanService : IPlanService
    {
        private readonly DataContext _context;

        public PlanService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<Plan>> CreateAsync(PlanDTO dto)
        {
            try
            {
                // Lista de extensiones válidas
                var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var maxSizeInMB = 5;
                var maxSizeInBytes = maxSizeInMB * 1024 * 1024;
                string? imagePath1 = null;
                string? imagePath2 = null;

                // Método local para validar y guardar imágenes
                async Task<string?> SaveImageAsync(IFormFile file)
                {
                    if (file == null || file.Length == 0)
                        return null;

                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                        throw new Exception($"Extensión de archivo no válida: {extension}");

                    if (file.Length > maxSizeInBytes)
                        throw new Exception($"El archivo excede el tamaño máximo de {maxSizeInMB} MB");

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var path = Path.Combine("wwwroot/images/plans", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(path)!); // Asegura que la carpeta exista

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return $"/images/plans/{fileName}";
                }

                imagePath1 = await SaveImageAsync(dto.ImageFile1); //Aqui se emplea el metodo de validacion
                imagePath2 = await SaveImageAsync(dto.ImageFile2);

                Plan plan = new Plan
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Basic_Price = dto.Basic_Price,
                    Type_Difficulty = dto.Type_Difficulty,
                    Max_Persons = dto.Max_Persons,
                    Distance = dto.Distance,
                    ImageUrl1 = imagePath1,
                    ImageUrl2 = imagePath2
                };

                await _context.Plans.AddAsync(plan);
                await _context.SaveChangesAsync();

                return new Response<Plan>
                {
                    IsSucess = true,
                    Message = "Plan creado con éxito",
                    Result = plan
                };
            }
            catch (Exception ex)
            {
                return new Response<Plan>
                {
                    IsSucess = false,
                    Message = $"Error al crear el plan: {ex.Message}"
                };
            }
        }


        public async Task<Response<Plan>> EditeAsync(PlanDTO dto)
        {
            try
            {
                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == dto.Id);
                if (plan == null)
                {
                    return new Response<Plan>
                    {
                        IsSucess = false,
                        Message = $"El plan con id '{dto.Id}' no existe"
                    };
                }

                // Validaciones comunes
                var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var maxSizeInBytes = 5 * 1024 * 1024;

                async Task<string?> SaveImageAsync(IFormFile file)
                {
                    if (file == null || file.Length == 0)
                        return null;

                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                        throw new Exception($"Extensión no válida: {extension}");

                    if (file.Length > maxSizeInBytes)
                        throw new Exception("El archivo supera el tamaño máximo permitido (5MB)");

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var path = Path.Combine("wwwroot/images/plans", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(path)!); // Asegura que la carpeta exista

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return $"/images/plans/{fileName}";
                }

                // Procesar Imagen 1
                if (dto.ImageFile1 != null)
                {
                    // Eliminar la imagen anterior si existe
                    if (!string.IsNullOrEmpty(plan.ImageUrl1))
                    {
                        var oldPath = Path.Combine("wwwroot", plan.ImageUrl1.TrimStart('/'));
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                    }

                    plan.ImageUrl1 = await SaveImageAsync(dto.ImageFile1);
                }

                // Procesar Imagen 2
                if (dto.ImageFile2 != null)
                {
                    if (!string.IsNullOrEmpty(plan.ImageUrl2))
                    {
                        var oldPath = Path.Combine("wwwroot", plan.ImageUrl2.TrimStart('/'));
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                    }

                    plan.ImageUrl2 = await SaveImageAsync(dto.ImageFile2);
                }

                // Actualizar propiedades
                plan.Name = dto.Name;
                plan.Description = dto.Description;
                plan.Basic_Price = dto.Basic_Price;
                plan.Type_Difficulty = dto.Type_Difficulty;
                plan.Max_Persons = dto.Max_Persons;
                plan.Distance = dto.Distance;

                _context.Plans.Update(plan);
                await _context.SaveChangesAsync();

                return new Response<Plan>
                {
                    IsSucess = true,
                    Message = "Plan actualizado con éxito",
                    Result = plan
                };
            }
            catch (Exception ex)
            {
                return new Response<Plan>
                {
                    IsSucess = false,
                    Message = $"Error al editar el plan: {ex.Message}"
                };
            }
        }


        public async Task<Response<Plan>> DeleteAsync(int id)
        {
            try
            {
                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);
                if (plan == null)
                {
                    return new Response<Plan>
                    {
                        IsSucess = false,
                        Message = $"El plan con id '{id}' no existe"
                    };
                }

                // Ruta física del wwwroot
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Eliminar imagen 1 si existe
                if (!string.IsNullOrEmpty(plan.ImageUrl1))
                {
                    var imagePath1 = Path.Combine(rootPath, plan.ImageUrl1.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath1))
                    {
                        System.IO.File.Delete(imagePath1);
                    }
                }

                // Eliminar imagen 2 si existe
                if (!string.IsNullOrEmpty(plan.ImageUrl2))
                {
                    var imagePath2 = Path.Combine(rootPath, plan.ImageUrl2.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath2))
                    {
                        System.IO.File.Delete(imagePath2);
                    }
                }

                _context.Plans.Remove(plan);
                await _context.SaveChangesAsync();

                return new Response<Plan>
                {
                    IsSucess = true,
                    Message = "Plan eliminado correctamente",
                    Result = plan
                };
            }
            catch (Exception ex)
            {
                return new Response<Plan>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }



        public async Task<Response<PlanDTO>> GetOne(int id)
        {
            try
            {
                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);
                if (plan == null)
                {
                    return new Response<PlanDTO>
                    {
                        IsSucess = false,
                        Message = $"El plan con id '{id}' no existe"
                    };
                }

                PlanDTO dto = new PlanDTO
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    Basic_Price = plan.Basic_Price,
                    Type_Difficulty = plan.Type_Difficulty,
                    Max_Persons = plan.Max_Persons,
                    Distance = plan.Distance
                };

                return new Response<PlanDTO>
                {
                    IsSucess = true,
                    Message = "Plan obtenido con éxito",
                    Result = dto
                };
            }
            catch (Exception ex)
            {
                return new Response<PlanDTO>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response<List<Plan>>> GetListAsync()
        {
            try
            {
                var list = await _context.Plans.ToListAsync();
                return new Response<List<Plan>>
                {
                    IsSucess = true, 
                    Message = "Lista de planes obtenida con éxito",
                    Result = list
                };
            }
            catch (Exception ex)
            {
                return new Response<List<Plan>>
                {
                    
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<List<PlanDTO>> GetAllPaquetesAsync()
        {
            try
            {
                var planes = await _context.Plans.ToListAsync();

                var dtoList = planes.Select(plan => new PlanDTO
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    Basic_Price = plan.Basic_Price,
                    Type_Difficulty = plan.Type_Difficulty,
                    Max_Persons = plan.Max_Persons,
                    Distance = plan.Distance,
                    ImagePath1 = plan.ImageUrl1,
                    ImagePath2 = plan.ImageUrl2
                }).ToList();

                return dtoList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de paquetes", ex);
            }
        }


        public async Task<PlanDTO?> GetPaqueteByIdAsync(int id)
        {
            try
            {
                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);
                if (plan == null) return null;

                return new PlanDTO
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    Basic_Price = plan.Basic_Price,
                    Type_Difficulty = plan.Type_Difficulty,
                    Max_Persons = plan.Max_Persons,
                    Distance = plan.Distance,
                    ImagePath1 = plan.ImageUrl1,
                    ImagePath2 = plan.ImageUrl2
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el paquete con ID {id}", ex);
            }
        }
    }



}


