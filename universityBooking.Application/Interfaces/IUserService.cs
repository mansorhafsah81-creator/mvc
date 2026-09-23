using universityBooking.Domain.Entities;

namespace universityBooking.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<User?> RegisterAsync(string fullName, string email, string password);
    }
}
