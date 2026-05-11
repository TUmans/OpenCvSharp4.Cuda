#pragma once

// -----------------------------------------------------------------------
// OpenCvSharpExtern – cv::cuda arithmetic wrappers
// These are the C-linkage functions that the C# P/Invoke layer calls.
// Each function catches cv::Exception, stores it, and returns an
// ExceptionStatus so managed code can rethrow it as a .NET exception.
// -----------------------------------------------------------------------

#include "include_opencv.h"
#include <opencv2/core/cuda.hpp>

CVAPI(ExceptionStatus) cuda_InputArray_new_byGpuMat(cv::cuda::GpuMat* mat, cv::_InputArray** returnValue)
{
    BEGIN_WRAP
        * returnValue = new cv::_InputArray(*mat);
    END_WRAP
}