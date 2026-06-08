using OpenCvSharp.Cuda;
using System.Runtime.InteropServices;
using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus cudacodec_createVideoWriter_1(
        [MarshalAs(UnmanagedType.LPStr)] string fileName,
        Size frameSize,
        Codec codec,
        double fps,
        ColorFormat colorFormat,
        ref EncoderParams encoderParams,
        IntPtr encoderCallback,
        IntPtr stream,
        out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus cudacodec_createVideoWriter_2(
        [MarshalAs(UnmanagedType.LPStr)] string fileName,
        Size frameSize,
        Codec codec,
        double fps,
        ColorFormat colorFormat,
        IntPtr encoderCallback,
        IntPtr stream,
        out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus cudacodec_VideoWriter_delete(IntPtr ptr);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus cudacodec_VideoWriter_get(
        IntPtr ptr, out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus cudacodec_VideoWriter_write(
        IntPtr obj, IntPtr frame);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus cudacodec_VideoWriter_release(IntPtr obj);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus cudacodec_VideoWriter_getEncoderParams(
        IntPtr obj, out EncoderParams returnValue);

   
}