using Microsoft.Extensions.DependencyInjection;
using universityBooking.Application.Interfaces;
using universityBooking.Application.Services;

namespace universityBooking.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IHallService, HallService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
