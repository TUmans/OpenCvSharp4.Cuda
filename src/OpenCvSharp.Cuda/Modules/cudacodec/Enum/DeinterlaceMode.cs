namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Deinterlacing mode used by decoder.
    /// </summary>
    public enum DeinterlaceMode
    {
        /// <summary> No deinterlacing </summary>
        Weave = 0,
        /// <summary> Bob deinterlacing </summary>
        Bob = 1,
        /// <summary> Adaptive deinterlacing </summary>
        Adaptive = 2
    }
}
