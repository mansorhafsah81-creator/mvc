using Microsoft.EntityFrameworkCore;
using universityBooking.Application;
using universityBooking.Infastructur;
using universityBooking.Infastructur.Date;

var builder = WebApplication.CreateBuilder(args);

// ضمان تعيين مسار wwwroot ومجلد الصور بشكل صحيح دائماً
if (string.IsNullOrEmpty(builder.Environment.WebRootPath))
{
    var contentPath = builder.Environment.ContentRootPath;
    var candidateWebRoot = Path.Combine(contentPath, "wwwroot");
    if (!Directory.Exists(candidateWebRoot))
    {
        Directory.CreateDirectory(candidateWebRoot);
    }
    builder.Environment.WebRootPath = candidateWebRoot;
}

var imagesFolder = Path.Combine(builder.Environment.WebRootPath, "images");
if (!Directory.Exists(imagesFolder))
{
    Directory.CreateDirectory(imagesFolder);
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// تفعيل CORS لتمكين تطبيق فلاتر من التواصل مع الـ API بدون حظر
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// تسجيل خدمات طبقات Clean Architecture
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// إنشاء قاعدة البيانات والجداول والبيانات الأولية تلقائياً
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        if (db.Database.IsSqlServer())
        {
            // 1. تحديث أعمدة جدول القاعات Halls تلقائياً في حال وجود قاعدة بيانات قديمة
            db.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Halls') AND name = 'IsBooked')
                    ALTER TABLE Halls ADD IsBooked BIT NOT NULL DEFAULT 0;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Halls') AND name = 'BookedBy')
                    ALTER TABLE Halls ADD BookedBy NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Halls') AND name = 'BookingPurpose')
                    ALTER TABLE Halls ADD BookingPurpose NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Halls') AND name = 'BookingDate')
                    ALTER TABLE Halls ADD BookingDate NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Halls') AND name = 'ActiveBookingId')
                    ALTER TABLE Halls ADD ActiveBookingId INT NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Halls') AND name = 'Building')
                    ALTER TABLE Halls ADD Building NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Halls') AND name = 'Capacity')
                    ALTER TABLE Halls ADD Capacity INT NOT NULL DEFAULT 50;
            ");

            // 2. تحديث أعمدة جدول الحجوزات Bookings تلقائياً في حال وجود قاعدة بيانات قديمة
            db.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'HallId')
                    ALTER TABLE Bookings ADD HallId INT NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'Capacity')
                    ALTER TABLE Bookings ADD Capacity INT NOT NULL DEFAULT 50;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'HallImage')
                    ALTER TABLE Bookings ADD HallImage NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'Building')
                    ALTER TABLE Bookings ADD Building NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'UserName')
                    ALTER TABLE Bookings ADD UserName NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'Purpose')
                    ALTER TABLE Bookings ADD Purpose NVARCHAR(MAX) NULL;

                -- معالجة أي قيم فارغة NULL لضمان عمل الاستعلامات بسلاسة تامة
                UPDATE Halls SET 
                    BookedBy = COALESCE(BookedBy, ''),
                    BookingPurpose = COALESCE(BookingPurpose, ''),
                    BookingDate = COALESCE(BookingDate, ''),
                    Building = COALESCE(Building, 'مبنى A'),
                    Details = COALESCE(Details, ''),
                    Image = COALESCE(Image, 'assets/hall_101.png');

                UPDATE Bookings SET 
                    HallImage = COALESCE(HallImage, 'assets/hall_101.png'),
                    Building = COALESCE(Building, 'مبنى A'),
                    UserName = COALESCE(UserName, 'حفصة منصور'),
                    Purpose = COALESCE(Purpose, 'حجز قاعة'),
                    Notes = COALESCE(Notes, '');
            ");

            // 3. ضمان وجود مستخدمين افتراضيين لتسجيل الدخول السريع
            db.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@university.edu')
                BEGIN
                    INSERT INTO Users (FullName, Email, PasswordHash, Role, CreatedAt)
                    VALUES (N'المدير المسؤول', 'admin@university.edu', '123456', 'Admin', GETUTCDATE());
                END
                IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'student@university.edu')
                BEGIN
                    INSERT INTO Users (FullName, Email, PasswordHash, Role, CreatedAt)
                    VALUES (N'أحمد محمد', 'student@university.edu', '123456', 'Student', GETUTCDATE());
                END
            ");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Database Initialization Error]: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// تم إيقاف UseHttpsRedirection لتمكين فلاتر والمتصفح من الاتصال المباشر بـ HTTP بدون حظر شهادات
// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// تفعيل سياسة CORS قبل الصلاحيات
app.UseCors("AllowAll");

app.UseAuthorization();

// تعيين التوجيه الافتراضي ومسارات الـ APIs
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=login}/{id?}");

app.Run();
