#pragma once

// -----------------------------------------------------------------------
// OpenCvSharpExtern – cv::cuda arithmetic wrappers
// These are the C-linkage functions that the C# P/Invoke layer calls.
// Each function catches cv::Exception, stores it, and returns an
// ExceptionStatus so managed code can rethrow it as a .NET exception.
// -----------------------------------------------------------------------

#include "include_opencv.h"
#include <opencv2/cudacodec.hpp>

CVAPI(ExceptionStatus) cuda_RawVideoSource_delete(cv::cudacodec::RawVideoSource* ptr)
{
    BEGIN_WRAP
    delete ptr;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_RawVideoSource_format(cv::cudacodec::RawVideoSource* obj, cv::cudacodec::FormatInfo* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->format();
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_RawVideoSource_get(cv::cudacodec::RawVideoSource* obj, int propertyId, double* propertyVal, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->get(propertyId, *propertyVal) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_RawVideoSource_getExtraData(cv::cudacodec::RawVideoSource* obj, cv::Mat* extraData)
{
    BEGIN_WRAP
        obj->getExtraData(*extraData);
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_RawVideoSource_getFirstFrameIdx(cv::cudacodec::RawVideoSource* obj, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->getFirstFrameIdx();
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_RawVideoSource_getNextPacket(cv::cudacodec::RawVideoSource* obj, unsigned char** data, size_t* size, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->getNextPacket(data, size) ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_RawVideoSource_lastPacketContainsKeyFrame(cv::cudacodec::RawVideoSource* obj, int* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->lastPacketContainsKeyFrame() ? 1 : 0;
    END_WRAP
}

CVAPI(ExceptionStatus) cuda_RawVideoSource_updateFormat(cv::cudacodec::RawVideoSource* obj, cv::cudacodec::FormatInfo* videoFormat)
{
    BEGIN_WRAP
        obj->updateFormat(*videoFormat);
    END_WRAP
}