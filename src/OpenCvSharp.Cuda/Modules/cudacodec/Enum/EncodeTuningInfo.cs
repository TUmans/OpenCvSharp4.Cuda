namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Tuning information for the encoder.
    /// </summary>
    public enum EncodeTuningInfo
    {
        Undefined = 0,
        HighQuality = 1,
        LowLatency = 2,
        UltraLowLatency = 3,
        Lossless = 4,
        Count
    }
}
