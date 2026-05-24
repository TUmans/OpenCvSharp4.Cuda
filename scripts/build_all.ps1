<#
.SYNOPSIS
    Master build script located in /scripts/build_all.ps1 to orchestrate the full CUDA pipeline.
#>
param (
    [Parameter(Mandatory=$false)]
    [ValidateSet("Windows", "Linux", "All")]
    [string]$TargetOS = "Windows",

    [Parameter(Mandatory=$false)]
    [switch]$SkipOpenCV,

    [Parameter(Mandatory=$false)]
    [switch]$SkipExtern,

    [Parameter(Mandatory=$false)]
    [switch]$SkipTests,

    [Parameter(Mandatory=$false)]
    [switch]$CreatePackage
)

$ErrorActionPreference = "Stop"

# Set up directory references
$ScriptDir = $PSScriptRoot
# Calculated as requested: moves up from /scripts/ to the repo root
$RepoRoot = (Resolve-Path (Join-Path $ScriptDir "../../")).Path.TrimEnd('\').TrimEnd('/')

# Define paths to sub-folders
$OpenCVBuildDir = Join-Path $ScriptDir "build-opencv"
$ExternBuildDir = Join-Path $ScriptDir "build-opencvsharp"
$NugetDir       = Join-Path $ScriptDir "nuget"
$TestDir        = Join-Path $ScriptDir "test"

# Helper function to run scripts in specific directories
function Run-SubScript {
    param(
        [string]$Directory,
        [string]$FileName
    )
    $ScriptPath = Join-Path $Directory $FileName
    
    Write-Host "`n>>> EXECUTING: $FileName" -ForegroundColor Cyan
    Write-Host "    In: $Directory" -ForegroundColor Gray

    if (Test-Path $ScriptPath) {
        # Change location to the script's own directory before running to ensure internal paths work
        Push-Location $Directory
        try {
            & $ScriptPath
            if ($LASTEXITCODE -ne 0) { 
                throw "Script $FileName failed with exit code $LASTEXITCODE" 
            }
        }
        finally {
            Pop-Location
        }
    } else {
        throw "Script not found at: $ScriptPath"
    }
}

try {
    Write-Host "Repo Root: $RepoRoot" -ForegroundColor Magenta

    # ---------------------------------------------------------
    # 1. WINDOWS PIPELINE
    # ---------------------------------------------------------
    if ($TargetOS -eq "Windows" -or $TargetOS -eq "All") {
        Write-Host "`n=== [ WINDOWS CUDA BUILD PIPELINE ] ===" -ForegroundColor Yellow
        
        if (-not $SkipOpenCV) {
            Run-SubScript $OpenCVBuildDir "build_opencv_windows.cuda.multi.ps1"
        }

        if (-not $SkipExtern) {
            Run-SubScript $ExternBuildDir "build_opencvsharpextern.windows.cuda.multi.ps1"
        }

        if (-not $SkipTests) {
            Run-SubScript $TestDir "test_all_windows.ps1"
        }
    }

    # ---------------------------------------------------------
    # 2. LINUX PIPELINE
    # ---------------------------------------------------------
    if ($TargetOS -eq "Linux" -or $TargetOS -eq "All") {
        Write-Host "`n=== [ LINUX CUDA BUILD PIPELINE ] ===" -ForegroundColor Yellow

        if (-not $SkipOpenCV) {
            Run-SubScript $OpenCVBuildDir "build_opencv_linux.cuda.multi.ps1"
        }

        if (-not $SkipExtern) {
            Run-SubScript $ExternBuildDir "build_opencvsharpextern.linux.cuda.multi.ps1"
        }

        if (-not $SkipTests) {
            Run-SubScript $TestDir "test_all_linux.ps1"
        }
    }

    # ---------------------------------------------------------
    # 3. PACKAGING (NuGet)
    # ---------------------------------------------------------
    if ($CreatePackage) {
        Write-Host "`n=== [ CREATING NUGET PACKAGE ] ===" -ForegroundColor Yellow
        Run-SubScript $NugetDir "build.nuget.cuda.ps1"
    }

    Write-Host "`nDONE: Full build orchestration finished successfully!" -ForegroundColor Green

}
catch {
    Write-Host "`nORCHESTRATION FAILED: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}