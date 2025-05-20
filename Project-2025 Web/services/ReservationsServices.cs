using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTOs;
using Microsoft.EntityFrameworkCore;
using Project_2025_Web.DTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_2025_Web.Services
{
    public interface IReservationService
    {
        Task<Response<Reservation>> CreateAsync(ReservationDTO dto);
        Task<Response<Reservation>> EditAsync(ReservationDTO dto);
        Task<Response<object>> DeleteAsync(int id);
        Task<Response<ReservationDTO>> GetOneAsync(int id);
        Task<Response<List<ReservationDTO>>> GetListAsync();
        Task<Response<List<ReservationDTO>>> GetUserReservationsAsync(int userId);
        Task<List<Reservation>> GetReservationPagedAsync(int pageNumber, int pageSize);
        Task<int> GetReservationCountAsync();
        Task<List<ReservationDTO>> GetFilteredAsync(ReservationFilterDTO filter);
    }

    public class ReservationService : IReservationService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReservationService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ReservationDTO>> GetFilteredAsync(ReservationFilterDTO filter)
        {
            var query = _context.Reservations.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(r => r.Status == filter.Status);

            if (filter.UserId.HasValue)
                query = query.Where(r => r.Id_User == filter.UserId.Value);

            if (filter.DateFrom.HasValue)
                query = query.Where(r => r.Date >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(r => r.Date <= filter.DateTo.Value);

            var list = await query.ToListAsync();

            return _mapper.Map<List<ReservationDTO>>(list);
        }

        public async Task<List<Reservation>> GetReservationPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Reservations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetReservationCountAsync()
        {
            return await _context.Reservations.CountAsync();
        }

        public async Task<Response<Reservation>> CreateAsync(ReservationDTO dto)
        {
            try
            {
                var reservation = _mapper.Map<Reservation>(dto);

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

        public async Task<Response<Reservation>> EditAsync(ReservationDTO dto)
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

                _mapper.Map(dto, reservation);

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

        public async Task<Response<ReservationDTO>> GetOneAsync(int id)
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

                var dto = _mapper.Map<ReservationDTO>(reservation);

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

        public async Task<Response<List<ReservationDTO>>> GetListAsync()
        {
            var reservations = await _context.Reservations.ToListAsync();

            var dtos = _mapper.Map<List<ReservationDTO>>(reservations);

            return new Response<List<ReservationDTO>>
            {
                Result = dtos,
                IsSucess = true
            };
        }

        public async Task<Response<List<ReservationDTO>>> GetUserReservationsAsync(int userId)
        {
            try
            {
                var reservations = await _context.Reservations
                    .Where(r => r.Id_User == userId)
                    .ToListAsync();

                var dtos = _mapper.Map<List<ReservationDTO>>(reservations);

                return new Response<List<ReservationDTO>>
                {
                    IsSucess = true,
                    Message = "Reservas del usuario obtenidas con éxito",
                    Result = dtos
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









