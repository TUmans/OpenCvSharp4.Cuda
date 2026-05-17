# Ensure the script keeps going even if dotnet test returns a 'failure' exit code
$ErrorActionPreference = "Continue"

$Archs = @("Turing", "Ampere", "Ada", "Blackwell", "Combined")
$TestProject = "test/OpenCvSharp.Cuda.Tests/OpenCvSharp.Cuda.Tests.csproj"
$ResultDir = "$PSScriptRoot/test/test-windows"

if (Test-Path $ResultDir) { Remove-Item -Recurse -Force $ResultDir }
New-Item -ItemType Directory -Path $ResultDir > $null

Write-Host "Starting Multi-Architecture GPU Test Suite..." -ForegroundColor Cyan
Write-Host ("=" * 70)

foreach ($Arch in $Archs) {
    Write-Host ">>> [RUNNING] Architecture: $Arch" -ForegroundColor Yellow
    
    dotnet test $TestProject -c Release -p:CudaArch=$Arch --logger "trx;LogFileName=$Arch.trx" --results-directory $ResultDir --nologo > $null 2>&1

    $TrxPath = "$ResultDir/$Arch.trx"
    if (Test-Path $TrxPath) {
        [xml]$xml = Get-Content $TrxPath
        
        # Get all test results into an array
        $allResults = @($xml.TestRun.Results.UnitTestResult)
        
        # Manually count based on the actual results list to ensure accuracy
        $total    = $allResults.Count
        $passed   = ($allResults | Where-Object { $_.outcome -eq 'Passed' }).Count
        $failed   = ($allResults | Where-Object { $_.outcome -eq 'Failed' }).Count
        $skipped  = ($allResults | Where-Object { $_.outcome -eq 'NotExecuted' }).Count
        $aborted  = ($allResults | Where-Object { $_.outcome -eq 'Aborted' }).Count
        $inconcl  = ($allResults | Where-Object { $_.outcome -eq 'Inconclusive' }).Count

        Write-Host "`n--- $Arch RESULTS ---" -ForegroundColor Cyan
        Write-Host "Total Discovered: $total"
        Write-Host "Passed:           $passed" -ForegroundColor Green
        
        # Display summary lines only for non-zero counts
        if ($failed -gt 0)  { Write-Host "Failed:           $failed" -ForegroundColor Red }
        if ($skipped -gt 0) { Write-Host "Skipped:          $skipped" -ForegroundColor Yellow }
        if ($inconcl -gt 0) { Write-Host "Inconclusive:     $inconcl" -ForegroundColor Magenta }
        if ($aborted -gt 0) { Write-Host "Aborted/Crash:    $aborted" -ForegroundColor DarkRed }

        # List all tests that did NOT Pass
        $nonPassed = @($allResults | Where-Object { $_.outcome -ne 'Passed' })
        
        if ($nonPassed.Count -gt 0) {
            Write-Host "`nNon-Passed Test Details:" -ForegroundColor White
            foreach ($item in $nonPassed) {
                $status = $item.outcome
                $color = "Red"
                
                # Format the status label for the list
                if ($status -eq "NotExecuted")  { $color = "Yellow"; $status = "Skipped" }
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