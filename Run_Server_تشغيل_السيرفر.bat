@echo off
chcp 65001 >nul
sqllocaldb create MSSQLLocalDB >nul 2>&1
sqllocaldb start MSSQLLocalDB >nul 2>&1
title تشغيل سيرفر حجز القاعات (ASP.NET MVC)
color 0B
echo ======================================================================
echo    سيرفر نظام حجز القاعات الجامعية (ASP.NET Core MVC - Clean Architecture)
echo ======================================================================
echo.
echo جاري تشغيل السيرفر...
echo الرابط في المتصفح: http://localhost:5051/Account/Login
echo رابط الـ API: http://localhost:5051/api/Halls
echo رابط الحجوزات API: http://localhost:5051/api/Bookings
echo.
cd /d "%~dp0\unviersityBooking.Web"
start http://localhost:5051/Account/Login
dotnet run --launch-profile "http"
pause
