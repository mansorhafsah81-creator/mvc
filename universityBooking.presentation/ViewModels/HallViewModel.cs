namespace universityBooking.presentation.ViewModels
{
    public class HallViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Details { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public bool IsBooked { get; set; }
        public string BookedBy { get; set; } = string.Empty;
        public string BookingPurpose { get; set; } = string.Empty;
        public string BookingDate { get; set; } = string.Empty;
        public int? ActiveBookingId { get; set; }
    }

    public class BookingViewModel
    {
        public int Id { get; set; }
        public int? HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int AttendeesCount { get; set; }
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string HallImage { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class DashboardViewModel
    {
        public int BookingsCount { get; set; }
        public int HallsCount { get; set; }
        public IEnumerable<BookingViewModel> RecentBookings { get; set; } = new List<BookingViewModel>();
        public IEnumerable<HallViewModel> AvailableHalls { get; set; } = new List<HallViewModel>();
    }
}
