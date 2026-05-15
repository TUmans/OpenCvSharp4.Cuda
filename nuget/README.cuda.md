# OpenCvSharp4.Cuda.runtime.win

This project provides native C++ binaries (`OpenCvSharpExtern.dll`) and required dependencies to run **OpenCvSharp4** with GPU acceleration on Windows x64.

It is a drop-in replacement for the standard `OpenCvSharp4.runtime.win` package, specifically compiled to enable CUDA-accelerated image processing and Deep Neural Network (DNN) inference.

## Package Versions

Because CUDA binaries can become extremely large, this library is offered in several architecture-specific packages to save deployment space, as well as a "Combined" package that supports all modern GPUs.

| Package Name | Architecture | Target Hardware | Notes |
| :--- | :--- | :--- | :--- |
| **`OpenCvSharp4.Cuda.runtime.win`** | **Combined** (7.5, 8.6, 8.9, 10.0) | **Any modern RTX GPU** | Best for distribution to unknown hardware. Very large file size (~600MB+). |
| `OpenCvSharp4.Cuda.runtime.win.Turing` | SM 7.5 | RTX 20-series, GTX 16-series | Optimized for Turing. |
| `OpenCvSharp4.Cuda.runtime.win.Ampere` | SM 8.6 | RTX 30-series, A-series workstations | Optimized for Ampere. |
| `OpenCvSharp4.Cuda.runtime.win.Ada` | SM 8.9 | RTX 40-series | Optimized for Ada Lovelace. |
| `OpenCvSharp4.Cuda.runtime.win.Blackwell`| SM 10.0 | RTX 50-series | Optimized for Blackwell. |

*Note: The architecture-specific packages include PTX code, allowing them to forward-compile (JIT) onto newer architectures at runtime. However, for the best performance and lowest startup latency, you should use the package that matches your target hardware.*

## Build Specifications

- **OpenCV Version:** 4.13.0
- **CUDA Toolkit:** 12.8 ( see OpenCvSharp4.Cuda.NvidiaRedist.win, or provide yourself)
- **cuDNN:** 9.2.0 (Included in build config)
- **Linkage:** OpenCV modules are statically linked into the wrapper to reduce file clutter.

## Software Requirements

1.  **NVIDIA Driver:** Version **566.03 or higher** is strictly required for CUDA 12.8 support.
2.  **OS:** Windows 10 or Windows 11 (x64).
3.  **Project Config:** Your .NET project must target `x64` (Any CPU will not work for native C++ bindings).

## Installation

1.  (If installed) Remove the standard CPU-only package from your project if you have it installed:
    ```bash
    dotnet remove package OpenCvSharp4.runtime.win
    ```
2.  Install **ONE** of the CUDA packages based on your deployment needs. For example:
    *To install the Combined fat-binary:*
    ```bash
    dotnet add package OpenCvSharp4.Cuda.runtime.win
    ```
    *To install an architecture-specific binary (e.g., Ada / RTX 40-series):*
    ```bash
    dotnet add package OpenCvSharp4.Cuda.runtime.win.Ada
    ```
3.  Ensure your application's Build Platform is explicitly set to `x64`.

## Resources

- **GitHub Repository:** [TUmans/OpenCvSharp4.Cuda](https://github.com/TUmans/OpenCvSharp4.Cuda)
- **Base Project:** [shimat/opencvsharp](https://github.com/shimat/opencvsharp)
- **Issue Tracker:** [Report a bug](https://github.com/TUmans/OpenCvSharp4.Cuda/issues)

## Credits
This build is based on the phenomenal work of **shimat** and the OpenCvSharp contributors. This specific fork/addition adds the build configurations, cuDNN integration, and multi-architecture packaging required for modern NVIDIA GPU acceleration.
