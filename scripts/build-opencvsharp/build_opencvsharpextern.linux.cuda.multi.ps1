param(
    [string[]]$Build = @()   # e.g. -Build Combined   or   -Build Turing,Ampere
)

$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$ScriptDir = $PSScriptRoot
$RepoRoot = (Resolve-Path (Join-Path $ScriptDir "../../")).Path.TrimEnd('\').TrimEnd('/')
$DockerfileDir = Join-Path $RepoRoot "docker/ubuntu24-dotnet10-opencv.cuda4.13.0-build"

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker is not installed or not running. Please start Docker Desktop."
    exit 1
}

# Ensure the builder image exists (it should, since we just built OpenCV)
$ImageExists = (docker images -q opencv-linux-builder)
if (-not $ImageExists) {
    Write-Host ">>> Docker image not found. Building it first..." -ForegroundColor Cyan
    docker build -t opencv-linux-builder -f "$DockerfileDir/Dockerfile" "$DockerfileDir"
}

# Pass -Build targets through to the shell script
$BuildArg = if ($Build.Count -gt 0) { "--build $($Build -join ',')" } else { "" }

Write-Host "`n>>> Running OpenCvSharpExtern Linux Build inside Docker..." -ForegroundColor Cyan
docker run --rm `
    -v "${RepoRoot}:/repo" `
    opencv-linux-builder `
    bash /repo/scripts/build-opencvsharp/build_opencvsharpextern.linux.cuda.multi.sh $BuildArg

Write-Host "`nDocker execution finished." -ForegroundColor Green