#!/bin/bash
set -e # Stop on error

REPO_ROOT=$(pwd)
JOBS=$(nproc) # Automatically use all available CPU cores

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
TARGETS=(
    "Turing:7.5:7.5"
    "Ampere:8.6:8.6"
    "Ada:8.9:8.9"
    "Blackwell:10.0:10.0"
    "Combined:7.5,8.6,8.9,10.0:10.0"
)

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