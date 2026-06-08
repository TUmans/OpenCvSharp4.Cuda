#pragma once

// -----------------------------------------------------------------------
// OpenCvSharpExtern – cv::cudacodec::VideoWriter wrappers
// -----------------------------------------------------------------------

#include "include_opencv.h"
#include <opencv2/cudacodec.hpp>

// ── createVideoWriter – full overload (with EncoderParams) ───────────────────

CVAPI(ExceptionStatus) cudacodec_createVideoWriter_1(
    const char* fileName, MyCvSize frameSize, cv::cudacodec::Codec codec, double fps, cv::cudacodec::ColorFormat colorFormat, cv::cudacodec::EncoderParams* params,
    cv::cudacodec::EncoderCallback* encoderCallback, // Take raw pointer
    cv::cuda::Stream* stream,
    cv::Ptr<cv::cudacodec::VideoWriter>** returnValue)
{
    BEGIN_WRAP
        cv::Ptr<cv::cudacodec::EncoderCallback> cbPtr;

    // Only wrap if it's not null!
    if (encoderCallback != nullptr) {
        cbPtr = cv::Ptr<cv::cudacodec::EncoderCallback>(encoderCallback, [](cv::cudacodec::EncoderCallback*) {});
    }

    cv::cuda::Stream& streamRef = stream ? *stream : cv::cuda::Stream::Null();

    auto writer = cv::cudacodec::createVideoWriter(
        fileName, cpp(frameSize), codec, fps,
        colorFormat, *params, cbPtr, streamRef);
    *returnValue = new cv::Ptr<cv::cudacodec::VideoWriter>(writer);
    END_WRAP
}

// ── createVideoWriter – short overload (no EncoderParams) ────────────────────

CVAPI(ExceptionStatus) cudacodec_createVideoWriter_2(
    const char* fileName, MyCvSize frameSize, cv::cudacodec::Codec codec, double fps, cv::cudacodec::ColorFormat colorFormat,
    cv::cudacodec::EncoderCallback* encoderCallback, cv::cuda::Stream* stream,  cv::Ptr<cv::cudacodec::VideoWriter>** returnValue)
{
    BEGIN_WRAP
        // If C# passed IntPtr.Zero, this stays empty!
        cv::Ptr<cv::cudacodec::EncoderCallback> cbPtr;

    if (encoderCallback != nullptr) {
        cbPtr = cv::Ptr<cv::cudacodec::EncoderCallback>(encoderCallback, [](cv::cudacodec::EncoderCallback*) {});
    }

    cv::cuda::Stream& streamRef = stream ? *stream : cv::cuda::Stream::Null();

    auto writer = cv::cudacodec::createVideoWriter(
        fileName, cpp(frameSize), codec, fps,
        colorFormat,  cbPtr, streamRef);

    *returnValue = new cv::Ptr<cv::cudacodec::VideoWriter>(writer);
    END_WRAP
}

// ── Lifetime ──────────────────────────────────────────────────────────────────

CVAPI(ExceptionStatus) cudacodec_VideoWriter_delete(
    cv::Ptr<cv::cudacodec::VideoWriter>* ptr)
{
    BEGIN_WRAP
        delete ptr;
    END_WRAP
}

CVAPI(ExceptionStatus) cudacodec_VideoWriter_get(
    cv::Ptr<cv::cudacodec::VideoWriter>* ptr,
    cv::cudacodec::VideoWriter** returnValue)
{
    BEGIN_WRAP
        * returnValue = ptr->get();
    END_WRAP
}

// ── Methods ───────────────────────────────────────────────────────────────────

CVAPI(ExceptionStatus) cudacodec_VideoWriter_write(
    cv::cudacodec::VideoWriter* obj,
    cv::_InputArray* frame)
{
    BEGIN_WRAP
        obj->write(*frame);
    END_WRAP
}

CVAPI(ExceptionStatus) cudacodec_VideoWriter_release(
    cv::cudacodec::VideoWriter* obj)
{
    BEGIN_WRAP
        obj->release();
    END_WRAP
}

CVAPI(ExceptionStatus) cudacodec_VideoWriter_getEncoderParams(
    cv::cudacodec::VideoWriter* obj,
    cv::cudacodec::EncoderParams* returnValue)
{
    BEGIN_WRAP
        * returnValue = obj->getEncoderParams();
    END_WRAP
}

