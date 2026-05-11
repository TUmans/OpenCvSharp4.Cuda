using System.Runtime.InteropServices;

using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{
    #region cv::cuda::GpuMat
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern IntPtr vector_GpuMat_new1();
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern IntPtr vector_GpuMat_new2(uint size);
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern IntPtr vector_GpuMat_new3(IntPtr[] data, uint dataLength);
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern nuint vector_GpuMat_getSize(IntPtr vector);
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern IntPtr vector_GpuMat_getPointer(IntPtr vector);
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void vector_GpuMat_delete(IntPtr vector);
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void vector_GpuMat_assignToArray(IntPtr vector, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] arr);
    #endregion
    #region ulong
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern IntPtr vector_uint64_new1();

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern IntPtr vector_uint64_new2(nuint size);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern IntPtr vector_uint64_new3([In] ulong[] data, nuint size);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void vector_uint64_delete(IntPtr vector);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern nuint vector_uint64_getSize(IntPtr vector);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern IntPtr vector_uint64_getPointer(IntPtr vector);
    #endregion
}

