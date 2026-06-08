#!/bin/bash

# -------------------------------------------------------------
# Configuration & Colors
# -------------------------------------------------------------
ALL_ARCHS=("Turing" "Ampere" "Ada" "Blackwell" "Combined")
BUILD_FILTER=""

export PATH=$PATH:/usr/share/dotnet

C_RESET="\e[0m"
C_CYAN="\e[1;36m"
C_GREEN="\e[1;32m"
C_RED="\e[1;31m"
C_YELLOW="\e[1;33m"
C_GRAY="\e[1;30m"
C_WHITE="\e[1;37m"
C_BG_CYAN="\e[1;30;46m"
C_BG_RED="\e[1;37;41m"
C_BG_YELLOW="\e[1;30;43m"

# -------------------------------------------------------------
# Parse arguments
# -------------------------------------------------------------
while [[ $# -gt 0 ]]; do
    case "$1" in
        --build)
            BUILD_FILTER="$2"
            shift 2
            ;;
        *)
            echo -e "${C_RED}Unknown argument: $1${C_RESET}" >&2
            exit 1
            ;;
    esac
done

# Filter architectures
if [ -n "$BUILD_FILTER" ]; then
    ARCHS=()
    IFS=',' read -ra REQUESTED <<< "$BUILD_FILTER"
    for ARCH in "${ALL_ARCHS[@]}"; do
        for REQ in "${REQUESTED[@]}"; do
            if [ "$ARCH" = "$REQ" ]; then
                ARCHS+=("$ARCH")
                break
            fi
        done
    done
    # Validate
    for REQ in "${REQUESTED[@]}"; do
        FOUND=0
        for ARCH in "${ALL_ARCHS[@]}"; do
            [ "$ARCH" = "$REQ" ] && FOUND=1 && break
        done
        if [ $FOUND -eq 0 ]; then
            echo -e "${C_RED}Unrecognised target: $REQ. Valid values: ${ALL_ARCHS[*]}${C_RESET}" >&2
            exit 1
        fi
    done
else
    ARCHS=("${ALL_ARCHS[@]}")
fi

echo -e "${C_GRAY}Repo root : /repo${C_RESET}"
echo -e "${C_CYAN}Architectures : ${ARCHS[*]}${C_RESET}"
echo "======================================================================"

# -------------------------------------------------------------
# Paths & Cleanup
# -------------------------------------------------------------
cd /repo || exit 1
TEST_PROJECT="test/OpenCvSharp.Cuda.Tests/OpenCvSharp.Cuda.Tests.csproj"
RESULT_DIR="./test/test-linux"

# Equivalent to: if (Test-Path) { Remove-Item -Recurse }
rm -rf "$RESULT_DIR"
mkdir -p "$RESULT_DIR"

