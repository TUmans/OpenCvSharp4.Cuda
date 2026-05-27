# OpenCvSharp4.Cuda.NvidiaRedist.win

This package provides the native **NVIDIA CUDA and cuDNN redistributable DLLs** required to run GPU-accelerated OpenCvSharp applications on Windows x64.

## Package Structure (Meta-Package)

Due to NuGet's maximum file size restrictions, the NVIDIA binaries had to be divided into two separate parts. **This package acts as a meta-package** that automatically handles downloading both. 

When you install this package, NuGet will automatically pull in the following dependencies:
- **`...NvidiaRedist.win.Core`**: Contains the core runtime and deep learning libraries (`cudart`, `npp`, `cudnn`).
- **`...NvidiaRedist.win.Compute`**: Contains the heavy mathematical and compute libraries (`cublas`, `cufft`, `curand`, `cusolver`, `cusparse`).

*(Bundles CUDA Toolkit version **12.8** and cuDNN version **9.2.0**)*

## How to Use

This package **does not** contain OpenCV itself. It is meant to be used alongside the OpenCvSharp C# wrapper and your chosen CUDA architecture runtime package. 

To fully enable CUDA-accelerated OpenCV in your project, you only need to install the following three packages:

1.  **The C# Wrapper:** `OpenCvSharp4`
2.  **The OpenCV Native C++ Binaries:** `OpenCvSharp4.Cuda.runtime.win` *(or a specific architecture variant like `.Ada`)*
3.  **The NVIDIA Dependencies (This Meta-Package):** `OpenCvSharp4.Cuda.NvidiaRedist.win`

*Note: You do not need to install the `Core` and `Compute` packages manually. Installing this meta-package will automatically handle them.*

## Hardware & Software Requirements

Even with this redistributable package, the target machine must still meet the following requirements:
1.  **Compatible GPU:** A modern NVIDIA RTX or Datacenter GPU.
2.  **NVIDIA Display Driver:** Version **566.03 or newer** is strictly required to run CUDA 12.8 applications.
3.  **Project Architecture:** Your .NET project must explicitly target `x64`.

## Licensing & Legal

While OpenCvSharp and this packaging project are licensed under Apache-2.0, the native `.dll` files included in these packages are the proprietary property of NVIDIA Corporation. 

By utilizing this package, you agree to the terms set forth in the [NVIDIA CUDA Toolkit End User License Agreement (EULA)](https://docs.nvidia.com/cuda/archive/12.8.0/eula/index.html) and the [cuDNN Software License Agreement](https://docs.nvidia.com/deeplearning/cudnn/backend/v9.20.0/reference/eula.html). License files are also included within the package directories.

## Resources
- **GitHub Repository:** [TUmans/OpenCvSharp4.Cuda](https://github.com/TUmans/OpenCvSharp4.Cuda)
- **Base Project:** [shimat/opencvsharp](https://github.com/shimat/opencvsharp)