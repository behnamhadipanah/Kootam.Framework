@echo off
title Kootam NuGet Pack Manager

:MENU
cls
echo ============================================
echo           Kootam NuGet Pack Manager
echo ============================================
echo.
echo   1. Update nugets packages
echo   2. Pack Kootam.Abstractions
echo   3. Pack Extensions
echo   4. Pack Kootam.Framework
echo   5. Exit
echo.
set /p choice=Select an option [1-5]: 

if "%choice%"=="1" goto UpdateNugets
if "%choice%"=="2" goto PackAbstractions
if "%choice%"=="3" goto PackExtensions
if "%choice%"=="4" goto PackFramework
if "%choice%"=="5" goto Exit

echo.
echo Invalid option.
pause
goto MENU


:UpdateNugets
cls
echo Running update-nugets.ps1
echo.

powershell -NoProfile -ExecutionPolicy Bypass -File ".\update-nugets.ps1"

echo.
echo Operation completed.
pause
goto MENU

:PackAbstractions
cls
echo Running Pack-Kootam.Abstractions.ps1...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -File ".\Pack-Kootam.Abstractions.ps1"

echo.
echo Operation completed.
pause
goto MENU


:PackExtensions
cls
echo Running Pack-Extensions.ps1...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -File ".\Pack-Extensions.ps1"

echo.
echo Operation completed.
pause
goto MENU


:PackFramework
cls
echo Running Pack-Kootam.Framework.ps1...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -File ".\Pack-Kootam.Framework.ps1"

echo.
echo Operation completed.
pause
goto MENU


:Exit
echo.
echo Goodbye!
timeout /t 1 >nul
exit