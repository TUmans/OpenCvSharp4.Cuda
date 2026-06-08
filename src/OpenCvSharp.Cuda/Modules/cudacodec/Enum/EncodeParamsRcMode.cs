namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Rate Control Modes for the encoder.
    /// </summary>
    public enum EncodeParamsRcMode
    {
        /// <summary> Constant QP </summary>
        ConstQP = 0x0,
        /// <summary> Variable Bit Rate </summary>
        VBR = 0x1,
        /// <summary> Constant Bit Rate </summary>
        CBR = 0x2
    }
}
