# OpenCvSharp4.Cuda

> [!NOTE]
> THIS PROJECT IS STILL WIP.
>
> TODO
> Add Nvidia Libraries
> Test Linux
> Create nuget packages

OpenCvSharp.Cuda is a cross-platform .NET wrapper for OpenCV. It is an extension on the existing opencvsharp created by shimat.
This project adds OpenCV cuda functions to OpenCvSharp.


All requirements from the original project apply here as wel: [The original OpenCv Readme](https://github.com/shimat/opencvsharp)

# OpenCvSharp.Cuda Build Instructions
## Prerequisite

* **Imporant** : Install latest nvidia driver for your GPU. 
* install visual studio with module "desktop c++ development"
* install cmake
* install git?


We also need the relevant DLL's. You can download them at:

* Windows
	* Nvidia CUDA 12.8 Dlls. [Download](https://developer.nvidia.com/cuda-12-8-0-download-archive)
	* Nvidia CuDnn 9.20 Dlls. [Download](https://developer.nvidia.com/cudnn-9-2-0-download-archive)
	* Nvidia video codex 13.0.37.  [Download](https://developer.nvidia.com/video-codec-sdk#section-get-started)
* Linux
	* WIP






# OpenCV Cuda Notes

## Stream

[see OpenCV docs](https://docs.opencv.org/4.x/d9/df3/classcv_1_1cuda_1_1Stream.html)
> [!WARNING] 
>
>   Currently, you may face problems if an operation is enqueued twice with different data. Some functions use the constant GPU memory, and next call may update the memory before the previous one has been finished. But calling different operations asynchronously is safe because each operation has its own constant buffer. Memory copy/upload/download/set operations to the buffers you hold are also safe.
>   The Stream class is not thread-safe. Please use different Stream objects for different CPU threads.

> [!WARNING] 
>
>   By default all CUDA routines are launched in Stream::Null() object, if the stream is not specified by user. In multi-threading environment the stream objects must be passed explicitly (see previous note). 


