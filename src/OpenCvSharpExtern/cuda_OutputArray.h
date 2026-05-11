#pragma once

// ReSharper disable IdentifierTypo
// ReSharper disable CppInconsistentNaming
// ReSharper disable CppNonInlineFunctionDefinitionInHeaderFile

#include "include_opencv.h"
#include <opencv2/core/cuda.hpp>

CVAPI(ExceptionStatus) cuda_OutputArray_new_byGpuMat(cv::cuda::GpuMat* gm, cv::_OutputArray** returnValue)
{
    BEGIN_WRAP
        cv::_OutputArray ia(*gm);
    *returnValue = new cv::_OutputArray(ia);
    END_WRAP
}
