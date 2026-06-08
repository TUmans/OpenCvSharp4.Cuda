namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Video surface formats output by the decoder.
    /// </summary>
    public enum SurfaceFormat
    {
        /// <summary> YUV 4:2:0 </summary>
        NV12 = 0,
        /// <summary> 16-bit YUV 4:2:0 </summary>
        P016 = 1,
        /// <summary> YUV 4:4:4 </summary>
        YUV444 = 2,
        /// <summary> 16-bit YUV 4:4:4 </summary>
        YUV444_16Bit = 3
    }
}
