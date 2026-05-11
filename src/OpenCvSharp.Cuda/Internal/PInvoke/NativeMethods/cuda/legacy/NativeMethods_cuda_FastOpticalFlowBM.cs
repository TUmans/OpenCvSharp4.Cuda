using System.Runtime.InteropServices;

using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_FastOpticalFlowBM_compute(
    IntPtr i0, IntPtr i1, IntPtr flowx, IntPtr flowy, int searchWindow, int blockWindow, IntPtr stream);
}
