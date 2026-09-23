@echo off
echo Restoring packages offline from local ./packages folder...
dotnet restore --source ./packages
dotnet build
echo Build succeeded! You can now open universityBooking.sln and run.
pause
