# OpenCvSharp Native Runtime Package

This is an **internal implementation package** for [OpenCvSharp](https://github.com/shimat/opencvsharp). It provides the native OpenCV shared library (`OpenCvSharpExtern`) for a specific platform.

## Package Versions & Architectures

Because CUDA binaries can become extremely large, this library is offered in several architecture-specific packages to save deployment space, as well as a "Combined" package that supports all modern GPUs. 

When installing, replace `{OS}` with either `win` `linux-x64`, depending on your target environment.

| Target Hardware | Package Naming Convention | SM Arch | Notes |
| :--- | :--- | :--- | :--- |
| **All Modern GPUs** | **`OpenCvSharp4.Cuda.runtime.{OS}`** | 7.5 - 10.0 | **Combined package.** Best for distribution to unknown hardware. Very large file size (~600MB+). |
| RTX 20-series | `OpenCvSharp4.Cuda.runtime.{OS}.Turing` | SM 7.5 | Optimized specifically for Turing. |
| RTX 30-series, A-series | `OpenCvSharp4.Cuda.runtime.{OS}.Ampere` | SM 8.6 | Optimized specifically for Ampere. |
| RTX 40-series, Ada generation | `OpenCvSharp4.Cuda.runtime.{OS}.Ada` | SM 8.9 | Optimized specifically for Ada Lovelace. |
| RTX 50-series, Blackwell | `OpenCvSharp4.Cuda.runtime.{OS}.Blackwell`| SM 10.0 | Optimized specifically for Blackwell. |

*Note: The architecture-specific packages include PTX code, allowing them to forward-compile (JIT) onto newer architectures at runtime. However, for the best performance and lowest startup latency, you should use the package that perfectly matches your target hardware.*




## Resources

- **GitHub Repository:** [TUmans/OpenCvSharp4.Cuda](https://github.com/TUmans/OpenCvSharp4.Cuda)
- **Base Project:** [shimat/opencvsharp](https://github.com/shimat/opencvsharp)