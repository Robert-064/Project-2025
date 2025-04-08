using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTOs;
using Microsoft.EntityFrameworkCore;


namespace Project_2025_Web.Services
{
    public interface IReservationService
    {
        Task<Response<Reservation>> CreateAsync(ReservationDTO dto);
        Task<Response<Reservation>> EditeAsync(ReservationDTO dto);
        Task<Response<object>> DeleteAsync(int id);
        Task<Response<ReservationDTO>> GetOne(int id);
        Task<Response<List<Reservation>>> GetListAsync();
        Task<Response<List<ReservationDTO>>> GetUserReservationsAsync(int userId);
    }


    public class ReservationService : IReservationService
    {
        private readonly DataContext _context;

        public ReservationService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<Reservation>> CreateAsync(ReservationDTO dto)
        {
            try
            {
                var reservation = new Reservation
                {
                    Id_Plan = dto.Id_Plan,
                    Id_User = dto.Id_User,
                    Date = dto.Date,
                    Status = dto.Status,
                    Person_Number = dto.Person_Number
                };

                await _context.Reservations.AddAsync(reservation);
                await _context.SaveChangesAsync();

                return new Response<Reservation>
                {
                    IsSucess = true,
                    Message = "Reserva creada con éxito",
                    Result = reservation
                };
            }
            catch (Exception ex)
            {
                return new Response<Reservation>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response<Reservation>> EditeAsync(ReservationDTO dto)
        {
            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == dto.Id);
                if (reservation == null)
                {
                    return new Response<Reservation>
                    {
                        IsSucess = false,
                        Message = $"La reserva con id '{dto.Id}' no existe"
                    };
                }

                reservation.Id_Plan = dto.Id_Plan;
                reservation.Id_User = dto.Id_User;
                reservation.Date = dto.Date;
                reservation.Status = dto.Status;
                reservation.Person_Number = dto.Person_Number;

                _context.Reservations.Update(reservation);
                await _context.SaveChangesAsync();

                return new Response<Reservation>
                {
                    IsSucess = true,
                    Message = "Reserva actualizada con éxito",
                    Result = reservation
                };
            }
            catch (Exception ex)
            {
                return new Response<Reservation>
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
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
                if (reservation == null)
                {
                    return new Response<object>
                    {
                        IsSucess = false,
                        Message = $"La reserva con id '{id}' no existe"
                    };
                }

                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();

                return new Response<object>
                {
                    IsSucess = true,
                    Message = "Reserva eliminada con éxito",
                    Result = reservation
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

        public async Task<Response<ReservationDTO>> GetOne(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
                if (reservation == null)
                {
                    return new Response<ReservationDTO>
                    {
                        IsSucess = false,
                        Message = $"La reserva con id '{id}' no existe"
                    };
                }

                var dto = new ReservationDTO
                {
                    Id = reservation.Id,
                    Id_Plan = reservation.Id_Plan,
                    Id_User = reservation.Id_User,
                    Date = reservation.Date,
                    Status = reservation.Status,
                    Person_Number = reservation.Person_Number
                };

                return new Response<ReservationDTO>
                {
                    IsSucess = true,
                    Message = "Reserva obtenida con éxito",
                    Result = dto
                };
            }
            catch (Exception ex)
            {
                return new Response<ReservationDTO>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response<List<Reservation>>> GetListAsync()
        {
            try
            {
                var list = await _context.Reservations.ToListAsync();
                return new Response<List<Reservation>>
                {
                    IsSucess = true,
                    Message = "Lista de reservas obtenida con éxito",
                    Result = list
                };
            }
            catch (Exception ex)
            {
                return new Response<List<Reservation>>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Response<List<ReservationDTO>>> GetUserReservationsAsync(int userId)
        {
            try
            {
                var reservations = await _context.Reservations
                    .Where(r => r.Id_User == userId)
                    .Select(r => new ReservationDTO
                    {
                        Id = r.Id,
                        Id_Plan = r.Id_Plan,
                        Id_User = r.Id_User,
                        Date = r.Date,
                        Status = r.Status,
                        Person_Number = r.Person_Number
                    })
                    .ToListAsync();

                return new Response<List<ReservationDTO>>
                {
                    IsSucess = true,
                    Message = "Reservas del usuario obtenidas con éxito",
                    Result = reservations
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ReservationDTO>>
                {
                    IsSucess = false,
                    Message = ex.Message
                };
            }
        }
    }
}








