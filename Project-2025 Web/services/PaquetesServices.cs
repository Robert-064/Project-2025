namespace Project_2025_Web.Services
{
    public class PaquetesService : IPaquetesService
    {
        private readonly ApplicationDbContext _context;

        public PaquetesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaqueteDTO> GetPackageByIdAsync(int id)
        {
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
            {
                return null;
            }

            return new PaqueteDTO
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Price = package.Price,
                MinPeople = package.MinPeople,
                MaxPeople = package.MaxPeople,
                Difficulty = package.Difficulty,  
                Distance = package.Distance  
            };
        }
    }
}

