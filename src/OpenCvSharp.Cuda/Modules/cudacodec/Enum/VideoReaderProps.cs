namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// cv::cudacodec::VideoReader generic properties identifier.
    /// </summary>
    public enum VideoReaderProps
    {
        /// <summary> Index of the currently decoded frame </summary>
        DecodedFrameIdx = 0,
        /// <summary> Index of extra data </summary>
        ExtraDataIndex = 1,
        /// <summary> Base index for raw packages </summary>
        RawPackagesBaseIndex = 2,
        /// <summary> Number of raw packages since last grab </summary>
        NumberOfRawPackagesSinceLastGrab = 3,
        /// <summary> Raw mode flag </summary>
        RawMode = 4,
        /// <summary> LRF has key frame flag </summary>
        LrfHasKeyFrame = 5,
        /// <summary> Color format </summary>
        ColorFormat = 6,
        /// <summary> UDP source flag </summary>
        UdpSource = 7,
        /// <summary> Allow frame drop flag </summary>
        AllowFrameDrop = 8,
        /// <summary> Bit depth </summary>
        BitDepth = 9,
        /// <summary> Planar flag </summary>
        Planar = 10
    }
}
