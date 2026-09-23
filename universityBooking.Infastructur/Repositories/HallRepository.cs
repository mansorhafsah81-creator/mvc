using Microsoft.EntityFrameworkCore;
using universityBooking.Domain.Entities;
using universityBooking.Domain.Interfaces;
using universityBooking.Infastructur.Date;

namespace universityBooking.Infastructur.Repositories
{
    public class HallRepository : IHallRepository
    {
        private readonly ApplicationDbContext _context;

        public HallRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hall>> GetAllHallsAsync()
        {
            return await _context.Halls
                .AsNoTracking()
                .OrderBy(h => h.Id)
                .ToListAsync();
        }

        public async Task<Hall?> GetHallByIdAsync(int id)
        {
            return await _context.Halls.FindAsync(id);
        }

        public async Task<Hall?> GetHallByNameAsync(string name)
        {
            return await _context.Halls.FirstOrDefaultAsync(h => h.Name.Trim().ToLower() == name.Trim().ToLower());
        }

        public async Task<Hall> AddHallAsync(Hall hall)
        {
            _context.Halls.Add(hall);
            await _context.SaveChangesAsync();
            return hall;
        }

        public async Task UpdateHallAsync(Hall hall)
        {
            _context.Halls.Update(hall);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHallAsync(int id)
        {
            var hall = await _context.Halls.FindAsync(id);
            if (hall != null)
            {
                var relatedBookings = await _context.Bookings
                    .Where(b => b.HallId == id || (!string.IsNullOrEmpty(hall.Name) && b.HallName == hall.Name))
                    .ToListAsync();

                if (relatedBookings.Any())
                {
                    _context.Bookings.RemoveRange(relatedBookings);
                }

                _context.Halls.Remove(hall);
                await _context.SaveChangesAsync();
            }
        }
    }
}

