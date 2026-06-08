#pragma once
#include "include_opencv.h"
#include <opencv2/cudacodec.hpp>

CVAPI(ExceptionStatus) cuda_VideoReaderInitParams_GetNativeDefaults(cv::cudacodec::VideoReaderInitParams* outParams)
{
    BEGIN_WRAP
        * outParams = cv::cudacodec::VideoReaderInitParams();
    END_WRAP
}