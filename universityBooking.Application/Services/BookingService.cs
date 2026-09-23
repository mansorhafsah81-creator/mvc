using universityBooking.Application.DTOs;
using universityBooking.Application.Interfaces;
using universityBooking.Domain.Entities;
using universityBooking.Domain.Interfaces;

namespace universityBooking.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHallRepository _hallRepository;

        public BookingService(IBookingRepository bookingRepository, IHallRepository hallRepository)
        {
            _bookingRepository = bookingRepository;
            _hallRepository = hallRepository;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                HallId = b.HallId,
                HallName = b.HallName,
                Building = b.Building,
                Capacity = b.Capacity,
                Purpose = b.Purpose,
                Department = b.Department,
                AttendeesCount = b.AttendeesCount,
                Date = b.Date,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                UserName = b.UserName,
                HallImage = b.HallImage,
                Notes = b.Notes,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            });
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var b = await _bookingRepository.GetBookingByIdAsync(id);
            if (b == null) return null;

            return new BookingDto
            {
                Id = b.Id,
                HallId = b.HallId,
                HallName = b.HallName,
                Building = b.Building,
                Capacity = b.Capacity,
                Purpose = b.Purpose,
                Department = b.Department,
                AttendeesCount = b.AttendeesCount,
                Date = b.Date,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                UserName = b.UserName,
                HallImage = b.HallImage,
                Notes = b.Notes,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            };
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto)
        {
            // تحديد صورة القاعة تلقائياً إذا لم تحدد
            string image = !string.IsNullOrWhiteSpace(dto.HallImage)
                ? dto.HallImage
                : GetDefaultImageForHall(dto.HallName ?? "");

            var booking = new Booking
            {
                HallName = string.IsNullOrWhiteSpace(dto.HallName) ? "قاعة 101" : dto.HallName,
                Building = string.IsNullOrWhiteSpace(dto.Building) ? "مبنى A" : dto.Building,
                Capacity = dto.Capacity ?? (dto.AttendeesCount > 0 ? dto.AttendeesCount.Value : 50),
                Purpose = string.IsNullOrWhiteSpace(dto.Purpose) ? "حجز قاعة" : dto.Purpose,
                Department = string.IsNullOrWhiteSpace(dto.Department) ? "علوم الحاسب" : dto.Department,
                AttendeesCount = dto.AttendeesCount ?? 40,
                Date = string.IsNullOrWhiteSpace(dto.Date) ? DateTime.Now.ToString("yyyy/MM/dd") : dto.Date,
                StartTime = string.IsNullOrWhiteSpace(dto.StartTime) ? "10:00 ص" : dto.StartTime,
                EndTime = string.IsNullOrWhiteSpace(dto.EndTime) ? "12:00 م" : dto.EndTime,
                UserName = string.IsNullOrWhiteSpace(dto.UserName) ? "حفصة منصور" : dto.UserName,
                HallImage = image,
                Notes = dto.Notes ?? string.Empty,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "مؤكد" : dto.Status,
                CreatedAt = DateTime.Now
            };

            // ربط القاعة وتحديث حالتها
            var existingHall = await _hallRepository.GetHallByNameAsync(booking.HallName);
            if (existingHall != null)
            {
                booking.HallId = existingHall.Id;
                if (!string.IsNullOrWhiteSpace(existingHall.Image))
                {
                    booking.HallImage = existingHall.Image;
                }
                existingHall.IsBooked = true;
                existingHall.BookedBy = booking.UserName;
                existingHall.BookingPurpose = booking.Purpose;
                existingHall.BookingDate = booking.Date;
                await _hallRepository.UpdateHallAsync(existingHall);
            }
            else
            {
                // إذا أضاف قاعة جديدة غير مسجلة مسبقاً، نضيفها كقاعة في النظام
                var newHall = new Hall
                {
                    Name = booking.HallName,
                    Building = booking.Building,
                    Capacity = booking.Capacity,
                    Details = $"{booking.Building} - سعة {booking.Capacity} شخصاً",
                    Image = booking.HallImage,
                    IsBooked = true,
                    BookedBy = booking.UserName,
                    BookingPurpose = booking.Purpose,
                    BookingDate = booking.Date
                };
                var addedHall = await _hallRepository.AddHallAsync(newHall);
                booking.HallId = addedHall.Id;
            }

            var created = await _bookingRepository.AddBookingAsync(booking);

            // ربط ActiveBookingId في القاعة
            if (booking.HallId.HasValue)
            {
                var hallToUpdate = await _hallRepository.GetHallByIdAsync(booking.HallId.Value);
                if (hallToUpdate != null)
                {
                    hallToUpdate.ActiveBookingId = created.Id;
                    await _hallRepository.UpdateHallAsync(hallToUpdate);
                }
            }

            return new BookingDto
            {
                Id = created.Id,
                HallId = created.HallId,
                HallName = created.HallName,
                Building = created.Building,
                Capacity = created.Capacity,
                Purpose = created.Purpose,
                Department = created.Department,
                AttendeesCount = created.AttendeesCount,
                Date = created.Date,
                StartTime = created.StartTime,
                EndTime = created.EndTime,
                UserName = created.UserName,
                HallImage = created.HallImage,
                Notes = created.Notes,
                Status = created.Status,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null) return false;

            // تحديث حالة القاعة المرتبطة لتصبح شاغرة
            var hall = await _hallRepository.GetHallByNameAsync(booking.HallName);
            if (hall != null)
            {
                hall.IsBooked = false;
                hall.BookedBy = string.Empty;
                hall.BookingPurpose = string.Empty;
                hall.BookingDate = string.Empty;
                hall.ActiveBookingId = null;
                await _hallRepository.UpdateHallAsync(hall);
            }

            await _bookingRepository.DeleteBookingAsync(id);
            return true;
        }

        public async Task<int> GetBookingsCountAsync()
        {
            return await _bookingRepository.GetBookingsCountAsync();
        }

        private string GetDefaultImageForHall(string hallName)
        {
            if (hallName.Contains("101")) return "assets/hall_101.png";
            if (hallName.Contains("202")) return "assets/hall_202.png";
            if (hallName.Contains("303")) return "assets/hall_303.png";
            if (hallName.Contains("404")) return "assets/hall_404.png";
            return "assets/hall_101.png";
        }
    }
}
