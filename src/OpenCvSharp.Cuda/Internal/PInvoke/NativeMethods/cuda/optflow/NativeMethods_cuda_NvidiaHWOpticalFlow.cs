using System.Runtime.InteropServices;

using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_NvidiaHWOpticalFlow_calc(
    IntPtr obj, IntPtr inputImage, IntPtr referenceImage, IntPtr flow, IntPtr stream, IntPtr hint, IntPtr cost);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_NvidiaHWOpticalFlow_collectGarbage(IntPtr obj);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_NvidiaHWOpticalFlow_getGridSize(IntPtr obj, out int returnValue);
}