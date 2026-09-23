using Microsoft.EntityFrameworkCore;
using universityBooking.Domain.Entities;

namespace universityBooking.Infastructur.Date
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تهيئة بيانات القاعات الافتراضية
            modelBuilder.Entity<Hall>().HasData(
                new Hall
                {
                    Id = 1,
                    Name = "قاعة 101",
                    Building = "مبنى A - الدور الأول",
                    Capacity = 50,
                    Details = "مبنى الطلاب - الدور الأول، سعة 50 شخصاً",
                    Image = "assets/hall_101.png",
                    IsBooked = true,
                    BookedBy = "حفصة منصور",
                    BookingPurpose = "اجتماع",
                    BookingDate = "2026/04/07",
                    ActiveBookingId = 1
                },
                new Hall
                {
                    Id = 2,
                    Name = "قاعة 202",
                    Building = "مبنى B - الدور الثاني",
                    Capacity = 60,
                    Details = "مبنى العلوم - الدور الثاني، سعة 80 شخصاً",
                    Image = "assets/hall_202.png",
                    IsBooked = false
                },
                new Hall
                {
                    Id = 3,
                    Name = "قاعة 303",
                    Building = "مبنى C - الدور الثالث",
                    Capacity = 100,
                    Details = "مبنى الإدارة - الدور الثالث، سعة 100 شخصاً",
                    Image = "assets/hall_303.png",
                    IsBooked = false
                },
                new Hall
                {
                    Id = 4,
                    Name = "قاعة 404",
                    Building = "مبنى D - الدور الرابع",
                    Capacity = 60,
                    Details = "مبنى الهندسة - الدور الرابع، سعة 60 شخصاً",
                    Image = "assets/hall_404.png",
                    IsBooked = false
                }
            );

            // حجز افتراضي تجريبي
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = 1,
                    HallId = 1,
                    HallName = "قاعة 101",
                    Building = "مبنى A - الدور الأول",
                    Capacity = 50,
                    Purpose = "اجتماع",
                    Department = "علوم الحاسب",
                    AttendeesCount = 45,
                    Date = "2026/04/07",
                    StartTime = "10:00 ص",
                    EndTime = "12:00 م",
                    UserName = "حفصة منصور",
                    HallImage = "assets/hall_101.png",
                    Notes = "حجز تجريبي أولي",
                    Status = "مؤكد"
                }
            );

            // تهيئة المستخدمين الافتراضيين
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "حفصة منصور",
                    Email = "student@university.edu",
                    PasswordHash = "123456",
                    Role = "Student",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    FullName = "المدير المسؤول",
                    Email = "admin@university.edu",
                    PasswordHash = "123456",
                    Role = "Admin",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}

namespace universityBooking.Infastructur.Data
{
    public class ApplicationDbContext : universityBooking.Infastructur.Date.ApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<universityBooking.Infastructur.Date.ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
