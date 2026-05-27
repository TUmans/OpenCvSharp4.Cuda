# OpenCvSharp4.Cuda.runtime (Windows & Linux)

This project provides the native C++ binaries (`OpenCvSharpExtern.dll` for Windows, `libOpenCvSharpExtern.so` for Linux) and required dependencies to run **OpenCvSharp4** with GPU acceleration on x64 platforms.

It is a drop-in replacement for the standard CPU-only `OpenCvSharp4.runtime.*` packages, specifically compiled to enable CUDA-accelerated image processing and Deep Neural Network (DNN) inference.

## Package Versions & Architectures

Because CUDA binaries can become extremely large, this library is offered in several architecture-specific packages to save deployment space, as well as a "Combined" package that supports all modern GPUs. 

When installing, replace `{OS}` with either `win` or your specific Linux moniker (e.g., `linux`, `ubuntu.22.04`), depending on your target environment.

| Target Hardware | Package Naming Convention | SM Arch | Notes |
| :--- | :--- | :--- | :--- |
| **All Modern GPUs** | **`OpenCvSharp4.Cuda.runtime.{OS}`** | 7.5 - 10.0 | **Combined package.** Best for distribution to unknown hardware. Very large file size (~600MB+). |
| RTX 20-series | `OpenCvSharp4.Cuda.runtime.{OS}.Turing` | SM 7.5 | Optimized specifically for Turing. |
| RTX 30-series, A-series | `OpenCvSharp4.Cuda.runtime.{OS}.Ampere` | SM 8.6 | Optimized specifically for Ampere. |
| RTX 40-series, Ada generation | `OpenCvSharp4.Cuda.runtime.{OS}.Ada` | SM 8.9 | Optimized specifically for Ada Lovelace. |
| RTX 50-series, Blackwell | `OpenCvSharp4.Cuda.runtime.{OS}.Blackwell`| SM 10.0 | Optimized specifically for Blackwell. |

*Note: The architecture-specific packages include PTX code, allowing them to forward-compile (JIT) onto newer architectures at runtime. However, for the best performance and lowest startup latency, you should use the package that perfectly matches your target hardware.*

## Build Specifications

- **OpenCV Version:** 4.13.0
- **CUDA Toolkit:** 12.8 
  - *Windows:* See the companion package `OpenCvSharp4.Cuda.NvidiaRedist.win` for zero-install redistributables.
  - *Linux:* You must ensure the host environment or Docker container has the CUDA 12.8 runtime libraries installed.
- **cuDNN:** 9.2.0 (Included in build config for DNN acceleration)
- **Linkage:** OpenCV C++ modules are statically linked into the wrapper to reduce file clutter and dependency chains.

## Hardware & Software Requirements

1.  **NVIDIA Driver:** Version **566.03 or higher** is strictly required for CUDA 12.8 support (both Windows and Linux).
2.  **Operating System:** 
    - Windows 10 or Windows 11 (x64)
    - Linux (Ubuntu 20.04/22.04/24.04, Debian, etc.) (x64)
3.  **Linux System Dependencies:** On Linux, your environment must have standard OpenCV dependencies installed (e.g., `libgl1`, `libglib2.0-0`, `libgomp1`).
4.  **Project Config:** Your .NET project must explicitly target `x64`. ("Any CPU" will not work correctly when loading native x64 binaries).

## Installation

1.  **(If installed) Remove the standard CPU-only runtime package** from your project:
    ```bash
    dotnet remove package OpenCvSharp4.runtime.win
    # or
    dotnet remove package OpenCvSharp4.runtime.ubuntu.22.04-x64
    ```

2.  **Install ONE of the CUDA packages** based on your OS and deployment needs:
    
    *Example: To install the Combined fat-binary for Windows:*
    ```bash
    dotnet add package OpenCvSharp4.Cuda.runtime.win
    ```
    
    *Example: To install an architecture-specific binary for Linux (e.g., Ada / RTX 40-series):*
    ```bash
    dotnet add package OpenCvSharp4.Cuda.runtime.linux.Ada
    ```

3.  *(Windows Only)* **Install the NVIDIA redistributables** (highly recommended):
    ```bash
    dotnet add package OpenCvSharp4.Cuda.NvidiaRedist.win
    ```

4.  Ensure your application's Build Platform is explicitly set to `x64`.

## Resources

- **GitHub Repository:** [TUmans/OpenCvSharp4.Cuda](https://github.com/TUmans/OpenCvSharp4.Cuda)
- **Base Project:** [shimat/opencvsharp](https://github.com/shimat/opencvsharp)
- **Issue Tracker:** [Report a bug](https://github.com/TUmans/OpenCvSharp4.Cuda/issues)

## Credits
This build is based on the phenomenal work of **shimat** and the OpenCvSharp contributors. This specific fork/addition adds the build configurations, cuDNN integration, multi-architecture packaging, and Linux support required for modern NVIDIA GPU acceleration.