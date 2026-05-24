param(
    [string[]]$Build = @(),   # e.g. -Build Combined   or   -Build Turing,Ampere
     [switch]$Rebuild
)

$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$ScriptDir = $PSScriptRoot
$RepoRoot = (Resolve-Path (Join-Path $ScriptDir "../../")).Path.TrimEnd('\').TrimEnd('/')
$DockerfileDir = Join-Path $RepoRoot "docker/ubuntu24-dotnet10-opencv.cuda4.13.0-runtime"
$ImageName = "opencv-linux-runtime"



Write-Host ">>> Repo root resolved to: $RepoRoot" -ForegroundColor DarkGray
Write-Host ">>> DockerfileDir: $DockerfileDir" -ForegroundColor DarkGray
Write-Host ">>> Exists: $(Test-Path "$DockerfileDir/Dockerfile")" -ForegroundColor DarkGray

$ImageExists = (docker images -q opencv-linux-runtime)
if ($Rebuild -or -not $ImageExists) {
    Write-Host ">>> Docker image not found. Building it first..." -ForegroundColor Cyan
   docker build -t $ImageName -f "$DockerfileDir/Dockerfile" "$DockerfileDir"
}


if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker is not installed or not running. Please start Docker Desktop."
    exit 1
}


# Normalize line endings on the bash script
Write-Host ">>> Normalizing script line endings (CRLF -> LF)..." -ForegroundColor Gray
$bashFile = Join-Path $RepoRoot "scripts/test/test_all_linux.sh"
if (Test-Path $bashFile) {
    $content = [System.IO.File]::ReadAllText($bashFile)
    $content = $content -replace "`r`n", "`n"
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($bashFile, $content, $utf8NoBom)
}

$BuildArg = if ($Build.Count -gt 0) { "--build $($Build -join ',')" } else { "" }

Write-Host "`n>>> Launching Linux Tests via Docker..." -ForegroundColor Cyan
docker run --rm -i --gpus all `
    -e NVIDIA_DRIVER_CAPABILITIES=all `
    -v "${RepoRoot}:/repo" `
    $ImageName `
    bash /repo/scripts/test/test_all_linux.sh $BuildArg

Write-Host "`nLinux Automation Finished." -ForegroundColor Green