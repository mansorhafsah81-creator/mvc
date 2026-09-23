using universityBooking.Application.DTOs;

namespace universityBooking.Application.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task<BookingDto> CreateBookingAsync(CreateBookingDto dto);
        Task<bool> CancelBookingAsync(int id);
        Task<int> GetBookingsCountAsync();
    }
}
