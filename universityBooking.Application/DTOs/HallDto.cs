namespace universityBooking.Application.DTOs
{
    public class HallDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Building { get; set; } = string.Empty;
        public int Capacity { get; set; } = 50;
        public string? Details { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public bool IsBooked { get; set; }
        public string? BookedBy { get; set; } = string.Empty;
        public string? BookingPurpose { get; set; } = string.Empty;
        public string? BookingDate { get; set; } = string.Empty;
        public int? ActiveBookingId { get; set; }
    }
}
