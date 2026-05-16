$Targets = @(
    "Turing",
    "Ampere",
    "Ada",
    "Blackwell",
    "Combined" 
)

$CsprojPath = "nuget/OpenCvSharp4.Cuda.runtime.win.csproj"
$OutputFolder = "nuget/bin/"

if (-not (Test-Path $OutputFolder)) { New-Item -ItemType Directory -Path $OutputFolder }

foreach ($Arch in $Targets) {
    Write-Host "Packing NuGet package for Windows $Arch..." -ForegroundColor Cyan
    
    dotnet pack $CsprojPath `
        -c Release `
        -p:CudaArch=$Arch `
        -o $OutputFolder

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully created package for Windows $Arch" -ForegroundColor Green
    } else {
        Write-Host "Failed to pack Windows $Arch" -ForegroundColor Red
    }
}

Write-Host "Packing NuGet package Nvidia Redist" -ForegroundColor Cyan
dotnet pack "nuget/OpenCvSharp4.Cuda.NvidiaRedist.win.csproj" -c Release -o $OutputFolder


$CsprojPath = "nuget/OpenCvSharp4.Cuda.runtime.linux-x64.csproj"

if (!(Test-Path $OutputFolder)) { New-Item -ItemType Directory -Path $OutputFolder }

foreach ($Arch in $Targets) {
    Write-Host "Packing NuGet package for Linux $Arch..." -ForegroundColor Cyan
    
    dotnet pack $CsprojPath `
        -c Release `
        -p:CudaArch=$Arch `
        -o $OutputFolder

  if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully created package for Linux $Arch" -ForegroundColor Green
    } else {
        Write-Host "Failed to pack Linux $Arch" -ForegroundColor Red
    }
}

Write-Host "Packing NuGet OpenCvSharp.Cuda" -ForegroundColor Cyan
dotnet pack "src/OpenCvSharp.Cuda/OpenCvSharp.Cuda.csproj" -c Release -o $OutputFolder

Write-Host "All packing complete!" -ForegroundColor Yellow