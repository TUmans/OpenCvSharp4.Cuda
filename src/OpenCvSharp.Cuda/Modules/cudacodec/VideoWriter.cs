using OpenCvSharp.Internal;
using System;

namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Managed wrapper for <c>cv::cudacodec::VideoWriter</c>.
    ///
    /// <para>
    /// <b>Important:</b> You must call <see cref="Release"/> before disposing
    /// this object. <see cref="Release"/> flushes the encoder queue, finalises
    /// the output container, and fires
    /// <see cref="EncoderCallback.OnEncodingFinished"/>. Simply disposing
    /// without calling <see cref="Release"/> first will produce a corrupt or
    /// missing output file, matching the behaviour of the underlying
    /// <c>cv::cudacodec::VideoWriter</c> API.
    /// </para>
    ///
    /// <para>Recommended usage:</para>
    /// <code>
    /// using var writer = new VideoWriter(path, size, callback: cb);
    /// foreach (var frame in frames)
    ///     writer.Write(frame);
    /// writer.Release();   // ← required before Dispose / end of using block
    /// </code>
    /// </summary>
    public sealed class VideoWriter : CvObject
    {
        private readonly EncoderCallback? _callback;

        // ── Constructors ──────────────────────────────────────────────────────

        /// <summary>
        /// Creates a VideoWriter with full control over encoder parameters.
        /// </summary>
        /// <param name="fileName">Output file path.</param>
        /// <param name="frameSize">Width × height of every frame.</param>
        /// <param name="codec">Codec to use.</param>
        /// <param name="fps">Target frames per second.</param>
        /// <param name="colorFormat">Pixel format of the input frames.</param>
        /// <param name="encoderParams">Advanced encoder parameters.</param>
        /// <param name="encoderCallback">
        ///   Callback that receives encoded packets. Required by NVENC —
        ///   use <see cref="EncoderCallback"/> if you don't need packets.
        /// </param>
        /// <param name="stream">CUDA stream, or null for the null stream.</param>
        public VideoWriter(
            string fileName,
            Size frameSize,
            Codec codec,
            double fps,
            ColorFormat colorFormat,
            EncoderParams encoderParams,
            EncoderCallback? encoderCallback = null,
            Stream? stream = null)
        {
            _callback = encoderCallback;

            NativeMethods.HandleException(
                NativeMethods_cuda.cudacodec_createVideoWriter_1(
                    fileName,
                    frameSize,
                    codec,
                    fps,
                    colorFormat,
                    ref encoderParams,
                   _callback?.RawPtr ?? IntPtr.Zero,
                    stream?.CvPtr ?? IntPtr.Zero,
                    out IntPtr smartPtr));

            SetSafeHandle(new OpenCvCudaPtrSafeHandle(smartPtr, ownsHandle: true,
                static h => NativeMethods_cuda.cudacodec_VideoWriter_delete(h)));
        }

        /// <summary>
        /// Creates a VideoWriter with default encoder parameters.
        /// </summary>
        /// <param name="fileName">Output file path.</param>
        /// <param name="frameSize">Width × height of every frame.</param>
        /// <param name="codec">Codec to use. Defaults to <see cref="Codec.H264"/>.</param>
        /// <param name="fps">Target frames per second. Defaults to 25.</param>
        /// <param name="colorFormat">Pixel format. Defaults to <see cref="ColorFormat.BGR"/>.</param>
        /// <param name="encoderCallback">
        ///   Callback that receives encoded packets. Required by NVENC —
        ///   use <see cref="EncoderCallback"/> if you don't need packets.
        /// </param>
        /// <param name="stream">CUDA stream, or null for the null stream.</param>
        public VideoWriter(
          string fileName,
          Size frameSize,
          Codec codec = Codec.H264,
          double fps = 25.0,
          ColorFormat colorFormat = ColorFormat.BGR,
          EncoderCallback? encoderCallback = null, // Can be null!
          Stream? stream = null)
        {
            // Keep a strong reference to prevent GC, if one was provided
            _callback = encoderCallback;

            NativeMethods.HandleException(
                NativeMethods_cuda.cudacodec_createVideoWriter_2(
                    fileName,
                    frameSize,
                    codec,
                    fps,
                    colorFormat,
                    _callback?.RawPtr ?? IntPtr.Zero, // Pass IntPtr.Zero if no callback
                    stream?.CvPtr ?? IntPtr.Zero,
                    out IntPtr smartPtr));

            SetSafeHandle(new OpenCvCudaPtrSafeHandle(smartPtr, ownsHandle: true,
                static h => NativeMethods_cuda.cudacodec_VideoWriter_delete(h)));
        }


        internal VideoWriter(IntPtr smartPtr)
        {
            SetSafeHandle(new OpenCvCudaPtrSafeHandle(smartPtr, ownsHandle: true,
                static h => NativeMethods_cuda.cudacodec_VideoWriter_delete(h)));
        }

        // ── Raw pointer ───────────────────────────────────────────────────────

        internal IntPtr RawPtr
        {
            get
            {
                NativeMethods.HandleException(
                    NativeMethods_cuda.cudacodec_VideoWriter_get(CvPtr, out IntPtr raw));
                return raw;
            }
        }

        // ── Public API ────────────────────────────────────────────────────────
        /// <summary>
        /// Encodes the next video frame.
        /// </summary>
        public void Write(CudaInputArray frame)
        {
            if (frame == null || frame.CvPtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(frame));
            NativeMethods.HandleException(
                NativeMethods_cuda.cudacodec_VideoWriter_write(RawPtr, frame.CvPtr));
        }

        /// <summary>
        /// Flushes the encoder, finalises the output file, and fires
        /// <see cref="EncoderCallback.OnEncodingFinished"/>.
        ///
        /// <para>
        /// <b>Must be called before <see cref="IDisposable.Dispose"/>.</b>
        /// Matches the behaviour of <c>cv::cudacodec::VideoWriter::release()</c>.
        /// </para>
        /// </summary>
        public void Release()
        {
            NativeMethods.HandleException(
                NativeMethods_cuda.cudacodec_VideoWriter_release(RawPtr));
        }

        /// <summary>
        /// Returns a snapshot of the active encoder parameters.
        /// </summary>
        public EncoderParams GetEncoderParams()
        {
            NativeMethods.HandleException(
                NativeMethods_cuda.cudacodec_VideoWriter_getEncoderParams(RawPtr, out EncoderParams p));
            return p;
        }

      
    }
}