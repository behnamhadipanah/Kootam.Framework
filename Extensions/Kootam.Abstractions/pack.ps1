$Solution = "Kootam.Extensions.Abstractions.sln"
$OutputDir = "E:\BackupWork\LocalNugets"


Write-Host "Using output path: $OutputDir"

if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

Write-Host "Cleaning solution..."
dotnet clean $Solution -c Release

Write-Host "Restoring packages..."
dotnet restore $Solution

Write-Host "Building solution..."
dotnet build $Solution -c Release


Write-Host "Packing all projects..."
dotnet pack $Solution -c Release -o $OutputDir


Write-Host "`nDone!"
Write-Host "NuGet packages saved to: $OutputDir"