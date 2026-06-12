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

$CudaSrcDir = "$RepoRoot/extern/cuda/12.8"
$ZipWorkDir = "$OutputFolder/temp"
$BigZip = "$ZipWorkDir/Nvidia.Redist.zip"
$ChunkSize = 240MB



if (Test-Path $ZipWorkDir) 
{ Remove-Item -LiteralPath $ZipWorkDir -Force -Recurse
}
New-Item -ItemType Directory -Path $ZipWorkDir | Out-Null

Write-Host ">>> Creating Master Zip..." -ForegroundColor Cyan
# Uses built-in Windows Zip
Compress-Archive -Path "$CudaSrcDir\*" -DestinationPath $BigZip -CompressionLevel Fastest

Write-Host ">>> Splitting Zip into '$ChunkSize' chunks..." -ForegroundColor Cyan
$Stream = [System.IO.File]::OpenRead($BigZip)
$Buffer = New-Object byte[] $ChunkSize
$PartNum = 1

while ($Read = $Stream.Read($Buffer, 0, $Buffer.Length)) {
    $PartID = $PartNum.ToString("D3") # 001, 002, etc.
    $ChunkPath = "$ZipWorkDir/Nvidia.Redist.zip.$PartID"
    
    $ChunkStream = [System.IO.File]::Create($ChunkPath)
    $ChunkStream.Write($Buffer, 0, $Read)
    $ChunkStream.Close()
    
    $PartNum++
}
$Stream.Close()

$PartCsproj = "$RepoRoot/nuget/OpenCvSharp4.Cuda.NvidiaRedist.win.prt.csproj"
$Chunks = Get-ChildItem "$ZipWorkDir/*.zip.*"

foreach ($File in $Chunks) {
    $Extension = $File.Extension.TrimStart('.') # e.g., "001"
    
    Write-Host "Packing NuGet package for Part $Extension..." -ForegroundColor Cyan
    
    $TempCopyPath = Join-Path "$RepoRoot/nuget" $File.Name
    Copy-Item $File.FullName $TempCopyPath

    # Pass "    zip.001" as the ID
    dotnet pack $PartCsproj -c Release -p:CudaZipPart="zip.$Extension" -p:PartNumber="$Extension" -o $OutputFolder
    
    Remove-Item $TempCopyPath
}


