namespace Project_2025_Web.DTO
{
    public class ReservationFilterDTO
    {
        public string? Status { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
