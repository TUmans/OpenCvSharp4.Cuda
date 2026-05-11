// Additional functions

#pragma once

#ifdef _WIN32
#pragma warning(disable: 4996) 
#endif

#include <opencv2/opencv.hpp>
#include <opencv2/core/cuda.hpp>


static cv::cuda::GpuMat entity(cv::cuda::GpuMat* obj)
{
    return (obj != nullptr) ? *obj : cv::cuda::GpuMat();
}