using universityBooking.Application.DTOs;
using universityBooking.Application.Interfaces;
using universityBooking.Domain.Entities;
using universityBooking.Domain.Interfaces;

namespace universityBooking.Application.Services
{
    public class HallService : IHallService
    {
        private readonly IHallRepository _hallRepository;
        private readonly IBookingRepository _bookingRepository;

        public HallService(IHallRepository hallRepository, IBookingRepository bookingRepository)
        {
            _hallRepository = hallRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<HallDto>> GetAllHallsAsync()
        {
            var halls = await _hallRepository.GetAllHallsAsync();
            var bookings = await _bookingRepository.GetAllBookingsAsync();

            var list = new List<HallDto>();
            foreach (var h in halls)
            {
                var activeBooking = bookings.FirstOrDefault(b => b.HallName == h.Name || b.HallId == h.Id);
                list.Add(new HallDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Building = h.Building,
                    Capacity = h.Capacity,
                    Details = h.Details,
                    Image = h.Image,
                    IsBooked = activeBooking != null,
                    BookedBy = activeBooking?.UserName ?? string.Empty,
                    BookingPurpose = activeBooking?.Purpose ?? string.Empty,
                    BookingDate = activeBooking?.Date ?? string.Empty,
                    ActiveBookingId = activeBooking?.Id
                });
            }

            return list;
        }

        public async Task<HallDto?> GetHallByIdAsync(int id)
        {
            var h = await _hallRepository.GetHallByIdAsync(id);
            if (h == null) return null;

            var bookings = await _bookingRepository.GetAllBookingsAsync();
            var activeBooking = bookings.FirstOrDefault(b => b.HallName == h.Name || b.HallId == h.Id);

            return new HallDto
            {
                Id = h.Id,
                Name = h.Name,
                Building = h.Building,
                Capacity = h.Capacity,
                Details = h.Details,
                Image = h.Image,
                IsBooked = activeBooking != null,
                BookedBy = activeBooking?.UserName ?? string.Empty,
                BookingPurpose = activeBooking?.Purpose ?? string.Empty,
                BookingDate = activeBooking?.Date ?? string.Empty,
                ActiveBookingId = activeBooking?.Id
            };
        }

        public async Task<HallDto> AddHallAsync(HallDto dto)
        {
            var hall = new Hall
            {
                Name = dto.Name,
                Building = dto.Building,
                Capacity = dto.Capacity,
                Details = dto.Details,
                Image = string.IsNullOrWhiteSpace(dto.Image) ? "assets/hall_101.png" : dto.Image,
                IsBooked = dto.IsBooked,
                BookedBy = dto.BookedBy,
                BookingPurpose = dto.BookingPurpose,
                BookingDate = dto.BookingDate
            };

            var created = await _hallRepository.AddHallAsync(hall);
            dto.Id = created.Id;
            return dto;
        }

        public async Task UpdateHallAsync(HallDto dto)
        {
            var hall = new Hall
            {
                Id = dto.Id,
                Name = dto.Name,
                Building = dto.Building,
                Capacity = dto.Capacity,
                Details = dto.Details,
                Image = dto.Image,
                IsBooked = dto.IsBooked,
                BookedBy = dto.BookedBy,
                BookingPurpose = dto.BookingPurpose,
                BookingDate = dto.BookingDate,
                ActiveBookingId = dto.ActiveBookingId
            };

            await _hallRepository.UpdateHallAsync(hall);
        }

        public async Task DeleteHallAsync(int id)
        {
            var hall = await _hallRepository.GetHallByIdAsync(id);
            if (hall != null)
            {
                var bookings = await _bookingRepository.GetAllBookingsAsync();
                var relatedBookings = bookings.Where(b => b.HallId == id || (!string.IsNullOrEmpty(hall.Name) && b.HallName == hall.Name)).ToList();
                foreach (var booking in relatedBookings)
                {
                    await _bookingRepository.DeleteBookingAsync(booking.Id);
                }
            }
            await _hallRepository.DeleteHallAsync(id);
        }

        public async Task<int> GetHallsCountAsync()
        {
            var halls = await _hallRepository.GetAllHallsAsync();
            return halls.Count();
        }
    }
}
