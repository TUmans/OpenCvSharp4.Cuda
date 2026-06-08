using System.Runtime.InteropServices;

namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Quantization Parameters (QP) for each frame type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EncodeQp
    {
        /// <summary> Specifies QP value for P-frame. </summary>
        public uint QpInterP;

        /// <summary> Specifies QP value for B-frame. </summary>
        public uint QpInterB;

        /// <summary> Specifies QP value for Intra Frame. </summary>
        public uint QpIntra;

        // No constructor needed. You can use it like this:
        // var qp = new EncodeQp { QpIntra = 25, QpInterP = 28, QpInterB = 31 };
    }
}