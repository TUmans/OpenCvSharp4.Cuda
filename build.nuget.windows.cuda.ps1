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
    Write-Host "Packing NuGet package for $Arch..." -ForegroundColor Cyan
    
    dotnet pack $CsprojPath `
        -c Release `
        -p:CudaArch=$Arch `
        -o $OutputFolder

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully created package for $Arch" -ForegroundColor Green
    } else {
        Write-Host "Failed to pack $Arch" -ForegroundColor Red
    }
}

Write-Host "All packing complete! Check the $OutputFolder directory." -ForegroundColor Yellow