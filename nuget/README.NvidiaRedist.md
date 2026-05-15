# OpenCvSharp4.Cuda.NvidiaRedist.win

This package provides the native **NVIDIA CUDA and cuDNN redistributable DLLs** required to run GPU-accelerated OpenCvSharp applications on Windows x64.

By including this package in your project, you enable a **"Zero-Install" deployment** for your end-users. They will not need to manually download or install the massive 3GB+ NVIDIA CUDA Toolkit on their machines; the required libraries will simply copy to your application's output directory.

## What is included?

This package bundles the Windows x64 DLLs for:
- **CUDA Toolkit:** 12.8 (e.g., `cudart64_12.dll`, `cublas64_12.dll`, `cufft64_11.dll`)
- **cuDNN:** 9.2.0 (Required for Deep Neural Network / DNN module acceleration)

## How to Use

This package **does not** contain OpenCV itself. It is meant to be used alongside the OpenCvSharp C# wrapper and your chosen CUDA architecture runtime package. 

To fully enable CUDA-accelerated OpenCV in your project, install the following three packages:

1.  **The C# Wrapper:** `OpenCvSharp4`
2.  **The OpenCV Native C++ Binaries:** `OpenCvSharp4.Cuda.runtime.win` *(or a specific architecture variant like `.Ada`)*
3.  **The NVIDIA Dependencies (This Package):** `OpenCvSharp4.Cuda.NvidiaRedist.win`

## Hardware & Software Requirements

Even with this redistributable package, the target machine must still meet the following requirements:
1.  **Compatible GPU:** A modern NVIDIA RTX or Datacenter GPU.
2.  **NVIDIA Display Driver:** Version **566.03 or newer** is strictly required to run CUDA 12.8 applications.
3.  **Project Architecture:** Your .NET project must explicitly target `x64`.

## Licensing & Legal

While OpenCvSharp and this packaging project are licensed under Apache-2.0, the native `.dll` files included in this package are proprietary property of NVIDIA Corporation. 

By utilizing this package, you agree to the terms set forth in the [NVIDIA CUDA Toolkit End User License Agreement (EULA)](https://docs.nvidia.com/cuda/archive/12.8.0/eula/index.html) and the [cuDNN Software License Agreement](https://docs.nvidia.com/deeplearning/cudnn/backend/v9.20.0/reference/eula.html).
License files are also included in the package.

## Resources
- **GitHub Repository:** [TUmans/OpenCvSharp4.Cuda](https://github.com/TUmans/OpenCvSharp4.Cuda)
- **Base Project:** [shimat/opencvsharp](https://github.com/shimat/opencvsharp)