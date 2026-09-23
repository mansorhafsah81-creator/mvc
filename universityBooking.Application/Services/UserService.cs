using universityBooking.Application.Interfaces;
using universityBooking.Domain.Entities;
using universityBooking.Domain.Interfaces;

namespace universityBooking.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            return await _userRepository.GetUserByEmailAndPasswordAsync(email, password);
        }

        public async Task<User?> RegisterAsync(string fullName, string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
                return null;

            var user = new User
            {
                FullName = string.IsNullOrWhiteSpace(fullName) ? "مستخدم جديد" : fullName,
                Email = email,
                PasswordHash = password,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            return await _userRepository.AddUserAsync(user);
        }
    }
}
