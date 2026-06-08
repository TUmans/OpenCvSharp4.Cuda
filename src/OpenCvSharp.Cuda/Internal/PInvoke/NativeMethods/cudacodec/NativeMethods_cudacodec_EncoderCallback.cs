using System.Runtime.InteropServices;
using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal unsafe delegate void OnEncodedCallback(
        byte** packetData, int* packetSizes, int packetCount, ulong* pts, IntPtr userdata);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void OnEncodingFinishedCallback(IntPtr userdata);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int SetFrameIntervalPCallback(int frameIntervalP, IntPtr userdata);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus EncoderCallbackBridge_create(
        OnEncodedCallback onEncoded,
        OnEncodingFinishedCallback onEncodingFinished,
        SetFrameIntervalPCallback setFrameIntervalP,
        IntPtr userdata,           
        out IntPtr returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus EncoderCallbackBridge_destroy(IntPtr ptr);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern ExceptionStatus EncoderCallbackBridge_get(
        IntPtr ptr, out IntPtr returnValue);
}