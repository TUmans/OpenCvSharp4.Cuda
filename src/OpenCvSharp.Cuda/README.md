# OpenCvSharp.Cuda

An extension for **OpenCvSharp** that provides .NET bindings for OpenCV's **CUDA** (GPU-accelerated) modules.

## Dependency on OpenCvSharp
This package is an extension of the base [OpenCvSharp](https://github.com/shimat/opencvsharp) library. It requires the core library for fundamental types and non-GPU operations. 

**Note:** When using this package, fundamental types like `Scalar`, `Size`, `Rect`, and `Point` are still accessed via the `OpenCvSharp` namespace.

## Supported Platforms

| Platform | Target Framework |
|---|---|
| .NET 8.0, 9.0, 10.0 | `net8.0`, `net9.0`, `net10.0` |
| .NET Standard 2.1 | `netstandard2.1` |

Target OpenCV version: **4.13.0** (with opencv_contrib)

Also compatible with docker, see dockerfile in github.

## GPU Architecture Builds

CUDA code is compiled for specific hardware generations. 
To keep installation sizes optimized, a dedidicated native runtime package is provided for each major NVIDIA architecture.
Ensure you install the one that matches your hardware:

*   **Turing**: (RTX 20-series, GTX 16-series, T4) - `runtime.win-x64.turing`
*   **Ampere**: (RTX 30-series, A-series) - `runtime.win-x64.ampere`
*   **Ada Lovelace**: (RTX 40-series, L4) - `runtime.win-x64.ada`
*   **Blackwell**: (RTX 50-series) - `runtime.win-x64.blackwell`
*   **Combined**: All-in-one universal package (largest size) - `runtime.win-x64.combined`

All packages are forward compatible. The Turing package will also work on Ampere or later.

## Quick Start


```bash
# 1. Add the base OpenCvSharp package
dotnet add package OpenCvSharp4

# 2. Add the CUDA managed extension
dotnet add package OpenCvSharp4.Cuda

# 3. Add the runtime for your specific GPU (e.g., RTX 40-series)
dotnet add package OpenCvSharp4.Cuda.runtime.win.ada
# or on Linux
dotnet add package OpenCvSharp4.Cuda.runtime.linux-x64.ada
```

## API Access

The library is structured to differentiate between global GPU functions and stateful GPU classes.

### 1. Functional Access via `Cv2Cuda`
Global CUDA-accelerated functions (arithmetic, bitwise, warping, color conversions) are located in the static `Cv2Cuda` class.

```csharp
// Absolute difference performed on the GPU
Cv2Cuda.AbsDiff(gpuMat1, gpuMat2, gpuResult);

// Thresholding on the GPU
Cv2Cuda.Threshold(gpuSrc, gpuDst, 128, 255, ThresholdTypes.Binary);
```

### 2. Class Access via `Cuda.{Class}`
Stateful algorithms like Filters, Feature Detectors, and Background Subtractors are found in the `OpenCvSharp.Cuda` namespace.

```csharp
// Creating a Sobel filter object on the GPU
using var filter = Cuda.Filter.CreateSobelFilter(MatType.CV_8UC1, MatType.CV_8UC1, 1, 0);
filter.Apply(gpuSrc, gpuDst);

// Creating a MOG2 background subtractor on the GPU
using var mog2 = Cuda.BackgroundSubtractorMOG2.Create();
```

## Mixing CPU and GPU Code

A typical workflow involves using the standard library for I/O and the CUDA library for processing.

```csharp
using OpenCvSharp;
using OpenCvSharp.Cuda;

// Load image via standard OpenCvSharp (CPU)
using var cpuSrc = new Mat("input.jpg", ImreadModes.Grayscale);

// Upload to GPU
using var gpuSrc = new GpuMat();
gpuSrc.Upload(cpuSrc);

// Process using Cv2Cuda (GPU)
using var gpuBlurred = new GpuMat();
using var filter = Cuda.Filter.CreateGaussianFilter(gpuSrc.Type(), gpuSrc.Type(), new Size(5, 5), 1.5);
filter.Apply(gpuSrc, gpuBlurred);

// Download back to standard Mat (CPU)
using var cpuResult = new Mat();
gpuBlurred.Download(cpuResult);
```


## Type Safety: `InputArray` vs. `CudaInputArray`

This wrapper aims to be as truthfull to the source material as possible, but an exception was made for InputArray.

In the native OpenCV c++ API, both CPU and GPU functions use the same `cv::_InputArray` proxy. 
While flexible, this is a common source of bugs: passing a CPU-based `cv::Mat` to a CUDA function will compile perfectly but will cause a **segmentation fault or a heavy crash at runtime** because the GPU cannot access CPU-mapped memory addresses.

To provide a better developer experience, these types were seperated.

*   **`OpenCvSharp.InputArray`**: Used for standard **CPU** operations (accepts `Mat`, `Scalar`, etc.).
*   **`OpenCvSharp.Cuda.CudaInputArray`**: Used exclusively for **GPU** operations. It only accepts types that are physically compatible with the GPU.

Keep this in mind when studying opencv examples from python or c++.

### Example
```csharp
using OpenCvSharp;
using OpenCvSharp.Cuda;

var cpuMat = new Mat("image.jpg", ImreadModes.Grayscale);
var gpuMat = new GpuMat();
gpuMat.Upload(cpuMat);

// ERROR: This will not compile. 
// Protects you from sending CPU memory to a GPU kernel.
Cv2Cuda.BitwiseNot(cpuMat, gpuMat); 

// SUCCESS: Correct type and memory location.
Cv2Cuda.BitwiseNot(gpuMat, gpuMat); 
```

### Namespace Ambiguity
Because both `OpenCvSharp` and `OpenCvSharp.Cuda` contain types that act as input proxies, we recommend using a namespace alias if you are mixing CPU and GPU code in the same file:

```csharp
using Cuda = OpenCvSharp.Cuda;

// Use standard InputArray for CPU
Cv2.GaussianBlur(src, dst, new Size(3,3), 0);

// Use CudaInputArray (via GpuMat) for GPU
Cuda.Cv2Cuda.BitwiseNot(gpuSrc, gpuDst);
```

---

### How to integrate this into your existing README:
You can place this section right after the **API Design** or **Usage** section. It provides a technical justification for why your wrapper is "smarter" than a basic 1:1 port.
## Requirements

*   **NVIDIA GPU**: Turing architecture or newer (Compute Capability 5.0+).
*   **Drivers**: Latest NVIDIA Display Drivers.
*   **Windows**: [Visual C++ 2022 Redistributable](https://support.microsoft.com/help/2977003/).
*   **Linux**: NVIDIA Container Toolkit (for Docker) or local installation of the CUDA runtime.

### **Linux (Ubuntu/Debian) Runtime Dependencies**
To run an OpenCvSharp.Cuda application on Linux, you must have the CUDA runtime and OpenCV dependencies installed:

**1. OpenCV & GUI Dependencies:**
```bash
apt-get update && apt-get install -y \
    libgomp1 libglib2.0-0 libsm6 libice6 libx11-6 libxext6 libxrender1 \
    libfontconfig1 libfreetype6 libharfbuzz0b \
    libjpeg-turbo8 libpng16-16 libtiff6 libwebp7 \
    libgdiplus libatomic1
```

**2. NVIDIA CUDA Runtime (Version 12.8):**
*Note: Ensure your driver version matches the CUDA toolkit version.*
```bash
apt-get install -y \
    cuda-cudart-12-8 \
    libcublas-12-8 \
    libcufft-12-8 \
    libcurand-12-8 \
    libcusolver-12-8 \
    libcusparse-12-8 \
    libnpp-12-8 \
    libcudnn9-cuda-12
```

*   **`LD_LIBRARY_PATH`**: Don't forget to set `export LD_LIBRARY_PATH=/usr/local/cuda/lib64:$LD_LIBRARY_PATH`.
*   **NVIDIA Container Toolkit:** When using NVIDIA's cuda docker, ignore step 2.

## Resources

- [OpenCV CUDA Documentation](https://docs.opencv.org/4.x/d1/d1e/group__cuda.html)
- [OpenCvSharp GitHub](https://github.com/shimat/opencvsharp)
- [Issue Tracker](https://github.com/TUmans/OpenCvSharp4.Cuda/issues)