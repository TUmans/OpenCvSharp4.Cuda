param(
    [string]$ApiKey          # NuGet API Key (Required if -Push is used)
)

$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$ScriptDir = $PSScriptRoot
$RepoRoot = (Resolve-Path (Join-Path $ScriptDir "../../")).Path.TrimEnd('\').TrimEnd('/')
$OutputFolder = "$RepoRoot\nuget\bin"

# ---------------------------------------------------------------------------
#  Push to NuGet.org
# ---------------------------------------------------------------------------

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    throw "API Key is required when pushing. Please provide -ApiKey '<Your-Key>'."
}

Write-Host "`n>>> Pushing packages to nuget.org..." -ForegroundColor Yellow
Write-Host "Looking for packages in: $OutputFolder" -ForegroundColor DarkGray

if (-not (Test-Path $OutputFolder)) {
    throw "The output folder does not exist: $OutputFolder"
}

$NugetFiles = @(Get-ChildItem -Path $OutputFolder -Filter "*.nupkg" -Recurse -File | Where-Object { $_.Name -notlike "*.snupkg" })

if ($NugetFiles.Count -eq 0) {
    Write-Host "No standard .nupkg files found in $OutputFolder to push." -ForegroundColor Red
    
    Write-Host "`nHere is what PowerShell actually sees in that folder:" -ForegroundColor Yellow
    # This helps you debug by showing you exactly what is inside the directory
    Get-ChildItem -Path $OutputFolder -Recurse | Select-Object FullName | Format-Table -AutoSize
} else {
    Write-Host "Found $($NugetFiles.Count) package(s) to push.`n" -ForegroundColor Green

    foreach ($file in $NugetFiles) {
        Write-Host "Pushing $($file.Name)..." -ForegroundColor Cyan
            
        # Using the official v3 NuGet API URL
        dotnet nuget push $file.FullName --api-key $ApiKey --source "https://api.nuget.org/v3/index.json" --skip-duplicate
            
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Successfully pushed $($file.Name)" -ForegroundColor Green
        } else {
            Write-Host "Failed to push $($file.Name) or it was a duplicate." -ForegroundColor DarkYellow
        }
    }
    
    Write-Host "`nPush process complete!" -ForegroundColor Yellow
}