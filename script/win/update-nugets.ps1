$steps = @(
    @{
        Path = "E:\BackupWork\VCS\Github\Kootam.Framework\Extensions\Kootam.Abstractions"
        Exclude = @()
    },
    @{
        Path = "E:\BackupWork\VCS\Github\Kootam.Framework\Extensions\Authentication\Kootam.Authentication"
        Exclude = @()
    },
    @{
        Path = "E:\BackupWork\VCS\Github\Kootam.Framework\Extensions"
        Exclude = @(
            "E:\BackupWork\VCS\Github\Kootam.Framework\Extensions\Kootam.Abstractions",
            "E:\BackupWork\VCS\Github\Kootam.Framework\Extensions\Authentication\Kootam.Authentication"
        )
    },
    @{
        Path = "E:\BackupWork\VCS\Github\Kootam.Framework"
        Exclude = @(
            "E:\BackupWork\VCS\Github\Kootam.Framework\Extensions"
        )
    }
)

foreach ($step in $steps) {

    Write-Host ""
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "Processing: $($step.Path)" -ForegroundColor Cyan
    Write-Host "==================================================" -ForegroundColor Cyan

    $projects = Get-ChildItem -Path $step.Path -Recurse -Filter *.csproj |
        Where-Object {
            $fullName = $_.FullName
            -not ($step.Exclude | Where-Object {
                $fullName.StartsWith($_, [System.StringComparison]::OrdinalIgnoreCase)
            })
        }

    if ($projects.Count -eq 0) {
        Write-Host "No projects found." -ForegroundColor Yellow
        continue
    }

    foreach ($project in $projects) {

        Write-Host ""
        Write-Host "------------------------------------------" -ForegroundColor DarkGray
        Write-Host "Project: $($project.FullName)" -ForegroundColor Green
        Write-Host "------------------------------------------" -ForegroundColor DarkGray

        Write-Host "Updating packages..."
        dotnet package update --project "$($project.FullName)"

        if ($LASTEXITCODE -ne 0) {
            Write-Host "Package update returned exit code $LASTEXITCODE" -ForegroundColor Yellow
        }

        Write-Host "Restoring..."
        dotnet restore "$($project.FullName)"

        if ($LASTEXITCODE -ne 0) {
            Write-Host "Restore failed." -ForegroundColor Red
            continue
        }

        Write-Host "Building..."
        dotnet build "$($project.FullName)" --no-restore

        if ($LASTEXITCODE -eq 0) {
            Write-Host "Build succeeded." -ForegroundColor Green
        }
        else {
            Write-Host "Build failed." -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "Done." -ForegroundColor Cyan