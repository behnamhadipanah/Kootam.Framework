$ErrorActionPreference = "Stop"

function Step($message)
{
    Write-Host ""
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host $message -ForegroundColor Yellow
    Write-Host "==================================================" -ForegroundColor Cyan
}

$Project = "E:\BackupWork\VCS\Github\Kootam.Framework\Extensions\Kootam.Abstractions"

# ریشه Repository را از روی مسیر پروژه پیدا می‌کند
$RepositoryRoot = Split-Path (Split-Path $Project -Parent) -Parent

# خروجی همیشه داخل پوشه nugets ریشه Repository
$OutputDir = Join-Path $RepositoryRoot "nugets"

Step "Checking output directory..."

if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
    Write-Host "Output directory created."
}
else {
    Write-Host "Output directory already exists."
}

Step "Changing directory to project..."

Set-Location $Project

Step "Switching to main branch..."

git checkout Main

Step "Pulling latest changes from remote..."

git pull origin Main

Step "Finding project file..."

$csproj = Get-ChildItem *.csproj | Select-Object -First 1

Write-Host "Project : $($csproj.Name)"

Step "Cleaning project..."

dotnet clean $csproj.FullName -c Release

Step "Restoring NuGet packages..."

dotnet restore $csproj.FullName

Step "Building project (Release)..."

dotnet build $csproj.FullName -c Release

Step "Creating NuGet package..."

Write-Host "Output directory: $OutputDir" -ForegroundColor Cyan

dotnet pack $csproj.FullName -c Release -o $OutputDir

Step "Completed successfully."

Write-Host "NuGet package created in:"
Write-Host $OutputDir -ForegroundColor Green