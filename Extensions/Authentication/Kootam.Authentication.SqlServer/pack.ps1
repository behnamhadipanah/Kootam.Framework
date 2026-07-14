# Find solution file automatically
$Solution = Get-ChildItem -Path . -Filter *.slnx -Recurse -ErrorAction SilentlyContinue |
            Select-Object -First 1 -ExpandProperty FullName

if (-not $Solution) {
    Write-Host "No .sln file found in this directory!" -ForegroundColor Red
    exit 1
}

Write-Host "Found solution: $Solution"

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

Write-Host "Searching for class library projects..."

# Find all csproj files in repo
$csprojPaths = Get-ChildItem -Path . -Recurse -Filter *.csproj | Select-Object -ExpandProperty FullName

$classlibProjects = @()

foreach ($projPath in $csprojPaths) {
    $xml = [xml](Get-Content $projPath)
    $propertyGroups = $xml.Project.PropertyGroup
    $isClassLib = $false

    foreach ($pg in $propertyGroups) {
        # If OutputType is Library OR empty (SDK-style classlib)
        if ($pg.OutputType -eq "Library" -or $pg.OutputType -eq $null) {
            $isClassLib = $true
        }
    }

    if ($isClassLib) {
        $classlibProjects += $projPath
    }
}

Write-Host "Found $($classlibProjects.Count) class library projects."

Write-Host "Packing only class library projects..."

foreach ($proj in $classlibProjects) {
    Write-Host "Packing: $proj"
    dotnet pack $proj -c Release -o $OutputDir
}

Write-Host "`nDone!"
Write-Host "NuGet packages saved to: $OutputDir"
