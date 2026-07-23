$RepoRoot = "E:\BackupWork\VCS\Github\Kootam.Framework"
$PriorityProject = "E:\BackupWork\VCS\Github\Kootam.Framework\Extensions\Kootam.Abstractions"
$OutputDir = "E:\BackupWork\LocalNugets"

if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
}

Set-Location $RepoRoot

Write-Host "==============================="
Write-Host "Updating repository..."
Write-Host "==============================="

git checkout main
if ($LASTEXITCODE -ne 0) { throw "Checkout failed." }

git pull origin main
if ($LASTEXITCODE -ne 0) { throw "Git pull failed." }

#######################################################
# Pack Kootam.Abstractions FIRST
#######################################################

Write-Host ""
Write-Host "==============================="
Write-Host "Packing Kootam.Abstractions..."
Write-Host "==============================="

Push-Location $PriorityProject

$proj = Get-ChildItem -Filter *.csproj | Select-Object -First 1

dotnet clean $proj.FullName -c Release
dotnet restore $proj.FullName
dotnet build $proj.FullName -c Release
dotnet pack $proj.FullName -c Release -o $OutputDir

Pop-Location

#######################################################
# Pack all solutions
#######################################################

$solutions = Get-ChildItem `
    -Path $RepoRoot `
    -Recurse `
    -Include *.sln,*.slnx |
    Sort-Object FullName

foreach ($solution in $solutions)
{
    Write-Host ""
    Write-Host "========================================"
    Write-Host "Processing $($solution.Name)"
    Write-Host "========================================"

    try
    {
        dotnet clean $solution.FullName -c Release
        if ($LASTEXITCODE -ne 0) { throw "Clean failed." }

        dotnet restore $solution.FullName
        if ($LASTEXITCODE -ne 0) { throw "Restore failed." }

        dotnet build $solution.FullName -c Release
        if ($LASTEXITCODE -ne 0) { throw "Build failed." }

        dotnet pack $solution.FullName -c Release -o $OutputDir
        if ($LASTEXITCODE -ne 0) { throw "Pack failed." }

        Write-Host "✔ $($solution.Name) completed."
    }
    catch
    {
        Write-Warning "$($solution.Name) failed."
        Write-Warning $_
    }
}

Write-Host ""
Write-Host "========================================"
Write-Host "Done!"
Write-Host "Packages saved to:"
Write-Host $OutputDir
Write-Host "========================================"