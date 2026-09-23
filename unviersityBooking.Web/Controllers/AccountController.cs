using Microsoft.AspNetCore.Mvc;
using universityBooking.Application.DTOs;
using universityBooking.Application.Interfaces;

namespace universityBooking.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHallService _hallService;
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(
            IHallService hallService, 
            IBookingService bookingService, 
            IUserService userService,
            IWebHostEnvironment webHostEnvironment)
        {
            _hallService = hallService;
            _bookingService = bookingService;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string? email, string? password)
        {
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                var user = await _userService.LoginAsync(email.Trim(), password.Trim());
                if (user != null)
                {
                    TempData["CurrentUserName"] = user.FullName;
                    return RedirectToAction("Dashboard", "Account");
                }
            }

            // تسجيل دخول فوري بالاسم المكتوب أو الافتراضي
            string displayName = !string.IsNullOrWhiteSpace(email) 
                ? (email.Contains("@") ? email.Split('@')[0] : email) 
                : "أحمد محمد";
            
            TempData["CurrentUserName"] = displayName;
            return RedirectToAction("Dashboard", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password, string? confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "الرجاء إدخال البريد الإلكتروني وكلمة المرور";
                return View();
            }

            if (!string.IsNullOrWhiteSpace(confirmPassword) && password != confirmPassword)
            {
                ViewBag.ErrorMessage = "كلمات المرور غير متطابقة";
                return View();
            }

            try
            {
                var user = await _userService.RegisterAsync(
                    string.IsNullOrWhiteSpace(fullName) ? "حفصة منصور" : fullName.Trim(),
                    email.Trim(),
                    password.Trim());

                TempData["CurrentUserName"] = user?.FullName ?? fullName ?? "حفصة منصور";
                return RedirectToAction("Dashboard", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.BookingsCount = await _bookingService.GetBookingsCountAsync();
            ViewBag.HallsCount = await _hallService.GetHallsCountAsync();
            var allBookings = await _bookingService.GetAllBookingsAsync();
            return View(allBookings.Take(5));
        }

        [HttpGet]
        public async Task<IActionResult> Halls()
        {
            var hallsList = await _hallService.GetAllHallsAsync();
            return View(hallsList);
        }

        // ==========================================
        // إضافة قاعة جديدة بالصورة والتفاصيل
        // ==========================================
        [HttpGet]
        public IActionResult AddHall()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddHall(
            string name, 
            string building, 
            int capacity, 
            string details, 
            string? imagePreset, 
            string? imageBase64,
            IFormFile? imageFile)
        {
            string finalImage = "assets/hall_101.png";

            if (!string.IsNullOrWhiteSpace(imagePreset))
            {
                if (imagePreset.Contains("1.jpg"))
                {
                    finalImage = "images/1.jpg";
                }
                else
                {
                    finalImage = imagePreset.Trim();
                }
            }

            // جمع كافة المسارات المحتملة لـ wwwroot/images لضمان الحفظ في المجلد النشط ومجلد المشروع
            var targetFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(_webHostEnvironment.WebRootPath))
            {
                targetFolders.Add(Path.Combine(_webHostEnvironment.WebRootPath, "images"));
            }

            string contentRoot = _webHostEnvironment.ContentRootPath ?? Directory.GetCurrentDirectory();
            targetFolders.Add(Path.Combine(contentRoot, "wwwroot", "images"));
            targetFolders.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "images"));
            targetFolders.Add(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"));

            // 1. معالجة ملف الصورة المرفوع
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    string ext = Path.GetExtension(imageFile.FileName);
                    if (string.IsNullOrEmpty(ext)) ext = ".jpg";
                    string uniqueFileName = $"{Guid.NewGuid():N}_{Path.GetFileNameWithoutExtension(imageFile.FileName)}{ext}";

                    bool savedAny = false;
                    foreach (var folder in targetFolders)
                    {
                        try
                        {
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                            string filePath = Path.Combine(folder, uniqueFileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                            {
                                imageFile.CopyTo(fileStream);
                            }
                            savedAny = true;
                        }
                        catch
                        {
                            // المحاولة في المجلد التالي
                        }
                    }

                    if (savedAny)
                    {
                        finalImage = "images/" + uniqueFileName;
                    }
                }
                catch
                {
                    // محاولة استخدام Base64 إذا توفر
                }
            }

            // 2. معالجة Base64 كبديل قوي في حال لم يُحفظ الملف المادي
            if ((finalImage == "assets/hall_101.png" || finalImage == imagePreset) && !string.IsNullOrWhiteSpace(imageBase64) && imageBase64.StartsWith("data:image"))
            {
                try
                {
                    int commaIdx = imageBase64.IndexOf(',');
                    string pureBase64 = commaIdx >= 0 ? imageBase64.Substring(commaIdx + 1) : imageBase64;
                    byte[] fileBytes = Convert.FromBase64String(pureBase64);
                    string uniqueFileName = $"{Guid.NewGuid():N}_upload.jpg";

                    bool savedAny = false;
                    foreach (var folder in targetFolders)
                    {
                        try
                        {
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                            string filePath = Path.Combine(folder, uniqueFileName);
                            System.IO.File.WriteAllBytes(filePath, fileBytes);
                            savedAny = true;
                        }
                        catch { }
                    }

                    if (savedAny)
                    {
                        finalImage = "images/" + uniqueFileName;
                    }
                    else
                    {
                        // تخزين Base64 مباشرة لتعمل حتى بدون نظام ملفات
                        finalImage = imageBase64;
                    }
                }
                catch
                {
                    if (!string.IsNullOrWhiteSpace(imageBase64))
                    {
                        finalImage = imageBase64;
                    }
                }
            }

            var hallDto = new HallDto
            {
                Name = string.IsNullOrWhiteSpace(name) ? "قاعة جديدة" : name.Trim(),
                Building = string.IsNullOrWhiteSpace(building) ? "المبنى الرئيسي" : building.Trim(),
                Capacity = capacity > 0 ? capacity : 50,
                Details = details?.Trim() ?? string.Empty,
                Image = finalImage,
                IsBooked = false
            };

            await _hallService.AddHallAsync(hallDto);
            TempData["SuccessMessage"] = $"تمت إضافة {hallDto.Name} بنجاح";
            return RedirectToAction("Halls");
        }

        // ==========================================
        // حذف قاعة من الـ MVC
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> DeleteHall(int id)
        {
            await _hallService.DeleteHallAsync(id);
            TempData["SuccessMessage"] = "تم حذف القاعة بنجاح";
            return RedirectToAction("Halls");
        }

        [HttpGet]
        public async Task<IActionResult> Bookings()
        {
            var userBookings = await _bookingService.GetAllBookingsAsync();
            return View(userBookings);
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var halls = await _hallService.GetAllHallsAsync();
            ViewBag.Halls = halls;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> BookingHall(int? hallId)
        {
            if (hallId.HasValue)
            {
                var hall = await _hallService.GetHallByIdAsync(hallId.Value);
                if (hall != null)
                {
                    ViewBag.SelectedHall = hall;
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BookingHall(
            string hallNumber, 
            string buildingName, 
            string bookingDate, 
            string purpose, 
            string userName, 
            string department, 
            int attendeesCount, 
            string notes,
            string hallImage)
        {
            var dto = new CreateBookingDto
            {
                HallName = string.IsNullOrWhiteSpace(hallNumber) ? "قاعة 101" : hallNumber,
                Building = string.IsNullOrWhiteSpace(buildingName) ? "مبنى A" : buildingName,
                Date = string.IsNullOrWhiteSpace(bookingDate) ? DateTime.Now.ToString("yyyy/MM/dd") : bookingDate,
                Purpose = string.IsNullOrWhiteSpace(purpose) ? "حجز قاعة" : purpose,
                UserName = string.IsNullOrWhiteSpace(userName) ? "حفصة منصور" : userName,
                Department = string.IsNullOrWhiteSpace(department) ? "علوم الحاسب" : department,
                AttendeesCount = attendeesCount > 0 ? attendeesCount : 40,
                Capacity = attendeesCount > 0 ? attendeesCount : 50,
                HallImage = !string.IsNullOrWhiteSpace(hallImage) ? hallImage : "assets/hall_101.png",
                Notes = notes ?? string.Empty,
                Status = "مؤكد"
            };

            var createdBooking = await _bookingService.CreateBookingAsync(dto);

            TempData["HallNumber"] = createdBooking.HallName;
            TempData["BuildingName"] = createdBooking.Building;
            TempData["BookingDate"] = createdBooking.Date;
            TempData["Purpose"] = createdBooking.Purpose;
            TempData["UserName"] = createdBooking.UserName;
            TempData["Capacity"] = createdBooking.Capacity;
            TempData["HallImage"] = createdBooking.HallImage;
            TempData.Keep();

            return RedirectToAction("Confirmation");
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return RedirectToAction("Bookings");
        }

        [HttpPost]
        public async Task<IActionResult> CancelBookingFromHalls(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return RedirectToAction("Halls");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error()
        {
            return RedirectToAction("Error", "Home");
        }
    }
}
