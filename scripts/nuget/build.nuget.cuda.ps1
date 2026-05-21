param(
    [string[]]$Build = @()   # e.g. -Build Combined   or   -Build Turing,Ampere
)

$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$ScriptDir = $PSScriptRoot
$RepoRoot = (Resolve-Path (Join-Path $ScriptDir "../../")).Path.TrimEnd('\').TrimEnd('/')

Write-Host ">>> Repo root resolved to: $RepoRoot" -ForegroundColor DarkGray

$AllTargets = @("Turing", "Ampere", "Ada", "Blackwell", "Combined")

if ($Build.Count -gt 0) {
    $unrecognised = $Build | Where-Object { $AllTargets -notcontains $_ }
    if ($unrecognised) {
        throw "Unrecognised target(s): $($unrecognised -join ', '). Valid values: $($AllTargets -join ', ')"
    }
    $Targets = $Build
    Write-Host ">>> Packing only: $($Targets -join ', ')" -ForegroundColor Yellow
} else {
    $Targets = $AllTargets
    Write-Host ">>> Packing all targets" -ForegroundColor Yellow
}

$OutputFolder = "$RepoRoot/nuget/bin"
if (-not (Test-Path $OutputFolder)) { New-Item -ItemType Directory -Path $OutputFolder | Out-Null }

# ---------------------------------------------------------------------------
# Windows runtime packages
# ---------------------------------------------------------------------------
$WinCsproj = "$RepoRoot/nuget/OpenCvSharp4.Cuda.runtime.win.csproj"

foreach ($Arch in $Targets) {
    Write-Host "Packing NuGet package for Windows $Arch..." -ForegroundColor Cyan

    dotnet pack $WinCsproj -c Release -p:CudaArch=$Arch -o $OutputFolder

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully created package for Windows $Arch" -ForegroundColor Green
    } else {
        Write-Host "Failed to pack Windows $Arch" -ForegroundColor Red
    }
}

# NVIDIA Redist is not arch-specific, always pack it
Write-Host "Packing NuGet package Nvidia Redist..." -ForegroundColor Cyan
dotnet pack "$RepoRoot/nuget/OpenCvSharp4.Cuda.NvidiaRedist.win.csproj" -c Release -o $OutputFolder

# ---------------------------------------------------------------------------
# Linux runtime packages
# ---------------------------------------------------------------------------
$LinuxCsproj = "$RepoRoot/nuget/OpenCvSharp4.Cuda.runtime.linux-x64.csproj"

foreach ($Arch in $Targets) {
    Write-Host "Packing NuGet package for Linux $Arch..." -ForegroundColor Cyan

    dotnet pack $LinuxCsproj -c Release -p:CudaArch=$Arch -o $OutputFolder

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully created package for Linux $Arch" -ForegroundColor Green
    } else {
        Write-Host "Failed to pack Linux $Arch" -ForegroundColor Red
    }
}

# ---------------------------------------------------------------------------
# Managed library (not arch-specific, always pack it)
# ---------------------------------------------------------------------------
Write-Host "Packing NuGet OpenCvSharp.Cuda..." -ForegroundColor Cyan
dotnet pack "$RepoRoot/src/OpenCvSharp.Cuda/OpenCvSharp.Cuda.csproj" -c Release -o $OutputFolder

Write-Host "`nAll packing complete!" -ForegroundColor Yellow

# ---------------------------------------------------------------------------
#  NVIDIA Redist (pack sub-packages first, then the meta-package)
# ---------------------------------------------------------------------------
foreach ($pkg in @("NvidiaRedist.win.Core", "NvidiaRedist.win.Compute", "NvidiaRedist.win")) {
    Write-Host "Packing NuGet package $pkg..." -ForegroundColor Cyan
    dotnet pack "$RepoRoot/nuget/OpenCvSharp4.Cuda.$pkg.csproj" -c Release -o $OutputFolder
}