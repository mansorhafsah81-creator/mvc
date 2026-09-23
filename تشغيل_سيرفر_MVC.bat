@echo off
chcp 65001 >nul
sqllocaldb create MSSQLLocalDB >nul 2>&1
sqllocaldb start MSSQLLocalDB >nul 2>&1
title تشغيل سيرفر نظام حجز القاعات الجامعية (ASP.NET MVC)
color 0B
echo ======================================================================
echo    تشغيل سيرفر نظام حجز القاعات الجامعية (ASP.NET MVC + Clean Architecture)
echo ======================================================================
echo.
echo جاري بدء تشغيل السيرفر وقاعدة البيانات...
echo.
echo الروابط المتاحة فور التشغيل:
echo 👉 https://localhost:7120
echo 👉 http://localhost:5051
echo 👉 واجهة القاعات: http://localhost:5051/Account/Halls
echo 👉 واجهة الـ API: http://localhost:5051/api/Halls
echo.
cd /d "%~dp0\unviersityBooking.Web"
start http://localhost:5051/Account/Login
dotnet run --launch-profile "https"
pause
