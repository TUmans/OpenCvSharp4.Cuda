namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Bit depth of the frame returned by VideoReader::nextFrame() and VideoReader::retrieve()
    /// </summary>
    public enum BitDepth
    {
        /// <summary> 8-bit depth </summary>
        Eight = 0,
        /// <summary> 16-bit depth </summary>
        Sixteen = 1,
        /// <summary> Keep original bit depth </summary>
        Unchanged = 2
    }
}
