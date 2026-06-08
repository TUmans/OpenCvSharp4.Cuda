namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// ColorFormat for the frame returned by VideoReader or used to initialize a VideoWriter.
    /// </summary>
    public enum ColorFormat
    {
        UNDEFINED = 0,
        BGRA = 1,
        BGR = 2,
        GRAY = 3,
        NV_NV12 = 4,
        RGB = 5,
        RGBA = 6,
        /// <summary> Native NVIDIA YUV surface format </summary>
        NV_YUV_SURFACE_FORMAT = 7,
        NV_YV12 = 8,
        NV_IYUV = 9,
        NV_YUV444 = 10,
        NV_AYUV = 11,
        /// <summary> 10-bit YUV 4:2:0 </summary>
        NV_YUV420_10BIT = 12,
        /// <summary> 10-bit YUV 4:4:4 </summary>
        NV_YUV444_10BIT = 13
    }

}
