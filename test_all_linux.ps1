$ErrorActionPreference = "Stop"
$RepoRoot = $PSScriptRoot

Write-Host ">>> Preparing Linux Docker Environment..." -ForegroundColor Cyan
docker build -t opencv-linux-builder -f Dockerfile .

Write-Host ">>> Normalizing script line endings (CRLF -> LF)..." -ForegroundColor Gray
$bashFile = "$RepoRoot/test_all_linux.sh"
if (Test-Path $bashFile) {
    $content = [System.IO.File]::ReadAllText($bashFile)
    $content = $content -replace "`r`n", "`n"
    # Write as UTF8 without BOM (Linux standard)
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($bashFile, $content, $utf8NoBom)
}

Write-Host "`n>>> Launching Linux Tests via Docker..." -ForegroundColor Cyan
# Run with --gpus all
docker run --rm -i --gpus all -v "${RepoRoot}:/repo" opencv-linux-builder bash ./test_all_linux.sh

Write-Host "`nLinux Automation Finished." -ForegroundColor Green