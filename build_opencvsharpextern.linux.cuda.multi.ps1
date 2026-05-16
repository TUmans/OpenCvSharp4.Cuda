$ErrorActionPreference = "Stop"
$RepoRoot = $PSScriptRoot
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker is not installed or not running. Please start Docker Desktop."
    exit 1
}

# Ensure the builder image exists (it should, since we just built OpenCV)
$ImageExists = (docker images -q opencv-linux-builder)
if (-not $ImageExists) {
    Write-Host ">>> Docker image not found. Building it first..." -ForegroundColor Cyan
    docker build -t opencv-linux-builder -f Dockerfile.linux .
}

Write-Host "`n>>> Running OpenCvSharpExtern Linux Build inside Docker..." -ForegroundColor Cyan

# Run the bash script inside the container
docker run --rm -v "${RepoRoot}:/repo" opencv-linux-builder bash ./build_opencvsharpextern.linux.cuda.multi.sh

Write-Host "`nDocker execution finished." -ForegroundColor Green