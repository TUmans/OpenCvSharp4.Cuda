namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Color space standards used by NVIDIA hardware surface converters.
    /// These define the coefficients used to transform YUV data to RGB.
    /// </summary>
    public enum ColorSpaceStandard
    {
        /// <summary>
        /// ITU-R Recommendation BT.709. 
        /// The standard for High-Definition (HD) television.
        /// </summary>
        BT709 = 1,

        /// <summary>
        /// Unspecified color space. 
        /// The hardware will use its default internal mapping.
        /// </summary>
        Unspecified = 2,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved = 3,

        /// <summary>
        /// Federal Communications Commission (FCC) coefficients. 
        /// Used in older North American television systems.
        /// </summary>
        FCC = 4,

        /// <summary>
        /// ITU-R Recommendation BT.470. 
        /// Typically used for older analog television systems (PAL/SECAM).
        /// </summary>
        BT470 = 5,

        /// <summary>
        /// ITU-R Recommendation BT.601. 
        /// The standard for Standard-Definition (SD) digital video.
        /// </summary>
        BT601 = 6,

        /// <summary>
        /// SMPTE 240M. 
        /// Used in early High-Definition television systems before BT.709 was finalized.
        /// </summary>
        SMPTE240M = 7,

        /// <summary>
        /// YCgCo color space. 
        /// Optimized for video compression (used in H.264/H.265) to reduce data while preserving color.
        /// </summary>
        YCgCo = 8,

        /// <summary>
        /// ITU-R Recommendation BT.2020. 
        /// The standard for Ultra-High-Definition (4K and 8K) television.
        /// </summary>
        BT2020 = 9,

        /// <summary>
        /// ITU-R Recommendation BT.2020 Constant Luminance. 
        /// A variation of BT.2020 that ensures luminance is independent of chroma for higher precision.
        /// </summary>
        BT2020C = 10
    }
}