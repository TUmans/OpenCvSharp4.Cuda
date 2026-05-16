$ErrorActionPreference = "Stop"
$RepoRoot = $PSScriptRoot
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker is not installed or not running. Please start Docker Desktop."
    exit 1
}

Write-Host ">>> Step 1: Preparing Linux Docker Image (opencv-linux-builder)..." -ForegroundColor Cyan
# This uses the Dockerfile.linux we just created
docker build -t opencv-linux-builder -f Dockerfile .

Write-Host "`n>>> Step 2: Running OpenCV Linux Build inside Docker..." -ForegroundColor Cyan
# --rm removes the container after it's done
# -v "${RepoRoot}:/repo" mounts your Windows folder into the Linux container
docker run --rm -v "${RepoRoot}:/repo" opencv-linux-builder bash ./build_opencv_linux.cuda.multi.sh

Write-Host "`nDocker execution finished." -ForegroundColor Green