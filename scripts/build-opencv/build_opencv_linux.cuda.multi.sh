#!/bin/bash
set -e # Stop on error

REPO_ROOT=$(pwd)
JOBS=$(nproc) # Automatically use all available CPU cores
BUILD=""

while [[ $# -gt 0 ]]; do
    case "$1" in
        --build)
            BUILD="$2"
            shift 2
            ;;
        *)
            echo "Unknown argument: $1" >&2
            exit 1
            ;;
    esac
done

echo "Detected $JOBS CPU cores for building."

# Check for required tools
command -v cmake >/dev/null 2>&1 || { echo "cmake is required but not installed. Aborting." >&2; exit 1; }
command -v ninja >/dev/null 2>&1 || { echo "ninja is required but not installed. Aborting." >&2; exit 1; }

# Paths
BASE_INSTALL_DIR="$REPO_ROOT/opencv_artifacts/linux"
BASE_BUILD_DIR="$REPO_ROOT/extern/OpenCvSharp/opencv/build-linux"

# Ensure vcpkg is found
if [ -z "$VCPKG_INSTALLATION_ROOT" ]; then
    echo "Please set VCPKG_INSTALLATION_ROOT environment variable. Aborting."
    exit 1
fi
VCPKG_TOOLCHAIN="$VCPKG_INSTALLATION_ROOT/scripts/buildsystems/vcpkg.cmake"

# Define Targets format: "Name:Arch:Ptx"
ALL_TARGETS=(
    "Turing:7.5:7.5"
    "Ampere:8.6:8.6"
    "Ada:8.9:8.9"
    "Blackwell:10.0:10.0"
    "Combined:7.5,8.6,8.9,10.0:10.0"
)

# Filter targets if --only was specified
if [ -n "$BUILD" ]; then
    echo ">>> Building only: $BUILD"
    TARGETS=()
    IFS=',' read -ra REQUESTED <<< "$BUILD"
    for TARGET in "${ALL_TARGETS[@]}"; do
        NAME="${TARGET%%:*}"
        for REQ in "${REQUESTED[@]}"; do
            if [ "$NAME" = "$REQ" ]; then
                TARGETS+=("$TARGET")
                break
            fi
        done
    done
    # Validate all requested names were found
    for REQ in "${REQUESTED[@]}"; do
        FOUND=0
        for TARGET in "${ALL_TARGETS[@]}"; do
            NAME="${TARGET%%:*}"
            [ "$NAME" = "$REQ" ] && FOUND=1 && break
        done
        if [ $FOUND -eq 0 ]; then
            echo "Unrecognised target: $REQ. Valid values: Turing, Ampere, Ada, Blackwell, Combined" >&2
            exit 1
        fi
    done
else
    echo ">>> Building all targets"
    TARGETS=("${ALL_TARGETS[@]}")
fi

for TARGET in "${TARGETS[@]}"; do
    IFS=":" read -r NAME ARCH PTX <<< "$TARGET"
    
    echo -e "\n\e[1;35m=======================================================\e[0m"
    echo -e "\e[1;35m STARTING LINUX OPENCV BUILD: $NAME (sm_$ARCH)\e[0m"
    echo -e "\e[1;35m=======================================================\e[0m"

    BUILD_DIR_CV="$BASE_BUILD_DIR/$NAME"
    INSTALL_DIR_ARCH="$BASE_INSTALL_DIR/$NAME"

    # Clean previous attempts
    echo ">>> Cleaning previous builds..."
    rm -rf "$BUILD_DIR_CV" "$INSTALL_DIR_ARCH"

    # --- STEP 1: CONFIGURE OPENCV ---
    echo -e "\e[1;36m>>> Configuring OpenCV for $NAME...\e[0m"
    
    cmake -C "$REPO_ROOT/cmake/opencv_build_options_cuda.cmake" \
          -S "$REPO_ROOT/extern/OpenCvSharp/opencv" \
          -B "$BUILD_DIR_CV" \
          -G "Ninja" \
          -D CMAKE_BUILD_TYPE=Release \
          -D VCPKG_OVERLAY_TRIPLETS="$REPO_ROOT/extern/OpenCvSharp/cmake/triplets" \
          -D CMAKE_TOOLCHAIN_FILE="$VCPKG_TOOLCHAIN" \
          -D VCPKG_TARGET_TRIPLET="x64-linux-static" \
          -D OPENCV_EXTRA_MODULES_PATH="$REPO_ROOT/extern/OpenCvSharp/opencv_contrib/modules" \
          -D CMAKE_INSTALL_PREFIX="$INSTALL_DIR_ARCH" \
          -D CUDA_ARCH_BIN="$ARCH" \
          -D CUDA_ARCH_PTX="$PTX"



    # --- STEP 2: COMPILE OPENCV ---
    echo -e "\e[1;30m>>> Compiling OpenCV (this may take a while)...\e[0m"
    cmake --build "$BUILD_DIR_CV" --config Release -j $JOBS
    
    # --- STEP 3: INSTALL ---
    echo -e "\e[1;30m>>> Installing to $INSTALL_DIR_ARCH...\e[0m"
    cmake --install "$BUILD_DIR_CV" --config Release

done

echo -e "\n\e[1;32mAll Linux OpenCV Builds Complete!\e[0m"
echo "Artifacts are located in: $BASE_INSTALL_DIR"