param(
    [string[]]$Build    = @(),       # e.g. -Build Combined   or   -Build Turing,Ampere
    [switch]  $Bisect,               # run each test solo to find the crashing one
    [int]     $HangTimeout = 120     # seconds before a test is considered hung
)

$ErrorActionPreference = "Continue"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# -------------------------------------------------------------
#  Paths
# -------------------------------------------------------------
$ScriptDir  = $PSScriptRoot
$RepoRoot   = (Resolve-Path (Join-Path $ScriptDir "../../")).Path.TrimEnd('\').TrimEnd('/')
$TestProject = "$RepoRoot/test/OpenCvSharp.Cuda.Tests/OpenCvSharp.Cuda.Tests.csproj"
$ResultDir   = "$RepoRoot/test/test-windows"

# -------------------------------------------------------------
#  Architecture selection
# -------------------------------------------------------------
$AllArchs = @("Turing", "Ampere", "Ada", "Blackwell", "Combined")
if ($Build.Count -gt 0) {
    $Archs = $Build | Where-Object { $AllArchs -contains $_ }
} else {
    $Archs = $AllArchs
}

Write-Host "Repo root : $RepoRoot" -ForegroundColor DarkGray
Write-Host "Architectures : $($Archs -join ', ')" -ForegroundColor Cyan
Write-Host ("=" * 70)

# -------------------------------------------------------------
#  Helper Functions
# -------------------------------------------------------------

function Get-SequenceInfo {
    param([string]$ResultDir)
    $file = Get-ChildItem -Path $ResultDir -Recurse -Filter "*Sequence.xml" -ErrorAction SilentlyContinue |
            Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if (-not $file) { return $null }

    [xml]$seq = Get-Content $file.FullName
    $tests    = @($seq.Sequence.Test)
    $crashed  = $tests | Where-Object { $_.Completed -eq "False" } | Select-Object -First 1

    return [PSCustomObject]@{
        Total       = $tests.Count
        CrashedTest = if ($crashed) { $crashed.Name } else { $null }
    }
}

function Write-ArchReport {
    param([string]$Arch, [string]$ArchDir, [int]$ExitCode)

    Write-Host ""
    Write-Host ">>> SUMMARY FOR ARCHITECTURE: $Arch <<<" -ForegroundColor Black -BackgroundColor Cyan

    $seq = Get-SequenceInfo -ResultDir $ArchDir
    $trxFile = Get-ChildItem -Path $ArchDir -Filter "*.trx" | Select-Object -First 1

    if ($trxFile) {
        [xml]$xml = Get-Content $trxFile.FullName
        $results = @($xml.TestRun.Results.UnitTestResult)
        
        $total   = $results.Count
        $passed  = ($results | Where-Object { $_.outcome -eq 'Passed' }).Count
        $failed  = ($results | Where-Object { $_.outcome -eq 'Failed' }).Count
        $skipped = ($results | Where-Object { $_.outcome -eq 'NotExecuted' -or $_.outcome -eq 'Skipped' }).Count

        # Determine colors without using ternary operators
        $failColor = "Gray";    if ($failed -gt 0) { $failColor = "Red" }
        $skipColor = "Gray";    if ($skipped -gt 0) { $skipColor = "Yellow" }

        Write-Host "  Total Tests: $total"
        Write-Host "  Passed     : $passed" -ForegroundColor Green
        Write-Host "  Failed     : $failed" -ForegroundColor $failColor
        Write-Host "  Skipped    : $skipped" -ForegroundColor $skipColor

        # List Skipped Tests
        $skippedTests = @($results | Where-Object { $_.outcome -eq 'NotExecuted' -or $_.outcome -eq 'Skipped' })
        if ($skippedTests.Count -gt 0) {
            Write-Host "`n  SKIPPED TESTS:" -ForegroundColor Yellow
            foreach ($s in $skippedTests) {
                Write-Host "    - $($s.testName)" -ForegroundColor DarkYellow
            }
        }

        # List Failed Tests (Logical Failures)
        $failedTests = @($results | Where-Object { $_.outcome -eq 'Failed' })
        if ($failedTests.Count -gt 0) {
            Write-Host "`n  FAILED TESTS (LOGIC):" -ForegroundColor Red
            foreach ($f in $failedTests) {
                Write-Host "    [FAIL] $($f.testName)" -ForegroundColor Red
                if ($f.Output.ErrorInfo) {
                    Write-Host "           Err: $($f.Output.ErrorInfo.Message.Trim())" -ForegroundColor Gray
                }
            }
        }
    } else {
        Write-Host "  !! NO TRX FILE GENERATED !!" -ForegroundColor Red
    }

    # Handle Hard Crashes (Access Violations)
    # Note: Exit code 1 usually indicates standard test failures, anything else is usually a crash.
    if ($ExitCode -ne 0 -and $ExitCode -ne 1) {
        Write-Host "`n  !! CRITICAL PROCESS CRASH DETECTED !!" -ForegroundColor White -BackgroundColor DarkRed
        Write-Host "  Exit Code: $ExitCode" -ForegroundColor Red
        if ($seq -and $seq.CrashedTest) {
            Write-Host "  Faulting Test: $($seq.CrashedTest)" -ForegroundColor Yellow -BackgroundColor Black
            Write-Host "  Hint: This test likely caused an Access Violation (0xC0000005) in native code." -ForegroundColor Gray
        }
    }
    
    Write-Host ("-" * 70)
}

# -------------------------------------------------------------
#  Main Loop
# -------------------------------------------------------------
if (Test-Path $ResultDir) { Remove-Item -Recurse -Force $ResultDir }
New-Item -ItemType Directory -Path $ResultDir > $null

foreach ($Arch in $Archs) {
    Write-Host "Testing Architecture: $Arch ..." -ForegroundColor Cyan
    
    $ArchDir = Join-Path $ResultDir $Arch
    if (-not (Test-Path $ArchDir)) { New-Item -ItemType Directory -Path $ArchDir > $null }

    # Execute tests with blame-crash enabled
    & dotnet test $TestProject -c Release "-p:CudaArch=$Arch" --arch x64 `
        --blame-crash --logger trx --results-directory $ArchDir --nologo 2>&1 | Out-Null
    
    $exitCode = $LASTEXITCODE

    # REPORT IMMEDIATELY BEFORE MOVING TO NEXT ARCH
    Write-ArchReport -Arch $Arch -ArchDir $ArchDir -ExitCode $exitCode
}

Write-Host "`nAll requested architectures finished." -ForegroundColor Green