# -------------------------------------------------------------
# Main Loop
# -------------------------------------------------------------
for ARCH in "${ARCHS[@]}"; do
    echo -e "${C_CYAN}Testing Architecture: $ARCH ...${C_RESET}"

    # Isolate results per architecture so Sequence.xml doesn't get overwritten
    ARCH_DIR="$RESULT_DIR/$ARCH"
    mkdir -p "$ARCH_DIR"

    BIN_DIR="/repo/test/OpenCvSharp.Cuda.Tests/bin/Release/net10.0"
    OPENCV_LIBS="/repo/opencv_artifacts/linux/$ARCH/lib"

    # Execute tests with --blame-crash enabled.
    # Note: Removed "|| true" so we can capture the actual exit code!
    LD_LIBRARY_PATH=${LD_LIBRARY_PATH:-}:$BIN_DIR:$OPENCV_LIBS \
    dotnet test "$TEST_PROJECT" \
        -c Release \
        -f net10.0 \
        -p:CudaArch=$ARCH \
        --arch x64 \
        -p:SignAssembly=false \
        -p:PublicSign=false \
        --blame-crash \
        --logger "trx" \
        --nologo \
        --results-directory "$ARCH_DIR"  > /dev/null 2>&1 || true
    
    EXIT_CODE=$?

    # -------------------------------------------------------------
    # Report Generation
    # -------------------------------------------------------------
    echo ""
    echo -e "${C_BG_CYAN}>>> SUMMARY FOR ARCHITECTURE: $ARCH <<<${C_RESET}"

    # Find the TRX file
    TRX_FILE=$(find "$ARCH_DIR" -maxdepth 1 -name "*.trx" | head -n 1)
    
    # Find Sequence.xml and extract the crashed test
    SEQ_FILE=$(find "$ARCH_DIR" -maxdepth 1 -name "*Sequence.xml" | head -n 1)
    CRASHED_TEST=""
    if [ -n "$SEQ_FILE" ]; then
        # Looks for Completed="False" and extracts the test Name attribute
        CRASHED_TEST=$(grep -m 1 'Completed="False"' "$SEQ_FILE" | sed -E 's/.*Name="([^"]*)".*/\1/')
    fi

    if [ -n "$TRX_FILE" ] && [ -f "$TRX_FILE" ]; then
        TOTAL=$(grep -c "<UnitTestResult" "$TRX_FILE")
        PASSED=$(grep "<UnitTestResult" "$TRX_FILE" | grep -c 'outcome="Passed"')
        FAILED=$(grep "<UnitTestResult" "$TRX_FILE" | grep -c 'outcome="Failed"')
        SKIPPED=$(grep "<UnitTestResult" "$TRX_FILE" | grep -E -c 'outcome="(NotExecuted|Skipped)"')

        F_COLOR=$C_GRAY; [ "$FAILED" -gt 0 ] && F_COLOR=$C_RED
        S_COLOR=$C_GRAY; [ "$SKIPPED" -gt 0 ] && S_COLOR=$C_YELLOW

        echo -e "  Total Tests: $TOTAL"
        echo -e "  Passed     : ${C_GREEN}$PASSED${C_RESET}"
        echo -e "  Failed     : ${F_COLOR}$FAILED${C_RESET}"
        echo -e "  Skipped    : ${S_COLOR}$SKIPPED${C_RESET}"

        # List Skipped Tests
        if [ "$SKIPPED" -gt 0 ]; then
            echo -e "\n  ${C_YELLOW}SKIPPED TESTS:${C_RESET}"
            grep "<UnitTestResult" "$TRX_FILE" | grep -E 'outcome="(NotExecuted|Skipped)"' | \
            sed -E 's/.*testName="([^"]*)".*/\1/' | while read -r name; do
                echo -e "    - $name"
            done
        fi

        # List Failed Tests (Logic Failures)
        if [ "$FAILED" -gt 0 ]; then
            echo -e "\n  ${C_RED}FAILED TESTS (LOGIC):${C_RESET}"
            grep "<UnitTestResult" "$TRX_FILE" | grep 'outcome="Failed"' | \
            sed -E 's/.*testName="([^"]*)".*/\1/' | while read -r name; do
                echo -e "    ${C_RED}[FAIL]${C_RESET} $name"
            done
        fi
    else
        echo -e "  ${C_RED}!! NO TRX FILE GENERATED !!${C_RESET}"
    fi

    # Handle Hard Crashes (Segmentation Faults / Aborted)
    # Exit code 0 is Success. Exit code 1 is standard Test Failures.
    # Linux segfaults often result in exit code 139 (128 + 11).
    if [ "$EXIT_CODE" -ne 0 ] && [ "$EXIT_CODE" -ne 1 ]; then
        echo -e "\n  ${C_BG_RED}!! CRITICAL PROCESS CRASH DETECTED !!${C_RESET}"
        echo -e "  ${C_RED}Exit Code: $EXIT_CODE${C_RESET}"
        
        if [ -n "$CRASHED_TEST" ]; then
            echo -e "  ${C_BG_YELLOW}${C_GRAY}Faulting Test: $CRASHED_TEST${C_RESET}"
            echo -e "  ${C_GRAY}Hint: This test likely caused a Segmentation Fault (SIGSEGV) in native code.${C_RESET}"
        else
            echo -e "  ${C_GRAY}Hint: Process crashed, but no faulting test could be determined from Sequence.xml.${C_RESET}"
        fi
    fi

    echo "----------------------------------------------------------------------"
done

echo -e "\n${C_GREEN}All requested architectures finished.${C_RESET}"