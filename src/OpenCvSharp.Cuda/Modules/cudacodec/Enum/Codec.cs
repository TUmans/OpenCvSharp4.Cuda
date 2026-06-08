namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Video codecs supported by cudacodec::VideoReader and cudacodec::VideoWriter.
    /// </summary>
    public enum Codec
    {
        MPEG1 = 0,
        MPEG2,
        MPEG4,
        VC1,
        H264,
        JPEG,
        H264_SVC,
        H264_MVC,
        HEVC,
        VP8,
        VP9,
        AV1,
        NumCodecs,
        /// <summary> Uncompressed YUV420 (FourCC: IYUV) </summary>
        Uncompressed_YUV420 = (('I' << 24) | ('Y' << 16) | ('U' << 8) | ('V')),
        /// <summary> Uncompressed YV12 (FourCC: YV12) </summary>
        Uncompressed_YV12 = (('Y' << 24) | ('V' << 16) | ('1' << 8) | ('2')),
        /// <summary> Uncompressed NV12 (FourCC: NV12) </summary>
        Uncompressed_NV12 = (('N' << 24) | ('V' << 16) | ('1' << 8) | ('2')),
        /// <summary> Uncompressed YUYV (FourCC: YUYV) </summary>
        Uncompressed_YUYV = (('Y' << 24) | ('U' << 16) | ('Y' << 8) | ('V')),
        /// <summary> Uncompressed UYVY (FourCC: UYVY) </summary>
        Uncompressed_UYVY = (('U' << 24) | ('Y' << 16) | ('V' << 8) | ('Y'))
    }
}
