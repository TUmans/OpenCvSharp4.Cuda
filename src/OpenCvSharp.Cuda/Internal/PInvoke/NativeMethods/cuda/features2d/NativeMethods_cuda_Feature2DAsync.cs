using System.Runtime.InteropServices;

using static OpenCvSharp.Internal.NativeMethods;

namespace OpenCvSharp.Internal;

#pragma warning disable 1591

static partial class NativeMethods_cuda
{
    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_Feature2DAsync_detectAsync(
    IntPtr obj, IntPtr image, IntPtr keypoints, IntPtr mask, IntPtr stream);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_Feature2DAsync_computeAsync(
        IntPtr obj, IntPtr image, IntPtr keypoints, IntPtr descriptors, IntPtr stream);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_Feature2DAsync_detectAndComputeAsync(
        IntPtr obj, IntPtr image, IntPtr mask, IntPtr keypoints, IntPtr descriptors, int useProvidedKeypoints, IntPtr stream);

    [DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern ExceptionStatus cuda_Feature2DAsync_convert(
        IntPtr obj, IntPtr gpu_keypoints, IntPtr keypoints);


}