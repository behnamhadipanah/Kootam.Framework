$ErrorActionPreference = "Stop"

function Step($message)
{
    Write-Host ""
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host $message -ForegroundColor Yellow
    Write-Host "==================================================" -ForegroundColor Cyan
}

$Project = "E:\BackupWork\VCS\Github\Kootam.Framework\Extensions\Kootam.Abstractions"
$Solution = "Kootam.Abstractions.sln"

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

try {
    git pull origin Main
}
catch {
    Write-Warning "git pull failed. Continuing with local code."
}

if (!(Test-Path $Solution)) {
    throw "Solution not found: $Solution"
}

Write-Host "Solution : $Solution"

Step "Cleaning solution..."

dotnet clean $Solution -c Release

Step "Restoring NuGet packages..."

dotnet restore $Solution

Step "Building solution (Release)..."

dotnet build $Solution -c Release

Step "Creating NuGet packages..."

Write-Host "Output directory: $OutputDir" -ForegroundColor Cyan

dotnet pack $Solution -c Release -o $OutputDir

Step "Completed successfully."

Write-Host "NuGet packages created in:"
Write-Host $OutputDir -ForegroundColor Green
