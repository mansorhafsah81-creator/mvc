using universityBooking.Application.DTOs;

namespace universityBooking.Application.Interfaces
{
    public interface IHallService
    {
        Task<IEnumerable<HallDto>> GetAllHallsAsync();
        Task<HallDto?> GetHallByIdAsync(int id);
        Task<HallDto> AddHallAsync(HallDto hallDto);
        Task UpdateHallAsync(HallDto hallDto);
        Task DeleteHallAsync(int id);
        Task<int> GetHallsCountAsync();
    }
}
