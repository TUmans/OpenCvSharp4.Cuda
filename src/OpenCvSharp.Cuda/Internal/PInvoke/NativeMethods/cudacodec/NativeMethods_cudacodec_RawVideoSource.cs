using OpenCvSharp.Cuda;
using System.Runtime.InteropServices;

using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void cuda_RawVideoSource_delete(IntPtr obj);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_RawVideoSource_format(IntPtr obj, out FormatInfo returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_RawVideoSource_get(IntPtr obj, int propertyId, out double propertyVal, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_RawVideoSource_getExtraData(IntPtr obj, IntPtr extraData);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_RawVideoSource_getFirstFrameIdx(IntPtr obj, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_RawVideoSource_getNextPacket(IntPtr obj, out IntPtr data, out UIntPtr size, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_RawVideoSource_lastPacketContainsKeyFrame(IntPtr obj, out int returnValue);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_RawVideoSource_updateFormat(IntPtr obj, ref FormatInfo videoFormat);
}
