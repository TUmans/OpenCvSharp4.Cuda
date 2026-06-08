using OpenCvSharp.Cuda;
using System.Runtime.InteropServices;

using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_createVideoReader1(IntPtr source, VideoReaderInitParams @params, out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_createVideoReader2([MarshalAs(UnmanagedType.LPStr)] string filename, int[] sourceParams, int sourceParamsSize, VideoReaderInitParams @params, out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_get1(IntPtr ptr, out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_delete(IntPtr ptr);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_format(IntPtr obj, out FormatInfo returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_get2(IntPtr obj, int propertyId, out double propertyVal, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_getProp(IntPtr obj, int propertyId, out double propertyVal, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_getVideoReaderProps(IntPtr obj, int propertyId, out double propertyValOut, double propertyValIn, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_grab(IntPtr obj, IntPtr stream, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_nextFrame(IntPtr obj, IntPtr frame, IntPtr stream, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_nextFrame_withHist(IntPtr obj, IntPtr frame, IntPtr histogram, IntPtr stream, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_retrieve(IntPtr obj, IntPtr frame, UIntPtr idx, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_set(IntPtr obj, int colorFormat, int bitDepth, int planar, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_VideoReader_setVideoReaderProps(
        IntPtr obj,
        int propertyId,
        double propertyVal,
        out int returnValue);
}

