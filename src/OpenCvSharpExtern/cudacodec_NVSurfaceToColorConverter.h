#pragma once

// -----------------------------------------------------------------------
// OpenCvSharpExtern – cv::cuda arithmetic wrappers
// These are the C-linkage functions that the C# P/Invoke layer calls.
// Each function catches cv::Exception, stores it, and returns an
// ExceptionStatus so managed code can rethrow it as a .NET exception.
// -----------------------------------------------------------------------

#include "include_opencv.h"
#include <opencv2/cudacodec.hpp>

CVAPI(ExceptionStatus) cuda_createNVSurfaceToColorConverter(int colorSpace, int videoFullRangeFlag, cv::Ptr<cv::cudacodec::NVSurfaceToColorConverter>** returnValue)
{
    BEGIN_WRAP
        auto ptr = cv::cudacodec::createNVSurfaceToColorConverter(
            static_cast<cv::cudacodec::ColorSpaceStandard>(colorSpace),
            videoFullRangeFlag != 0);
    *returnValue = new cv::Ptr<cv::cudacodec::NVSurfaceToColorConverter>(ptr);
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_NVSurfaceToColorConverter_get(cv::Ptr<cv::cudacodec::NVSurfaceToColorConverter>* ptr, cv::cudacodec::NVSurfaceToColorConverter** returnValue)
{
    BEGIN_WRAP
        *returnValue = ptr->get(); 
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_NVSurfaceToColorConverter_delete(cv::Ptr<cv::cudacodec::NVSurfaceToColorConverter>* ptr)
{
    BEGIN_WRAP 
        delete ptr; 
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_NVSurfaceToColorConverter_convert(
    cv::cudacodec::NVSurfaceToColorConverter* obj,
    cv::_InputArray* yuv,
    cv::_OutputArray* color,
    int surfaceFormat,
    int outputFormat,
    int bitDepth,
    int planar,
    cv::cuda::Stream* stream,
    int* returnValue)
{
    BEGIN_WRAP
        cv::cuda::Stream& streamRef = stream ? *stream : cv::cuda::Stream::Null();

    bool result = obj->convert(
        *yuv,
        *color,
        static_cast<cv::cudacodec::SurfaceFormat>(surfaceFormat),
        static_cast<cv::cudacodec::ColorFormat>(outputFormat),
        static_cast<cv::cudacodec::BitDepth>(bitDepth),
        planar != 0,
        streamRef
    );

    *returnValue = result ? 1 : 0;
    END_WRAP
}

