#!/bin/bash

ARCHS=("Turing" "Ampere" "Ada" "Blackwell" "Combined")
TEST_PROJECT="test/OpenCvSharp.Cuda.Tests/OpenCvSharp.Cuda.Tests.csproj"
RESULT_DIR="./test/test-linux"

mkdir -p $RESULT_DIR
rm -f $RESULT_DIR/*.trx

echo -e "\e[1;36mStarting Linux GPU Test Suite (.NET 10 / Docker)...\e[0m"

# Set Library Paths
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/usr/local/cuda/lib64:/usr/local/cuda/targets/x86_64-linux/lib

for ARCH in "${ARCHS[@]}"; do
    echo -n ">>> [RUNNING] $ARCH... "
    
    BIN_DIR="/repo/test/OpenCvSharp.Cuda.Tests/bin/Release/net10.0"
    
    LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$BIN_DIR \
    dotnet test "$TEST_PROJECT" \
        -c Release \
        -f net10.0 \
        -p:CudaArch=$ARCH \
        -p:SignAssembly=false \
        -p:PublicSign=false \
        --logger "trx;LogFileName=$ARCH.trx" \
        --results-directory $RESULT_DIR > /dev/null 2>&1 || true

    TRX_FILE="$RESULT_DIR/$ARCH.trx"
    if [ -f "$TRX_FILE" ]; then
        # FIX: Only look at <UnitTestResult> lines to ignore the Summary tag
        FAILED_LINES=$(grep '<UnitTestResult' "$TRX_FILE" | grep 'outcome="Failed"')
        
        # Count failures accurately
        FAIL_COUNT=$(echo "$FAILED_LINES" | grep -v '^$' | wc -l)
        
        # Extract clean titles
        NAMES=$(echo "$FAILED_LINES" | sed -E 's/.*testName="([^"]*)".*/\1/' | tr '\n' ',' | sed 's/,$//')

        if [ "$FAIL_COUNT" -eq 0 ]; then
            echo -e "\e[1;32mPASSED\e[0m"
            echo -e "   Summary: 0 failures."
        else
            echo -e "\e[1;31mFAILED\e[0m (Actual Count: $FAIL_COUNT)"
            echo -e "   Titles: \e[0;90m$NAMES\e[0m"
        fi
    else
        echo -e "\e[1;37;41mCRASHED\e[0m"
    fi
    echo "----------------------------------------------------------------------"
done

echo -e "\n\e[1;36mAll Linux architecture tests completed.\e[0m"