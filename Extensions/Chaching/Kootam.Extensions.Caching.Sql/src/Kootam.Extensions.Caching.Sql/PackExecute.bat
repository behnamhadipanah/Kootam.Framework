@echo off
title Running NuGet Pack Script
echo Starting PowerShell script...

powershell -NoProfile -ExecutionPolicy Bypass -File ".\pack.ps1"

echo.
echo Process finished.
pause