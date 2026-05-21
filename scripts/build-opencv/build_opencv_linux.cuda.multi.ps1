param(
    [string[]]$Build = @()   # e.g. -Build Combined   or   -Build Turing,Ampere
)

$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# /scripts/build-opencv/ -> /scripts/ -> /
$ScriptDir = $PSScriptRoot
$RepoRoot = (Resolve-Path (Join-Path $ScriptDir "../../")).Path.TrimEnd('\').TrimEnd('/')

$DockerfileDir = Join-Path $RepoRoot "docker/ubuntu24-dotnet10-opencv.cuda4.13.0-build"

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker is not installed or not running. Please start Docker Desktop."
    exit 1
}


Write-Host ">>> Repo root  : $RepoRoot" -ForegroundColor DarkGray
Write-Host ">>> Dockerfile : $DockerfileDir/Dockerfile" -ForegroundColor DarkGray
Write-Host ">>> Step 1: Building Docker image..." -ForegroundColor Cyan
docker build -t opencv-linux-builder -f "$DockerfileDir/Dockerfile" "$DockerfileDir"

Write-Host "`n>>> Step 2: Running OpenCV Linux Build inside Docker..." -ForegroundColor Cyan
docker run --rm `
    -v "${RepoRoot}:/repo" `
    opencv-linux-builder `
    bash /repo/scripts/build-opencv/build_opencv_linux.cuda.multi.sh $Build

Write-Host "`nDocker execution finished." -ForegroundColor Green