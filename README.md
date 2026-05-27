# OpenCvSharp4.Cuda

OpenCvSharp.Cuda is a cross-platform .NET extension for [OpenCvSharp](https://github.com/shimat/opencvsharp) by Shimat. It provides comprehensive .NET bindings for OpenCV's CUDA modules, enabling GPU-accelerated image processing and computer vision.

## Features

### 1. Functional Access via `Cv2Cuda`
Global GPU-accelerated functions (arithmetic, warping, color conversions) are located in the static `Cv2Cuda` class.
```csharp
// Absolute difference performed on the GPU
Cv2Cuda.AbsDiff(gpuMat1, gpuMat2, gpuResult);
```

### 2. Class Access via `Cuda.(class)`
Stateful algorithms such as Filters, Feature Detectors (ORB, FAST), and Background Subtractors are instantiated through the `Cuda` namespace.
```csharp
// Create a GPU-based Gaussian Filter
using var filter = Cuda.Filter.CreateGaussianFilter(MatType.CV_8UC1, MatType.CV_8UC1, new Size(3, 3), 0.5);
filter.Apply(gpuSrc, gpuDst);
```

### 3. Safety: `CudaInputArray`
Unlike the C++ API, this library differentiates between CPU and GPU memory at compile-time.
* **Standard `InputArray`**: Used for CPU/Mat operations.
* **`CudaInputArray`**: Used exclusively for GPU/GpuMat operations.

This prevents common mistakes, such as accidentally passing a CPU `Mat` to a GPU kernel, which would cause a crash at runtime.


## GPU Architecture Builds

CUDA binaries are hardware-generation specific. We provide dedicated native runtime packages for major NVIDIA architectures to optimize performance and download size:

| Architecture | Series | Runtime Suffix |
| :--- | :--- | :--- |
| **Turing** | RTX 20-series, GTX 16-series, T4 | `.turing` |
| **Ampere** | RTX 30-series, A-series | `.ampere` |
| **Ada Lovelace** | RTX 40-series, L4 | `.ada` |
| **Blackwell** | RTX 50-series | `.blackwell` |
| **Combined** | Universal (Includes all of the above) | No suffix |

Support is provided for windows (x64) and linux (x64) builds.

## Requirements
### Runtime
#### Windows Prerequisites

1.  **NVIDIA GPU**: Maxwell architecture or newer.
2.  **Latest NVIDIA Drivers**: Version **525.xx** or higher is required for CUDA 12.8 compatibility.
3.  **Visual C++ 2022 Redistributable**: [Download here](https://aka.ms/vs/17/release/vc_redist.x64.exe).
4.  **No Toolkit Required**: Install nuget package : OpenCvSharp4.Cuda.NvidiaRedist.win.12.8.0

**Bundled NVIDIA Libraries**

Your installation will automatically include the following high-performance libraries:
*   `cudart64_12.dll` (Core Runtime)
*   `npp*.dll` (Performance Primitives - used for most image math)
*   `cufft64_11.dll` (Fast Fourier Transforms)
*   `cublas64_12.dll` (Linear Algebra)
*   `cudnn64_9.dll` (Deep Learning support)
  
#### Linux (Ubuntu/Debian) Requirements
The native libraries require the following system dependencies:

**1. OpenCV & GUI Dependencies:**
```bash
apt-get install -y libgomp1 libglib2.0-0 libsm6 libice6 libx11-6 libxext6 libxrender1 \
    libfontconfig1 libfreetype6 libharfbuzz0b libjpeg-turbo8 libpng16-16 libtiff6 libwebp7 \
    libtesseract5 tesseract-ocr ffmpeg libatomic1 libgdiplus ca-certificates libgtk2.0-0 \
    libwebp-dev wget
```

**2. NVIDIA CUDA Runtime (v12.8):**
```bash
wget https://developer.download.nvidia.com/compute/cuda/repos/ubuntu2404/x86_64/cuda-keyring_1.1-1_all.deb \
&& dpkg -i cuda-keyring_1.1-1_all.deb \
&& rm cuda-keyring_1.1-1_all.deb \
&& apt-get update && apt-get install -y --no-install-recommends cuda-cudart-12-8 libcublas-12-8 libcufft-12-8  \
    libcurand-12-8 libcusolver-12-8 libcusparse-12-8 libnpp-12-8 libcudnn9-cuda-12 \
&& rm -rf /var/lib/apt/lists/*
```

### Build
#### Windows Prerequisites
* **Drivers**: Latest NVIDIA Display Driver.
* **Compiler**: Visual Studio 2022 with "Desktop development with C++".
* **Tools**: CMake, Git.
* **NVIDIA SDKs**:
    * [CUDA Toolkit 12.8](https://developer.nvidia.com/cuda-12-8-0-download-archive)
    * [cuDNN 9.x](https://developer.nvidia.com/cudnn-9-2-0-download-archive)
    * [Video Codec SDK 13.0+](https://developer.nvidia.com/video-codec-sdk)

#### Linux (Ubuntu/Debian) Requirements
The native libraries require the following system dependencies:

**1. OpenCV & GUI Dependencies:**
```bash
apt-get update && apt-get install -y build-essential cmake ninja-build git \
    curl zip unzip tar pkg-config nasm libtesseract-dev libleptonica-dev libicu74 \
    wget ca-certificates libavcodec-dev libavformat-dev libavutil-dev libswscale-dev \
    libavdevice-dev libgtk2.0-dev
```

**2. NVIDIA CUDA Runtime (v12.8):**
```bash
wget https://developer.download.nvidia.com/compute/cuda/repos/ubuntu2404/x86_64/cuda-keyring_1.1-1_all.deb \
&& dpkg -i cuda-keyring_1.1-1_all.deb 
&& rm cuda-keyring_1.1-1_all.deb \
&& apt-get update && apt-get install -y cuda-toolkit-12-8 libcudnn9-dev-cuda-12 \
&& rm -rf /var/lib/apt/lists/*
```

For more detailed instructions, see docker files.

## Example Usage

```csharp
using OpenCvSharp;
using OpenCvSharp.Cuda; // Managed extension
using Cuda = OpenCvSharp.Cuda; // Recommended alias for class access

// 1. Upload to GPU
using var cpuSrc = new Mat("test.jpg", ImreadModes.Grayscale);
using var gpuSrc = new GpuMat();
gpuSrc.Upload(cpuSrc);

// 2. Apply a Filter (GPU)
using var gpuDst = new GpuMat();
using var filter = Cuda.Filter.CreateSobelFilter(gpuSrc.Type(), gpuSrc.Type(), 1, 0);
filter.Apply(gpuSrc, gpuDst);

// 3. Download to CPU
using var cpuResult = new Mat();
gpuDst.Download(cpuResult);
```


## CUDA Stream Usage

[OpenCV Stream Documentation](https://docs.opencv.org/4.x/d9/df3/classcv_1_1cuda_1_1Stream.html)

> [!WARNING] 
> **Data Race Risk**: Currently, you may face problems if an operation is enqueued twice with different data. Some functions use constant GPU memory; a subsequent call may update that memory before the previous one has finished. Calling *different* operations asynchronously is safe.

> [!WARNING] 
> **Thread Safety**: The `Stream` class is **not thread-safe**. Use unique `Stream` objects for different CPU threads. By default, all CUDA routines are launched in `Stream::Null()` (synchronous) if no stream is specified.


## OpenCvSharp
This project is an extension. You **must** have the base `OpenCvSharp4` package installed. Core types like `Scalar`, `Size`, `Rect`, and `Point` are shared between both libraries.

When installing OpenCv nuget packages with CUDA support, make sure to remove OpenCvSharp4.runtime.win. OpenCvSharp4.Cuda.runtime.win replaces this.



*   [Original OpenCvSharp Repository](https://github.com/shimat/opencvsharp)
*   [OpenCV CUDA Module Documentation](https://docs.opencv.org/4.x/d1/d1e/group__cuda.html)