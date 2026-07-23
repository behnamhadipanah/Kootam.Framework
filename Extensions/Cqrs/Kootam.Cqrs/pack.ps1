$Solution = "Kootam.Extensions.Cqrs.sln"
$OutputDir = "E:\BackupWork\LocalNugets"


Write-Host "Using output path: $OutputDir"

if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

Write-Host "Fetching latest changes..."
git fetch origin

Write-Host "Switching to main..."
git checkout Main

Write-Host "Pulling latest main..."
git pull origin Main


Write-Host "Cleaning solution..."
dotnet clean $Solution -c Release

Write-Host "Restoring packages..."
dotnet restore $Solution

Write-Host "Building solution..."
dotnet build $Solution -c Release

Write-Host "Looking for class library projects..."

$csprojPaths = Get-ChildItem -Path . -Recurse -Filter *.csproj | Select-Object -ExpandProperty FullName

$classlibProjects = @()

foreach ($projPath in $csprojPaths) {
    $xml = [xml](Get-Content $projPath)
    $outputTypeNode = $xml.Project.PropertyGroup.OutputType
    $propertyGroups = $xml.Project.PropertyGroup
    $isClassLib = $false
    foreach ($pg in $propertyGroups) {
        if ($pg.OutputType -eq "Library" -or $pg.OutputType -eq $null) {
            $isClassLib = $true
        }
    }
    if ($isClassLib) {
        $classlibProjects += $projPath
    }
}

Write-Host "Packing class libraries only..."

foreach ($proj in $classlibProjects) {
    Write-Host "Packing: $proj"
    dotnet pack $proj -c Release -o $OutputDir
}

Write-Host "`nDone!"
Write-Host "NuGet packages saved to: $OutputDir"