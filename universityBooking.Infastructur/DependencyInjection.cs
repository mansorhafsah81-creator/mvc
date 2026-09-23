using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using universityBooking.Domain.Interfaces;
using universityBooking.Infastructur.Date;
using universityBooking.Infastructur.Repositories;

namespace universityBooking.Infastructur
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
            bool isSqlServerWorking = false;

            if (!string.IsNullOrWhiteSpace(connectionString) && !connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var testConnStr = connectionString;
                    if (!testConnStr.Contains("Timeout", StringComparison.OrdinalIgnoreCase))
                    {
                        testConnStr += ";Connect Timeout=2;";
                    }

                    using var conn = new Microsoft.Data.SqlClient.SqlConnection(testConnStr);
                    conn.Open();
                    isSqlServerWorking = true;
                }
                catch
                {
                    isSqlServerWorking = false;
                }
            }

            if (isSqlServerWorking)
            {
                Console.WriteLine("[Database Provider]: Connected to SQL Server LocalDB successfully.");
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }
            else
            {
                string dbPath = Path.Combine(AppContext.BaseDirectory, "UniversityBooking.db");
                Console.WriteLine($"[Database Provider]: SQL Server not found on this computer. Using SQLite Database file: {dbPath}");
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite($"Data Source={dbPath}"));
            }

            services.AddScoped<IHallRepository, HallRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
