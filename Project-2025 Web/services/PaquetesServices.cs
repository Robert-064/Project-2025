using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Project_2025_Web.Services
{
    public interface IPlanService
    {
        Task<Response<Plan>> CreateAsync(PlanDTO dto);
        Task<Response<Plan>> EditeAsync(PlanDTO dto);
        Task<Response<object>> DeleteAsync(int id);
        Task<Response<PlanDTO>> GetOne(int id);
        Task<Response<List<Plan>>> GetListAsync();
        Task<List<Plan>> GetAllPaquetesAsync();
        Task<Plan?> GetPaqueteByIdAsync(int id);
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
                Plan plan = new Plan
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Basic_Price = dto.Basic_Price,
                    Type_Difficulty = dto.Type_Difficulty,
                    Max_Persons = dto.Max_Persons,
                    Distance = dto.Distance
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
                    Message = ex.Message
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
                    Message = ex.Message
                };
            }
        }

        public async Task<Response<object>> DeleteAsync(int id)
        {
            try
            {
                var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);
                if (plan == null)
                {
                    return new Response<object>
                    {
                        IsSucess = false,
                        Message = $"El plan con id '{id}' no existe"
                    };
                }

                _context.Plans.Remove(plan);
                await _context.SaveChangesAsync();

                return new Response<object>
                {
                    IsSucess = true,
                    Message = "Plan eliminado con éxito",
                    Result = plan
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
        public async Task<List<Plan>> GetAllPaquetesAsync()
        {
            try
            {
                return await _context.Plans.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de paquetes", ex);
            }
        }

        public async Task<Plan?> GetPaqueteByIdAsync(int id)
        {
            try
            {
                return await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el paquete con ID {id}", ex);
            }
        }
    }



}


