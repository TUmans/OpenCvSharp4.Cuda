// Additional functions

#pragma once

#ifdef _WIN32
#pragma warning(disable: 4996) 
#endif

#include "include_opencv.h"
#include <opencv2/opencv.hpp>
#include <opencv2/core/cuda.hpp>


static cv::cuda::GpuMat entity(cv::cuda::GpuMat* obj)
{
    return (obj != nullptr) ? *obj : cv::cuda::GpuMat();
}

#pragma region cv::cuda::GpuMat
CVAPI(std::vector<cv::cuda::GpuMat>*) vector_GpuMat_new1()
{
    return new std::vector<cv::cuda::GpuMat>;
}
CVAPI(std::vector<cv::cuda::GpuMat>*) vector_GpuMat_new2(uint32_t size)
{
    return new std::vector<cv::cuda::GpuMat>(size);
}
CVAPI(std::vector<cv::cuda::GpuMat>*) vector_GpuMat_new3(cv::cuda::GpuMat** data, uint32_t dataLength)
{
    auto* vec = new std::vector<cv::cuda::GpuMat>(dataLength);
    for (size_t i = 0; i < dataLength; i++)
    {
        (*vec)[i] = *(data[i]);
    }
    return vec;
}

CVAPI(size_t) vector_GpuMat_getSize(std::vector<cv::cuda::GpuMat>* vector)
{
    return vector->size();
}

CVAPI(cv::cuda::GpuMat*) vector_GpuMat_getPointer(std::vector<cv::cuda::GpuMat>* vector)
{
    return &(vector->at(0));
}

CVAPI(void) vector_GpuMat_assignToArray(std::vector<cv::cuda::GpuMat>* vector, cv::cuda::GpuMat** arr)
{
    for (size_t i = 0; i < vector->size(); i++)
    {
        (vector->at(i)).assignTo(*(arr[i]));
    }
}

CVAPI(void) vector_GpuMat_delete(std::vector<cv::cuda::GpuMat>* vector)
{
    // vector->~vector();
    delete vector;
}
#pragma endregion
#pragma region ulong
CVAPI(std::vector<uint64_t>*) vector_uint64_new1()
{
    return new std::vector<uint64_t>();
}

CVAPI(std::vector<uint64_t>*) vector_uint64_new2(size_t size)
{
    return new std::vector<uint64_t>(size);
}

CVAPI(std::vector<uint64_t>*) vector_uint64_new3(uint64_t* data, size_t size)
{
    return new std::vector<uint64_t>(data, data + size);
}

CVAPI(void) vector_uint64_delete(std::vector<uint64_t>* v)
{
    delete v;
}

CVAPI(size_t) vector_uint64_getSize(std::vector<uint64_t>* v)
{
    return v->size();
}

CVAPI(uint64_t*) vector_uint64_getPointer(std::vector<uint64_t>* v)
{
    return v->data();
}
#pragma endregion