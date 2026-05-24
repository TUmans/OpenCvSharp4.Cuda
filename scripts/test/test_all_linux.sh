#!/bin/bash

ALL_ARCHS=("Turing" "Ampere" "Ada" "Blackwell" "Combined")
BUILD_FILTER=""

export PATH=$PATH:/usr/share/dotnet

# Parse arguments
while [[ $# -gt 0 ]]; do
    case "$1" in
        --build)
            BUILD_FILTER="$2"
            shift 2
            ;;
        *)
            echo "Unknown argument: $1" >&2
            exit 1
            ;;
    esac
done

# Filter architectures if --build was specified
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
            echo "Unrecognised target: $REQ. Valid values: ${ALL_ARCHS[*]}" >&2
            exit 1
        fi
    done
    echo ">>> Testing only: ${ARCHS[*]}"
else
    ARCHS=("${ALL_ARCHS[@]}")
    echo ">>> Testing all architectures"
fi

cd /repo
TEST_PROJECT="test/OpenCvSharp.Cuda.Tests/OpenCvSharp.Cuda.Tests.csproj"
RESULT_DIR="./test/test-linux"

mkdir -p "$RESULT_DIR"
rm -f "$RESULT_DIR"/*.trx

echo -e "\e[1;36mStarting Linux GPU Test Suite (.NET 10 / Docker)...\e[0m"
echo "======================================================================"

for ARCH in "${ARCHS[@]}"; do
    echo -e "\e[1;33m>>> [RUNNING] Architecture: $ARCH\e[0m"

    BIN_DIR="/repo/test/OpenCvSharp.Cuda.Tests/bin/Release/net10.0"
    OPENCV_LIBS="/repo/opencv_artifacts/linux/$ARCH/lib"

    LD_LIBRARY_PATH=${LD_LIBRARY_PATH:-}:$BIN_DIR:$OPENCV_LIBS \
    dotnet test "$TEST_PROJECT" \
        -c Release \
        -f net10.0 \
        -p:CudaArch=$ARCH \
        -p:SignAssembly=false \
        -p:PublicSign=false \
        --logger "trx;LogFileName=$ARCH.trx" \
        --nologo \
        --results-directory "$RESULT_DIR"  > /dev/null 2>&1 || true

    TRX_FILE="$RESULT_DIR/$ARCH.trx"
    if [ -f "$TRX_FILE" ]; then
        TOTAL=$(grep -c "<UnitTestResult" "$TRX_FILE")
        PASSED=$(grep "<UnitTestResult" "$TRX_FILE" | grep -c 'outcome="Passed"')
        FAILED=$(grep "<UnitTestResult" "$TRX_FILE" | grep -c 'outcome="Failed"')
        SKIPPED=$(grep "<UnitTestResult" "$TRX_FILE" | grep -c 'outcome="NotExecuted"')
        INCONCL=$(grep "<UnitTestResult" "$TRX_FILE" | grep -c 'outcome="Inconclusive"')
        ABORTED=$(grep "<UnitTestResult" "$TRX_FILE" | grep -E -c 'outcome="(Aborted|Error)"')

        echo -e "\n\e[1;36m--- $ARCH RESULTS ---\e[0m"
        echo "Total Discovered: $TOTAL"
        echo -e "Passed:           \e[1;32m$PASSED\e[0m"

        [ "$FAILED" -gt 0 ]  && echo -e "Failed:           \e[1;31m$FAILED\e[0m"
        [ "$SKIPPED" -gt 0 ] && echo -e "Skipped:          \e[1;33m$SKIPPED\e[0m"
        [ "$INCONCL" -gt 0 ] && echo -e "Inconclusive:     \e[1;35m$INCONCL\e[0m"
        [ "$ABORTED" -gt 0 ] && echo -e "Aborted/Crash:    \e[1;31m$ABORTED\e[0m"

        if [ "$PASSED" -ne "$TOTAL" ]; then
            echo -e "\n\e[1;37mNon-Passed Test Details:\e[0m"
            grep "<UnitTestResult" "$TRX_FILE" | grep -v 'outcome="Passed"' | \
            sed -E 's/.*testName="([^"]*)".*outcome="([^"]*)".*/\2|\1/' | \
            while IFS='|' read -r status name; do
                case $status in
                    "Failed")       COLOR="\e[1;31m"; LABEL="[Failed]       ";;
                    "NotExecuted")  COLOR="\e[1;33m"; LABEL="[Skipped]      ";;
                    "Inconclusive") COLOR="\e[1;35m"; LABEL="[Inconclusive] ";;
                    "Aborted")      COLOR="\e[1;31m"; LABEL="[Aborted]      ";;
                    "Error")        COLOR="\e[1;31m"; LABEL="[Error]        ";;
                    *)              COLOR="\e[0m";    LABEL="[$status] ";;
                esac
                echo -e "  ${COLOR}${LABEL}\e[0m $name"
            done
        fi
    else
        echo -e "\e[1;37;41mCRASHED\e[0m: Could not find TRX results for $ARCH."
    fi
    echo "----------------------------------------------------------------------"
done

echo -e "\n\e[1;36mAll Linux architecture tests completed.\e[0m"