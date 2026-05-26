param(
    [string[]]$Build = @()   # e.g. -Build Combined   or   -Build Turing,Ampere
)

# Ensure the script keeps going even if dotnet test returns a 'failure' exit code
$ErrorActionPreference = "Continue"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$ScriptDir = $PSScriptRoot
$RepoRoot = (Resolve-Path (Join-Path $ScriptDir "../../")).Path.TrimEnd('\').TrimEnd('/')

Write-Host ">>> Repo root resolved to: $RepoRoot" -ForegroundColor DarkGray

$AllArchs = @("Turing", "Ampere", "Ada", "Blackwell", "Combined")

if ($Build.Count -gt 0) {
    $unrecognised = $Build | Where-Object { $AllArchs -notcontains $_ }
    if ($unrecognised) {
        throw "Unrecognised target(s): $($unrecognised -join ', '). Valid values: $($AllArchs -join ', ')"
    }
    $Archs = $Build
    Write-Host ">>> Testing only: $($Archs -join ', ')" -ForegroundColor Yellow
} else {
    $Archs = $AllArchs
    Write-Host ">>> Testing all architectures" -ForegroundColor Yellow
}

$TestProject = "$RepoRoot/test/OpenCvSharp.Cuda.Tests/OpenCvSharp.Cuda.Tests.csproj"
$ResultDir   = "$RepoRoot/test/test-windows"

if (Test-Path $ResultDir) { Remove-Item -Recurse -Force $ResultDir }
New-Item -ItemType Directory -Path $ResultDir > $null

Write-Host "Starting Multi-Architecture GPU Test Suite..." -ForegroundColor Cyan
Write-Host ("=" * 70)

foreach ($Arch in $Archs) {
    Write-Host ">>> [RUNNING] Architecture: $Arch" -ForegroundColor Yellow

    dotnet test $TestProject -c Release -p:CudaArch=$Arch --arch x64 --logger "trx;LogFileName=$Arch.trx" --results-directory $ResultDir --nologo > $null 2>&1

    $TrxPath = "$ResultDir/$Arch.trx"
    if (Test-Path $TrxPath) {
        [xml]$xml = Get-Content $TrxPath

        $allResults = @($xml.TestRun.Results.UnitTestResult)

        $total   = $allResults.Count
        $passed  = ($allResults | Where-Object { $_.outcome -eq 'Passed' }).Count
        $failed  = ($allResults | Where-Object { $_.outcome -eq 'Failed' }).Count
        $skipped = ($allResults | Where-Object { $_.outcome -eq 'NotExecuted' }).Count
        $aborted = ($allResults | Where-Object { $_.outcome -eq 'Aborted' }).Count
        $inconcl = ($allResults | Where-Object { $_.outcome -eq 'Inconclusive' }).Count

        Write-Host "`n--- $Arch RESULTS ---" -ForegroundColor Cyan
        Write-Host "Total Discovered: $total"
        Write-Host "Passed:           $passed" -ForegroundColor Green

        if ($failed -gt 0)  { Write-Host "Failed:           $failed" -ForegroundColor Red }
        if ($skipped -gt 0) { Write-Host "Skipped:          $skipped" -ForegroundColor Yellow }
        if ($inconcl -gt 0) { Write-Host "Inconclusive:     $inconcl" -ForegroundColor Magenta }
        if ($aborted -gt 0) { Write-Host "Aborted/Crash:    $aborted" -ForegroundColor DarkRed }

        $nonPassed = @($allResults | Where-Object { $_.outcome -ne 'Passed' })

        if ($nonPassed.Count -gt 0) {
            Write-Host "`nNon-Passed Test Details:" -ForegroundColor White
            foreach ($item in $nonPassed) {
                $status = $item.outcome
                $color  = "Red"

                if ($status -eq "NotExecuted")  { $color = "Yellow";  $status = "Skipped" }
                if ($status -eq "Inconclusive") { $color = "Magenta" }
                if ($status -eq "Aborted")      { $color = "DarkRed" }

                $label = "[$($status)]".PadRight(15)
                Write-Host "  $label $($item.testName)" -ForegroundColor $color
            }
        }
    } else {
        Write-Host "CRASHED: Could not find TRX results for $Arch." -ForegroundColor White -BackgroundColor Red
    }
    Write-Host ("-" * 70) -ForegroundColor Gray
}

Write-Host "`nAll architecture tests completed." -ForegroundColor Cyan