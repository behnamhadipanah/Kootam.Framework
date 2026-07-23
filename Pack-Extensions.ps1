$ErrorActionPreference = "Stop"

function Step($message) {
    Write-Host ""
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host $message -ForegroundColor Yellow
    Write-Host "==================================================" -ForegroundColor Cyan
}

$RepoRoot = "E:\BackupWork\VCS\Github\Kootam.Framework"
$OutputDir = Join-Path $RepoRoot "nugets"

$TempNuGetConfig = Join-Path $env:TEMP "Kootam.NuGet.Config"

@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$OutputDir" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@ | Set-Content -Path $TempNuGetConfig -Encoding UTF8


$Root = Join-Path $RepoRoot "Extensions"
$Exclude = Join-Path $Root "Kootam.Abstractions"

if (!(Test-Path $OutputDir)) {
    Step "Creating output directory..."
    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
}


Set-Location $Root


Step "Switching to main branch..."
git checkout Main


Step "Pulling latest changes from remote..."
git pull origin Main


Step "Finding solutions..."


$solutions = Get-ChildItem $Root -Recurse -Include *.sln, *.slnx |
Where-Object {
    $_.FullName -notlike "$Exclude*" -and
    $_.FullName -notmatch "\\Sample\\|\.Sample"
}


foreach ($solution in $solutions) {
    Step "Processing solution: $($solution.Name)"
    
    Write-Host "Restoring packages..."
    dotnet restore $solution.FullName --configfile $TempNuGetConfig

    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Restore failed. Skipping $($solution.Name)"
        continue
    }

    Write-Host "Cleaning..."

    dotnet clean $solution.FullName -c Release

    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Clean failed. Skipping $($solution.Name)"
        continue
    }


    



    Write-Host "Building..."

    dotnet build $solution.FullName -c Release  --no-restore

    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Build failed. Skipping $($solution.Name)"
        continue
    }


    Write-Host "Packing..."

    dotnet pack $solution.FullName -c Release --no-build  -o $OutputDir --configfile $TempNuGetConfig

    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Pack failed. Skipping $($solution.Name)"
        continue
    }


    Write-Host "$($solution.Name) completed successfully." -ForegroundColor Green
}


Step "All operations completed."

Write-Host "NuGet packages saved to:"
Write-Host $OutputDir -ForegroundColor Green