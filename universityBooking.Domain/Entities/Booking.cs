namespace universityBooking.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int? HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public string? Building { get; set; } = string.Empty;
        public int Capacity { get; set; } = 50;
        public string? Purpose { get; set; } = string.Empty;
        public string? Department { get; set; } = string.Empty;
        public int AttendeesCount { get; set; }
        public string? Date { get; set; } = string.Empty;
        public string? StartTime { get; set; } = "10:00 ص";
        public string? EndTime { get; set; } = "12:00 م";
        public string? UserName { get; set; } = "حفصة منصور";
        public string? HallImage { get; set; } = "assets/hall_101.png";
        public string? Notes { get; set; } = string.Empty;
        public string? Status { get; set; } = "مؤكد";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
