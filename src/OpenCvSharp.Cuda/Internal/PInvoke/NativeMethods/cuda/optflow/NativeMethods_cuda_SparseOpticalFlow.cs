using System.Runtime.InteropServices;

using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_SparseOpticalFlow_calc(
    IntPtr obj, IntPtr prevImg, IntPtr nextImg, IntPtr prevPts, IntPtr nextPts,
    IntPtr status, IntPtr err, IntPtr stream);
}