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



$Repo = $RepoRoot

if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
}

Set-Location $Repo

Step "Switching to main branch..."
git checkout Main

Step "Pulling latest changes..."
git pull origin Main

Step "Finding solutions..."

$solutions = Get-ChildItem $Repo -Recurse -Include *.sln, *.slnx |
Where-Object {
    $_.FullName -notlike "$Repo\Extensions\*"
}

foreach ($solution in $solutions) {
    Step "Processing solution: $($solution.Name)"

    Write-Host "Cleaning..."
    dotnet clean $solution.FullName -c Release

    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Clean failed. Skipping..."
        continue
    }

    Write-Host "Restoring packages..."
    dotnet restore $solution.FullName -c Release --configfile $TempNuGetConfig

    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Restore failed. Skipping..."
        continue
    }

    Write-Host "Building..."
    dotnet build $solution.FullName -c Release --no-restore
    
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Build failed. Skipping..."
        continue
    }

    Write-Host "Packing..."
    dotnet pack $solution.FullName -c Release --no-build  -o $OutputDir --configfile $TempNuGetConfig

    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Pack failed. Skipping..."
        continue
    }

    Write-Host "$($solution.Name) completed successfully." -ForegroundColor Green
}

Step "Finished."

Write-Host "Packages saved to:"
Write-Host $OutputDir -ForegroundColor Green