namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Supported Encoder Profiles.
    /// </summary>
    public enum EncodeProfile
    {
        AutoSelect = 0,
        H264_Baseline = 1,
        H264_Main = 2,
        H264_High = 3,
        H264_High444 = 4,
        H264_Stereo = 5,
        H264_ProgressiveHigh = 6,
        H264_ConstrainedHigh = 7,
        HEVC_Main = 8,
        HEVC_Main10 = 9,
        HEVC_Frext = 10
    }
}
