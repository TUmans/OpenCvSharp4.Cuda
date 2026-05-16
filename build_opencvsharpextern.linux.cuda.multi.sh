#!/bin/bash
set -e # Stop on error

REPO_ROOT=$(pwd)
JOBS=$(nproc) # Automatically use all available CPU cores

echo "Detected $JOBS CPU cores for building."

command -v cmake >/dev/null 2>&1 || { echo "cmake is required. Aborting." >&2; exit 1; }
command -v ninja >/dev/null 2>&1 || { echo "ninja is required. Aborting." >&2; exit 1; }

# Paths
BASE_INSTALL_DIR="$REPO_ROOT/opencv_artifacts/linux"
EXTERN_SOURCE="$REPO_ROOT/src/OpenCvSharpExtern"
FINAL_DIST="$REPO_ROOT/src/build/linux"

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
    echo -e "\e[1;35m STARTING EXTERN BUILD: $NAME (sm_$ARCH)\e[0m"
    echo -e "\e[1;35m=======================================================\e[0m"

    BUILD_DIR_EX="$REPO_ROOT/src/build/linux/$NAME"
    INSTALL_DIR_ARCH="$BASE_INSTALL_DIR/$NAME"
    TARGET_FOLDER="$FINAL_DIST/$NAME"

    # Clean previous build attempt
    rm -rf "$BUILD_DIR_EX"
    mkdir -p "$TARGET_FOLDER"

    # Dynamically locate OpenCVConfig.cmake
    OPENCV_CONFIG_FILE=$(find "$INSTALL_DIR_ARCH" -name "OpenCVConfig.cmake" | head -n 1)
    if [ -z "$OPENCV_CONFIG_FILE" ]; then
        echo -e "\e[1;31mFailed to find OpenCVConfig.cmake in $INSTALL_DIR_ARCH.\e[0m"
        exit 1
    fi
    OPENCV_CONFIG_PATH=$(dirname "$OPENCV_CONFIG_FILE")
    echo -e "\e[1;36m>>> Found OpenCVConfig.cmake at: $OPENCV_CONFIG_PATH\e[0m"

    # --- CONFIGURE ---
    # Note: Linux does not use multi-threaded runtime flags (/MT) like Windows does.
    cmake -S "$EXTERN_SOURCE" -B "$BUILD_DIR_EX" -G "Ninja" \
          -D CMAKE_BUILD_TYPE=Release \
          -D ENABLED_CUDA=ON \
          -D OpenCV_DIR="$OPENCV_CONFIG_PATH" \
          -D CMAKE_TOOLCHAIN_FILE="$VCPKG_TOOLCHAIN" \
          -D VCPKG_TARGET_TRIPLET="x64-linux-static"

    # --- COMPILE ---
    echo -e "\e[1;30m>>> Linking libOpenCvSharpExtern.so...\e[0m"
    cmake --build "$BUILD_DIR_EX" --config Release -j $JOBS

   

done

echo -e "\n\e[1;33mAll Extern Builds Complete! Check the '$FINAL_DIST' folder.\e[0m"