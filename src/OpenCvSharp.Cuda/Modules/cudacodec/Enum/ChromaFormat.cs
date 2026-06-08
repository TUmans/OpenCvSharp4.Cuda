namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Chroma formats supported by cudacodec::VideoReader.
    /// </summary>
    public enum ChromaFormat
    {
        /// <summary> YUV 4:0:0 </summary>
        Monochrome = 0,
        /// <summary> YUV 4:2:0 </summary>
        YUV420,
        /// <summary> YUV 4:2:2 </summary>
        YUV422,
        /// <summary> YUV 4:4:4 </summary>
        YUV444,
        /// <summary> Number of formats </summary>
        NumFormats
    }

}
