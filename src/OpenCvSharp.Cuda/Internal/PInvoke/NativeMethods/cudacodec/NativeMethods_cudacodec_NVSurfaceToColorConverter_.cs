using System.Runtime.InteropServices;

using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_createNVSurfaceToColorConverter(
    int colorSpace, int videoFullRangeFlag, out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_NVSurfaceToColorConverter_get(IntPtr ptr, out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_NVSurfaceToColorConverter_delete(IntPtr ptr);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_NVSurfaceToColorConverter_convert(IntPtr obj, IntPtr yuv, IntPtr color, int surfaceFormat, int outputFormat, int bitDepth, int planar, IntPtr stream, out int returnValue);
}
