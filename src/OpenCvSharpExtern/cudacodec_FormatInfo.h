#pragma once
#include "include_opencv.h"
#include <opencv2/cudacodec.hpp>

CVAPI(ExceptionStatus) cuda_FormatInfo_GetNativeDefaults(cv::cudacodec::FormatInfo* outInfo)
{
    BEGIN_WRAP
        * outInfo = cv::cudacodec::FormatInfo();
    END_WRAP
}