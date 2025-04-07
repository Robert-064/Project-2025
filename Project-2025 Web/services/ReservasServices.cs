public interface IReservaService
{
    Task<List<ReservaDTO>> GetUserReservationsAsync(string userName);
    Task<Response<ReservaDTO>> CreateReservationAsync(ReservaDTO model);
    Task<Response> CancelReservationAsync(int reservationId);
}

public class ReservaService : IReservaService
{
    private readonly ApplicationDbContext _context;
    private readonly IPaqueteService _paqueteService;

    public ReservaService(ApplicationDbContext context, IPaqueteService paqueteService)
    {
        _context = context;
        _paqueteService = paqueteService;
    }

    public async Task<List<ReservaDTO>> GetUserReservationsAsync(string userName)
    {
        var reservas = await _context.Reservas
            .Where(r => r.UserName == userName)
            .Include(r => r.Paquete)  
            .ToListAsync();

        return reservas.Select(r => new ReservaDTO
        {
            Id = r.Id,
            PaqueteId = r.PaqueteId,
            PaqueteName = r.Paquete.Name,
            Difficulty = r.Paquete.Difficulty,
            Distance = r.Paquete.Distance,
            NumberOfPeople = r.NumberOfPeople,
            Fecha = r.Fecha
        }).ToList();
    }

    public async Task<Response<ReservaDTO>> CreateReservationAsync(ReservaDTO model)
    {
        var paquete = await _paqueteService.GetPaqueteByIdAsync(model.PaqueteId);

        if (paquete == null)
        {
            return new Response<ReservaDTO> { IsSuccess = false, Message = "Paquete no encontrado" };
        }

        if (model.NumberOfPeople < paquete.MinPeople || model.NumberOfPeople > paquete.MaxPeople)
        {
            return new Response<ReservaDTO> { IsSuccess = false, Message = $"El número de personas debe estar entre {paquete.MinPeople} y {paquete.MaxPeople}" };
        }

        var reserva = new Reserva
        {
            PaqueteId = model.PaqueteId,
            UserName = model.UserName,
            NumberOfPeople = model.NumberOfPeople,
            Fecha = model.Fecha
        };

        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();

        return new Response<ReservaDTO>
        {
            IsSuccess = true,
            Message = "Reserva creada con éxito",
            Result = model
        };
    }

    public async Task<Response> CancelReservationAsync(int reservationId)
    {
        var reserva = await _context.Reservas.FindAsync(reservationId);
        if (reserva == null)
        {
            return new Response { IsSuccess = false, Message = "Reserva no encontrada" };
        }

        _context.Reservas.Remove(reserva);
        await _context.SaveChangesAsync();

        return new Response { IsSuccess = true, Message = "Reserva cancelada con éxito" };
    }
}



