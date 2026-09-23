using universityBooking.Domain.Entities;

namespace universityBooking.Domain.Interfaces
{
    public interface IHallRepository
    {
        Task<IEnumerable<Hall>> GetAllHallsAsync();
        Task<Hall?> GetHallByIdAsync(int id);
        Task<Hall?> GetHallByNameAsync(string name);
        Task<Hall> AddHallAsync(Hall hall);
        Task UpdateHallAsync(Hall hall);
        Task DeleteHallAsync(int id);
    }
}
