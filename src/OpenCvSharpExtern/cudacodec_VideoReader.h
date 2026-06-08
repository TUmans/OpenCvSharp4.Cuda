#pragma once

// -----------------------------------------------------------------------
// OpenCvSharpExtern – cv::cuda arithmetic wrappers
// These are the C-linkage functions that the C# P/Invoke layer calls.
// Each function catches cv::Exception, stores it, and returns an
// ExceptionStatus so managed code can rethrow it as a .NET exception.
// -----------------------------------------------------------------------

#include "include_opencv.h"
#include <opencv2/cudacodec.hpp>

CVAPI(ExceptionStatus) cuda_createVideoReader1(cv::Ptr<cv::cudacodec::RawVideoSource>* source, cv::cudacodec::VideoReaderInitParams params,cv::Ptr<cv::cudacodec::VideoReader>** returnValue)
{
    BEGIN_WRAP
        auto ptr = cv::cudacodec::createVideoReader(*source, params);
    *returnValue = new cv::Ptr<cv::cudacodec::VideoReader>(ptr);
    END_WRAP
}


CVAPI(ExceptionStatus) cuda_createVideoReader2( const char* filename, int* sourceParams, int sourceParamsSize, cv::cudacodec::VideoReaderInitParams params, cv::Ptr<cv::cudacodec::VideoReader>** returnValue)
{
    BEGIN_WRAP
        std::vector<int> sParams;
    if (sourceParams && sourceParamsSize > 0)
        sParams.assign(sourceParams, sourceParams + sourceParamsSize);

    auto ptr = cv::cudacodec::createVideoReader(filename, sParams, params);
    *returnValue = new cv::Ptr<cv::cudacodec::VideoReader>(ptr);
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_get1(cv::Ptr<cv::cudacodec::VideoReader>* ptr, cv::cudacodec::VideoReader** returnValue)
{
    BEGIN_WRAP
        * returnValue = ptr->get();
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_delete(cv::Ptr<cv::cudacodec::VideoReader>* ptr)
{
    BEGIN_WRAP
    delete ptr;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_format(cv::cudacodec::VideoReader* obj, cv::cudacodec::FormatInfo* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->format();
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_get2(cv::cudacodec::VideoReader* obj, int propertyId, double* propertyVal, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->get(propertyId, *propertyVal) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_getProp(cv::cudacodec::VideoReader* obj, int propertyId, double* propertyVal, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->get(static_cast<cv::cudacodec::VideoReaderProps>(propertyId), *propertyVal) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_getVideoReaderProps(cv::cudacodec::VideoReader* obj, int propertyId, double* propertyValOut, double propertyValIn, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->getVideoReaderProps(static_cast<cv::cudacodec::VideoReaderProps>(propertyId), *propertyValOut, propertyValIn) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_grab(cv::cudacodec::VideoReader* obj, cv::cuda::Stream* stream, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->grab(stream ? *stream : cv::cuda::Stream::Null()) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_nextFrame(cv::cudacodec::VideoReader* obj, cv::cuda::GpuMat* frame, cv::cuda::Stream* stream, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->nextFrame(*frame, stream ? *stream : cv::cuda::Stream::Null()) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_nextFrame_withHist(cv::cudacodec::VideoReader* obj, cv::cuda::GpuMat* frame, cv::cuda::GpuMat* histogram, cv::cuda::Stream* stream, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->nextFrame(*frame, *histogram, stream ? *stream : cv::cuda::Stream::Null()) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_retrieve(cv::cudacodec::VideoReader* obj, cv::_OutputArray* frame, size_t idx, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->retrieve(*frame, idx) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_set(cv::cudacodec::VideoReader* obj, int colorFormat, int bitDepth, int planar, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->set(static_cast<cv::cudacodec::ColorFormat>(colorFormat),
            static_cast<cv::cudacodec::BitDepth>(bitDepth),
            planar != 0) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_VideoReader_setVideoReaderProps(cv::cudacodec::VideoReader* obj, int propertyId, double propertyVal, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->set(static_cast<cv::cudacodec::VideoReaderProps>(propertyId), propertyVal) ? 1 : 0;
    END_WRAP
}
