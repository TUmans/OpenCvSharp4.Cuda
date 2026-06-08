#pragma once
#include "include_opencv.h"
#include <opencv2/cudacodec.hpp>


CVAPI(ExceptionStatus) cuda_EncoderParams_GetNativeDefaults(cv::cudacodec::EncoderParams* outParams)
{
    BEGIN_WRAP
        * outParams = cv::cudacodec::EncoderParams();
    END_WRAP
}

