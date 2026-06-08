using OpenCvSharp.Internal;

namespace OpenCvSharp.Cuda
{
    public class NVSurfaceToColorConverter : Algorithm
    {
        private NVSurfaceToColorConverter(IntPtr smartPtr, IntPtr rawPtr)
            : base(smartPtr, rawPtr, p => NativeMethods.HandleException(NativeMethods_cuda.cuda_NVSurfaceToColorConverter_delete(p)))
        {
        }

        /// <summary>
        /// Creates a NVSurfaceToColorConverter.
        /// </summary>
        public static NVSurfaceToColorConverter Create(ColorSpaceStandard colorSpace, bool videoFullRangeFlag = false)
        {
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_createNVSurfaceToColorConverter((int)colorSpace, videoFullRangeFlag ? 1 : 0, out var smartPtr));

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_NVSurfaceToColorConverter_get(smartPtr, out var rawPtr));

            return new NVSurfaceToColorConverter(smartPtr, rawPtr);
        }


        /// <summary>
        /// Converts the input surface to the target color format with full control over formats.
        /// </summary>
        /// <param name="yuv">Input YUV surface (GpuMat).</param>
        /// <param name="color">Output color image (GpuMat).</param>
        /// <param name="surfaceFormat">Format of the input surface.</param>
        /// <param name="outputFormat">Desired output color format.</param>
        /// <param name="bitDepth">Bit depth of the output. Default is Unchanged.</param>
        /// <param name="planar">If true, output is planar instead of interleaved.</param>
        /// <param name="stream">Stream for asynchronous execution.</param>
        /// <returns>True if conversion was successful.</returns>
        public bool Convert(CudaInputArray yuv, CudaOutputArray color, SurfaceFormat surfaceFormat, ColorFormat outputFormat,BitDepth bitDepth = BitDepth.Unchanged, bool planar = false, Stream? stream = null)
        {
            if (yuv == null) throw new ArgumentNullException(nameof(yuv));
            if (color == null) throw new ArgumentNullException(nameof(color));

            yuv.ThrowIfDisposed();
            color.ThrowIfNotReady();
            ThrowIfDisposed();

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_NVSurfaceToColorConverter_convert(
                    RawPtr,
                    yuv.CvPtr,
                    color.CvPtr,
                    (int)surfaceFormat,
                    (int)outputFormat,
                    (int)bitDepth,
                    planar ? 1 : 0,
                    stream?.CvPtr ?? IntPtr.Zero,
                    out int ret));

            color.Fix();
            GC.KeepAlive(this);
            GC.KeepAlive(yuv);
            GC.KeepAlive(stream);

            return ret != 0;
        }

     
    }
}
