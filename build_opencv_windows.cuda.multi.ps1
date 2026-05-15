param(
    [int]$Jobs = 8,
    [ValidateSet("Release", "Debug")]
    [string]$Config = "Release"
)

$ErrorActionPreference = "Stop"
$RepoRoot = $PSScriptRoot
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

function Require-Command($name) {
    if (-not (Get-Command $name -ErrorAction SilentlyContinue)) {
        throw "Required command '$name' not found in PATH."
    }
}

if (-not (Get-Command cmake -ErrorAction SilentlyContinue)) {
    Write-Error @"
cmake was not found in PATH.
To fix this, choose one of the following:
  1. Install CMake via winget: winget install Kitware.CMake
  2. Install CMake 3.20+ from https://cmake.org/download/
"@
    exit 1
}
Require-Command git

# ---------------------------------------------------------------------------
# Verify submodules
# ---------------------------------------------------------------------------
if (-not (Test-Path "$RepoRoot/extern/OpenCvSharp/opencv/CMakeLists.txt")) {
    throw "opencv submodule not found. Run: git submodule update --init --recursive"
}

$OpenCvVersion = (git -C "$RepoRoot/extern/OpenCvSharp/opencv" describe --tags --exact-match 2>$null)
if (-not $OpenCvVersion) { $OpenCvVersion = (git -C "$RepoRoot/extern/OpenCvSharp/opencv" rev-parse --short HEAD) }
Write-Host "OpenCV version: $OpenCvVersion"

# ---------------------------------------------------------------------------
# Detect Visual Studio generator via vswhere
# ---------------------------------------------------------------------------
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vswhere)) { throw "vswhere.exe not found." }

$vsInstallVersion = & $vswhere -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationVersion 2>$null
$vsDisplayName    = & $vswhere -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property displayName    2>$null
$vsInstallPath    = & $vswhere -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath   2>$null

if (-not $vsInstallVersion) { throw "No Visual Studio installation with C++ tools found." }

$vsMajor = [int]($vsInstallVersion.Split('.')[0])
$generatorMap = @{
    17 = "Visual Studio 17 2022"
    18 = "Visual Studio 18 2026"
}
$vsGenerator = $generatorMap[$vsMajor]
if (-not $vsGenerator) { throw "Unsupported Visual Studio major version: $vsMajor" }
Write-Host "Using generator: $vsGenerator ($vsDisplayName)"

# ---------------------------------------------------------------------------
# Define targets (Individual + ALL)
# ---------------------------------------------------------------------------
$Targets = @(
    @{ Name = "Turing";    Arch = "7.5";             Ptx = "7.5" }
    @{ Name = "Ampere";    Arch = "8.6";             Ptx = "8.6" }
    @{ Name = "Ada";       Arch = "8.9";             Ptx = "8.9" }
    @{ Name = "Blackwell"; Arch = "10.0";            Ptx = "10.0" }
    @{ Name = "Combined"; Arch = "7.5,8.6,8.9,10.0"; Ptx = "10.0" } 
)

# ---------------------------------------------------------------------------
# Configure Paths
# ---------------------------------------------------------------------------
$installDir   = "$RepoRoot/opencv_artifacts"
$buildDir     = "$RepoRoot/extern/OpenCvSharp/opencv/build-vs$vsMajor"
$ExternSource = "$RepoRoot/src/OpenCvSharpExtern"
$FinalDist    = "$RepoRoot/src/build"

# ---------------------------------------------------------------------------
# Resolve Tesseract prefix via vcpkg
# ---------------------------------------------------------------------------
$vcpkgRoot = $env:VCPKG_INSTALLATION_ROOT
if (-not $vcpkgRoot) {
    $vcpkgCmd = Get-Command vcpkg -ErrorAction SilentlyContinue
    if ($vcpkgCmd) { $vcpkgRoot = Split-Path $vcpkgCmd.Source }
}
if (-not $vcpkgRoot) { throw "vcpkg was not found." }

$vcpkgToolchain = "$vcpkgRoot/scripts/buildsystems/vcpkg.cmake"
$vcpkgInstalledDir = "$RepoRoot/vcpkg_installed"

# ---------------------------------------------------------------------------
# Build Loop
# ---------------------------------------------------------------------------
foreach ($T in $Targets) {
    Write-Host "`n=======================================================" -ForegroundColor Magenta
    Write-Host " STARTING BUILD: $($T.Name) (sm_$($T.Arch))" -ForegroundColor Magenta
    Write-Host "=======================================================" -ForegroundColor Magenta

    $ArchLabel   = $T.Name
    $BuildDir_CV = "$buildDir/$ArchLabel"
    $BuildDir_Ex = "$RepoRoot/src/build_temp/$ArchLabel"
    $InstallDirArch  = "$installDir/$ArchLabel"

    # --- STEP 1: CLEAN PREVIOUS ATTEMPTS ---
    if (Test-Path $BuildDir_CV) { Remove-Item -Recurse -Force $BuildDir_CV }
    if (Test-Path $InstallDirArch)  { Remove-Item -Recurse -Force $InstallDirArch }

    # --- STEP 2: BUILD OPENCV (ARCH SPECIFIC) ---
    Write-Host ">>> Configuring OpenCV for $ArchLabel..." -ForegroundColor Cyan

    cmake `
        -C "$RepoRoot/cmake/opencv_build_options_cuda.cmake" `
        -S "$RepoRoot/extern/OpenCvSharp/opencv" `
        -B $BuildDir_CV `
        -G "$vsGenerator" -A x64 -T v143 `
        -D "CMAKE_GENERATOR_INSTANCE=$vsInstallPath" `
        -D "CMAKE_TOOLCHAIN_FILE=$vcpkgToolchain" `
        -D "VCPKG_TARGET_TRIPLET=x64-windows-static" `
        -D "VCPKG_INSTALLED_DIR=$vcpkgInstalledDir" `
        -D "VCPKG_OVERLAY_TRIPLETS=$RepoRoot/extern/OpenCvSharp/cmake/triplets" `
        -D "OPENCV_EXTRA_MODULES_PATH=$RepoRoot/extern/OpenCvSharp/opencv_contrib/modules" `
        -D "CMAKE_INSTALL_PREFIX=$InstallDirArch" `
        -D "CUDA_ARCH_BIN=$($T.Arch)" `
        -D "CUDA_ARCH_PTX=$($T.Ptx)"

    Write-Host ">>> Compiling OpenCV (this may take a while)..." -ForegroundColor Gray
    cmake --build $BuildDir_CV --config Release -j $Jobs
    cmake --install $BuildDir_CV --config Release

   
}

Write-Host "`nAll Builds Complete! Check the '$FinalDist' folder." -ForegroundColor Yellow