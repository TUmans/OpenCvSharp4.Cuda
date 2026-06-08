#pragma once

#include "include_opencv.h"
#include <opencv2/cudacodec.hpp>
#include <vector>
#include <cstdint>

typedef void (*OnEncodedCallback)(
    const uint8_t* const* packetData,
    const int* packetSizes,
    int                   packetCount,
    const uint64_t* pts,
    void* userdata);

typedef void (*OnEncodingFinishedCallback)(void* userdata);

typedef int  (*SetFrameIntervalPCallback)(int frameIntervalP, void* userdata);

// ── Concrete subclass ─────────────────────────────────────────────────────────

class ManagedEncoderCallback final : public cv::cudacodec::EncoderCallback
{
public:
    ManagedEncoderCallback(
        OnEncodedCallback          onEncoded,
        OnEncodingFinishedCallback onEncodingFinished,
        SetFrameIntervalPCallback  setFrameIntervalP,
        void* userdata)
        : m_onEncoded(onEncoded)
        , m_onEncodingFinished(onEncodingFinished)
        , m_setFrameIntervalP(setFrameIntervalP)
        , m_userdata(userdata)   
        , m_isFinished(false)
    {
    }

    ~ManagedEncoderCallback() override = default;

    void onEncoded(
        const std::vector<std::vector<uint8_t>>& vPacket,
        const std::vector<uint64_t>& pts) override
    {
        const int count = static_cast<int>(vPacket.size());
        std::vector<const uint8_t*> ptrs(count);
        std::vector<int>            sizes(count);
        for (int i = 0; i < count; ++i) {
            ptrs[i] = vPacket[i].data();
            sizes[i] = static_cast<int>(vPacket[i].size());
        }
        m_onEncoded(ptrs.data(), sizes.data(), count, pts.data(), m_userdata);
    }

    void onEncodingFinished() override
    {
        if (!m_isFinished)
        {
            m_isFinished = true;
            m_onEncodingFinished(m_userdata);
        }
    }

    bool setFrameIntervalP(const int frameIntervalP) override
    {
        return m_setFrameIntervalP(frameIntervalP, m_userdata) != 0;
    }

private:
    OnEncodedCallback          m_onEncoded;
    OnEncodingFinishedCallback m_onEncodingFinished;
    SetFrameIntervalPCallback  m_setFrameIntervalP;
    void* m_userdata;
    bool  m_isFinished;
};

// ── C API ─────────────────────────────────────────────────────────────────────

CVAPI(ExceptionStatus) EncoderCallbackBridge_create(
    OnEncodedCallback          onEncoded,
    OnEncodingFinishedCallback onEncodingFinished,
    SetFrameIntervalPCallback  setFrameIntervalP,
    void* userdata,           // ← GCHandle cookie from C#
    cv::Ptr<cv::cudacodec::EncoderCallback>** returnValue)
{
    BEGIN_WRAP
        auto cb = cv::makePtr<ManagedEncoderCallback>(
            onEncoded, onEncodingFinished, setFrameIntervalP, userdata);
    *returnValue = new cv::Ptr<cv::cudacodec::EncoderCallback>(cb);
    END_WRAP
}

CVAPI(ExceptionStatus) EncoderCallbackBridge_get(
    cv::Ptr<cv::cudacodec::EncoderCallback>* ptr,
    cv::cudacodec::EncoderCallback** returnValue)
{
    BEGIN_WRAP
        * returnValue = ptr->get();
    END_WRAP
}

CVAPI(ExceptionStatus) EncoderCallbackBridge_destroy(
    cv::Ptr<cv::cudacodec::EncoderCallback>* ptr)
{
    BEGIN_WRAP
        delete ptr;
    END_WRAP
}