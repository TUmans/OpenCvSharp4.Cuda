using System.Runtime.InteropServices;

using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_createBackgroundSubtractorMOG2(int history, double varThreshold, int detectShadows, out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_BackgroundSubtractorMOG2_get(IntPtr ptr, out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_BackgroundSubtractorMOG2_delete(IntPtr ptr);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_BackgroundSubtractorMOG2_apply(IntPtr obj, IntPtr image, IntPtr fgmask, double learningRate, IntPtr stream);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_BackgroundSubtractorMOG2_apply_withMask( IntPtr obj, IntPtr image, IntPtr knownForegroundMask, IntPtr fgmask, double learningRate, IntPtr stream);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_BackgroundSubtractorMOG2_getBackgroundImage(IntPtr obj, IntPtr backgroundImage, IntPtr stream);
}