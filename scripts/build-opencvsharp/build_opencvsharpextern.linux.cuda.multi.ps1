param(
    [string[]]$Build = @(),   # e.g. -Build Combined   or   -Build Turing,Ampere
    [switch]$Rebuild
)

$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$ScriptDir = $PSScriptRoot
$RepoRoot = (Resolve-Path (Join-Path $ScriptDir "../../")).Path.TrimEnd('\').TrimEnd('/')
#$DockerfileDir = Join-Path $RepoRoot "docker/ubuntu24-dotnet10-opencv.cuda4.13.0-build"
$DockerfileDir = Join-Path $RepoRoot "docker/manylinux-dotnet10-opencv.cuda4.13.0-build"
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker is not installed or not running. Please start Docker Desktop."
    exit 1
}

Write-Host ">>> Repo root  : $RepoRoot" -ForegroundColor DarkGray
Write-Host ">>> Dockerfile : $DockerfileDir/Dockerfile" -ForegroundColor DarkGray

# Ensure the builder image exists (it should, since we just built OpenCV)
$ImageExists = (docker images -q opencv-linux-builder)
if ($Rebuild -or -not $ImageExists) {
    Write-Host ">>> Docker image not found. Building it first..." -ForegroundColor Cyan
    docker build -t opencv-linux-builder -f "$DockerfileDir/Dockerfile" "$RepoRoot"
}

Write-Host ">>> Normalizing script line endings (CRLF -> LF)..." -ForegroundColor Gray
$bashFile = Join-Path $RepoRoot "scripts/build-opencvsharp/build_opencvsharpextern.linux.cuda.multi.sh"
if (Test-Path $bashFile) {
    $content = [System.IO.File]::ReadAllText($bashFile)
    $content = $content -replace "`r`n", "`n"
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($bashFile, $content, $utf8NoBom)
}

# Pass -Build targets through to the shell script
$DockerArgs = @()
if ($Build.Count -gt 0) {
    $DockerArgs += "--build"
    $DockerArgs += ($Build -join ",")
}



Write-Host "`n>>> Running OpenCvSharpExtern Linux Build inside Docker..." -ForegroundColor Cyan
docker run --rm `
    -v "${RepoRoot}:/repo" `
    -e "VCPKG_FORCE_LINUX_PATHS=1" `
    opencv-linux-builder `
    bash /repo/scripts/build-opencvsharp/build_opencvsharpextern.linux.cuda.multi.sh $DockerArgs

Write-Host "`nDocker execution finished." -ForegroundColor Green