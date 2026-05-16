# Ensure the script keeps going even if dotnet test returns a 'failure' exit code
$ErrorActionPreference = "Continue"

$Archs = @("Turing", "Ampere", "Ada", "Blackwell", "Combined")
$TestProject = "test/OpenCvSharp.Cuda.Tests/OpenCvSharp.Cuda.Tests.csproj"
$ResultDir = "$PSScriptRoot/test/test-windows"

# Prepare results directory
if (Test-Path $ResultDir) { Remove-Item -Recurse -Force $ResultDir }
New-Item -ItemType Directory -Path $ResultDir > $null

Write-Host "Starting Multi-Architecture GPU Test Suite..." -ForegroundColor Cyan

foreach ($Arch in $Archs) {
    Write-Host ">>> [RUNNING] $Arch... " -NoNewline -ForegroundColor Gray
    
    # Run test silently - the output is captured in the TRX file
    # Redirecting output to $null ensures the console stays clean
    dotnet test $TestProject -c Release -p:CudaArch=$Arch --logger "trx;LogFileName=$Arch.trx" --results-directory $ResultDir > $null 2>&1

    $TrxPath = "$ResultDir/$Arch.trx"
    if (Test-Path $TrxPath) {
        [xml]$xml = Get-Content $TrxPath
        
        # Capture all failed results into an array
        $failedList = @($xml.TestRun.Results.UnitTestResult | Where-Object { $_.outcome -eq 'Failed' })
        $failCount = $failedList.Count
        $names = if ($failCount -gt 0) { $failedList.testName -join ", " } else { "None" }

        # --- SUMMARY LOGIC ---
        if ($failCount -eq 0) {
            Write-Host "PASSED" -ForegroundColor Green
            Write-Host "Summary: 0 failures." -ForegroundColor Green
        }
        else {
            Write-Host "FAILURES DETECTED" -ForegroundColor Red
            Write-Host "Summary: $failCount failures." -ForegroundColor Red
            Write-Host "Titles: $names" -ForegroundColor Red
        }
    } else {
        Write-Host "CRASHED" -ForegroundColor White -BackgroundColor Red
        Write-Host "Summary: Test runner failed to start or write results for $Arch." -ForegroundColor Red
    }
    Write-Host ("-" * 70) -ForegroundColor Gray
}

Write-Host "`nAll architecture tests completed." -ForegroundColor Cyan