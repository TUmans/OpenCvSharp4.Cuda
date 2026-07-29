#!/bin/bash
set -e

REPO_ROOT=$(pwd)
JOBS=$(nproc)
BUILD_FILTER=""

# Parse arguments
while [[ $# -gt 0 ]]; do
    case "$1" in
        --build)
            BUILD="$2"
            shift 2
            ;;
        *)
            if [[ -z "$BUILD" ]]; then
                BUILD="$1"
                shift 1
            else
                echo "Unknown argument: $1" >&2
                exit 1
            fi
            ;;
    esac
done

echo "Detected $JOBS CPU cores for building."

command -v cmake >/dev/null 2>&1 || { echo "cmake is required. Aborting." >&2; exit 1; }
command -v ninja >/dev/null 2>&1 || { echo "ninja is required. Aborting." >&2; exit 1; }

# Paths
BASE_INSTALL_DIR="$REPO_ROOT/opencv_artifacts/linux"
EXTERN_SOURCE="$REPO_ROOT/src/OpenCvSharpExtern"
FINAL_DIST="$REPO_ROOT/src/build/linux"

if [ -z "$VCPKG_INSTALLATION_ROOT" ]; then
    echo "Please set VCPKG_INSTALLATION_ROOT environment variable. Aborting."
    exit 1
fi
VCPKG_TOOLCHAIN="$VCPKG_INSTALLATION_ROOT/scripts/buildsystems/vcpkg.cmake"
VCPKG_INSTALLED_DIR="/repo/vcpkg_installed_linux"

ALL_TARGETS=(
    "Turing:7.5:7.5"
    "Ampere:8.6:8.6"
    "Ada:8.9:8.9"
    "Blackwell:10.0:10.0"
    "Combined:7.5,8.6,8.9,10.0:10.0"
)

# Filter targets if --build was specified
if [ -n "$BUILD_FILTER" ]; then
    echo ">>> Building only: $BUILD_FILTER"
    TARGETS=()
    IFS=',' read -ra REQUESTED <<< "$BUILD_FILTER"
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
    echo -e "\e[1;35m STARTING EXTERN BUILD: $NAME (sm_$ARCH)\e[0m"
    echo -e "\e[1;35m=======================================================\e[0m"

    BUILD_DIR_EX="$REPO_ROOT/src/build/linux/$NAME"
    INSTALL_DIR_ARCH="$BASE_INSTALL_DIR/$NAME"
    TARGET_FOLDER="$FINAL_DIST/$NAME"

    rm -rf "$BUILD_DIR_EX"
    mkdir -p "$TARGET_FOLDER"

    OPENCV_CONFIG_FILE=$(find "$INSTALL_DIR_ARCH" -name "OpenCVConfig.cmake" | head -n 1)
    if [ -z "$OPENCV_CONFIG_FILE" ]; then
        echo -e "\e[1;31mFailed to find OpenCVConfig.cmake in $INSTALL_DIR_ARCH. Has OpenCV been built for $NAME yet?\e[0m"
        exit 1
    fi
    OPENCV_CONFIG_PATH=$(dirname "$OPENCV_CONFIG_FILE")
    echo -e "\e[1;36m>>> Found OpenCVConfig.cmake at: $OPENCV_CONFIG_PATH\e[0m"

# --- PATH SETUP ---
# Ensure we are strictly using the Linux-specific vcpkg folder
LINUX_VCPKG_DIR="/$REPO_ROOT/vcpkg_installed_linux"
LINUX_TRIPLET="x64-linux-static"
LINUX_LIB_PATH="$LINUX_VCPKG_DIR/$LINUX_TRIPLET/lib"
LINUX_INCLUDE_PATH="$LINUX_VCPKG_DIR/$LINUX_TRIPLET/include"

# Combine all Linker Flags into one variable to prevent overwriting
# 1. -L forces the search path so -ltesseract is found
# 2. --exclude-libs,ALL ensures symbols aren't re-exported (smaller .so)
# 3. static-libgcc/stdc++ ensures Ubuntu 22/24 compatibility
SHARED_FLAGS="-L$LINUX_LIB_PATH -Wl,-Bsymbolic -static-libgcc -static-libstdc++"

echo ">>> Starting CMake Configuration with Path: $LINUX_VCPKG_DIR"

cmake -S "$EXTERN_SOURCE" -B "$BUILD_DIR_EX" -G "Ninja" \
      -D CMAKE_BUILD_TYPE=Release \
      -D ENABLED_CUDA=ON \
      -D OpenCV_DIR="$OPENCV_CONFIG_PATH" \
      -D CMAKE_TOOLCHAIN_FILE="$VCPKG_TOOLCHAIN" \
      -D VCPKG_TARGET_TRIPLET="$LINUX_TRIPLET" \
      -D VCPKG_MANIFEST_MODE=ON \
      -D VCPKG_MANIFEST_DIR="/repo" \
      -D VCPKG_INSTALLED_DIR="$LINUX_VCPKG_DIR" \
      -D VCPKG_OVERLAY_TRIPLETS="/repo/extern/OpenCvSharp/cmake/triplets" \
      -D CMAKE_PREFIX_PATH="$LINUX_VCPKG_DIR/$LINUX_TRIPLET" \
      -D CMAKE_LIBRARY_PATH="$LINUX_LIB_PATH" \
      -D CMAKE_INCLUDE_PATH="$LINUX_INCLUDE_PATH" \
      -D Tesseract_DIR="$LINUX_VCPKG_DIR/$LINUX_TRIPLET/share/tesseract" \
      -D CMAKE_SHARED_LINKER_FLAGS="$SHARED_FLAGS" \
      -D CMAKE_EXE_LINKER_FLAGS="$SHARED_FLAGS" \
      -D CMAKE_BUILD_WITH_INSTALL_RPATH=OFF \
      -D CMAKE_SKIP_RPATH=ON
    echo -e "\e[1;30m>>> Linking libOpenCvSharpExtern.so...\e[0m"
    cmake --build "$BUILD_DIR_EX" --config Release -j $JOBS

done

echo -e "\n\e[1;33mAll Extern Builds Complete! Check the '$FINAL_DIST' folder.\e[0m"