using System;
using OpenCvSharp.Internal;

namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Video reader class for CUDA-accelerated decoding.
    /// </summary>
    public class VideoReader : Algorithm
    {
        /// <summary>
        /// Protected constructor for Ptr initialization
        /// </summary>
        protected VideoReader(IntPtr smartPtr, IntPtr rawPtr)
            : base(smartPtr, rawPtr, p => NativeMethods_cuda.cuda_VideoReader_delete(p))
        {
        }

        /// <summary>
        /// Creates video reader from a raw video source.
        /// </summary>
        public static VideoReader Create(RawVideoSource source, VideoReaderInitParams? @params = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.ThrowIfDisposed();

            VideoReaderInitParams p = @params ?? VideoReaderInitParams.Default();

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_createVideoReader1(source.CvPtr, p, out IntPtr smartPtr));

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_VideoReader_get1(smartPtr, out IntPtr rawPtr));

            return new VideoReader(smartPtr, rawPtr);
        }

        /// <summary>
        /// Creates video reader from a file.
        /// </summary>
        /// <param name="filename">Path to the video file.</param>
        /// <param name="sourceParams">Optional parameters passed to the backend source.</param>
        /// <param name="params">Initial parameters for the decoder.</param>
        public static VideoReader Create(string filename, int[]? sourceParams = null, VideoReaderInitParams? @params = null)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException(nameof(filename));

            VideoReaderInitParams p = @params ?? VideoReaderInitParams.Default();
            int[] sParams = sourceParams ?? Array.Empty<int>();

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_createVideoReader2(filename, sParams, sParams.Length, p, out IntPtr smartPtr));

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_VideoReader_get1(smartPtr, out IntPtr rawPtr));

            return new VideoReader(smartPtr, rawPtr);
        }

        public FormatInfo Format
        {
            get
            {
                ThrowIfDisposed();
                NativeMethods.HandleException(NativeMethods_cuda.cuda_VideoReader_format(RawPtr, out var ret));
                GC.KeepAlive(this);
                return ret;
            }
        }

        /// <summary>
        /// Retrieves the specified property used by the VideoSource. 
        /// </summary>
        public bool Get(int propertyId, out double propertyVal)
        {
            ThrowIfDisposed();
            NativeMethods.HandleException(NativeMethods_cuda.cuda_VideoReader_get2(RawPtr, propertyId, out propertyVal, out int ret));
            GC.KeepAlive(this);
            return ret != 0;
        }

        /// <summary>
        /// Returns the specified VideoReader property.
        /// </summary>
        public bool Get(VideoReaderProps propertyId, out double propertyVal)
        {
            ThrowIfDisposed();
            NativeMethods.HandleException(NativeMethods_cuda.cuda_VideoReader_getProp(RawPtr, (int)propertyId, out propertyVal, out int ret));
            GC.KeepAlive(this);
            return ret != 0;
        }

        /// <summary>
        /// Returns the specified VideoReader property.
        /// </summary>
        public bool GetVideoReaderProps(VideoReaderProps propertyId, out double propertyValOut, double propertyValIn = 0)
        {
            ThrowIfDisposed();
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_VideoReader_getVideoReaderProps(
                    RawPtr, (int)propertyId, out propertyValOut, propertyValIn, out int ret));

            GC.KeepAlive(this);
            return ret != 0;
        }

        /// <summary>
        /// Grabs the next frame from the video source. 
        /// </summary>
        public bool Grab(Stream? stream = null)
        {
            ThrowIfDisposed();
            NativeMethods.HandleException(NativeMethods_cuda.cuda_VideoReader_grab(RawPtr, stream?.CvPtr ?? IntPtr.Zero, out int ret));
            GC.KeepAlive(this);
            if (stream != null) GC.KeepAlive(stream);
            return ret != 0;
        }

        /// <summary>
        /// Grabs, decodes and returns the next video frame and frame luma histogram. 
        /// </summary>
        public bool NextFrame(GpuMat frame, Stream? stream = null)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            ThrowIfDisposed();
            frame.ThrowIfDisposed();
            NativeMethods.HandleException(NativeMethods_cuda.cuda_VideoReader_nextFrame(RawPtr, frame.CvPtr, stream?.CvPtr ?? IntPtr.Zero, out int ret));
            GC.KeepAlive(this);
            GC.KeepAlive(frame);
            if (stream != null) GC.KeepAlive(stream);
            return ret != 0;
        }

        /// <summary>
        /// Grabs, decodes and returns the next video frame. 
        /// </summary>

        public bool NextFrame(GpuMat frame, GpuMat histogram, Stream? stream = null)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            if (histogram == null) throw new ArgumentNullException(nameof(histogram));
            ThrowIfDisposed();
            frame.ThrowIfDisposed();
            histogram.ThrowIfDisposed();
            NativeMethods.HandleException(NativeMethods_cuda.cuda_VideoReader_nextFrame_withHist(RawPtr, frame.CvPtr, histogram.CvPtr, stream?.CvPtr ?? IntPtr.Zero, out int ret));
            GC.KeepAlive(this);
            GC.KeepAlive(frame);
            GC.KeepAlive(histogram);
            if (stream != null) GC.KeepAlive(stream);
            return ret != 0;
        }

        /// <summary>
        /// Returns previously grabbed encoded video data into a CPU Mat.
        /// </summary>
        public bool Retrieve(Mat frame, nuint idx)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            ThrowIfDisposed();
            frame.ThrowIfDisposed();

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_VideoReader_retrieve(RawPtr, frame.CvPtr, idx, out int ret));


            GC.KeepAlive(this);
            GC.KeepAlive(frame);
            return ret != 0;
        }

        /// <summary>
        /// Returns previously grabbed video data.
        /// </summary>
        public bool Retrieve(CudaOutputArray frame, VideoReaderProps idx = VideoReaderProps.DecodedFrameIdx)
        {
            return Retrieve(frame, (nuint)idx);
        }

        // Internal master method
        private bool Retrieve(CudaOutputArray frame, nuint idx)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            ThrowIfDisposed();
            frame.ThrowIfNotReady();

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_VideoReader_retrieve(RawPtr, frame.CvPtr, idx, out int ret));

            // Sync C# header with C++ allocation
            frame.Fix();

            GC.KeepAlive(this);
            GC.KeepAlive(frame);
            return ret != 0;
        }

        /// <summary>
        /// Set the desired ColorFormat for the frame returned by nextFrame()/retrieve(). 
        /// </summary>
        public bool Set(ColorFormat colorFormat, BitDepth bitDepth = BitDepth.Unchanged, bool planar = false)
        {
            ThrowIfDisposed();
            NativeMethods.HandleException(NativeMethods_cuda.cuda_VideoReader_set(RawPtr, (int)colorFormat, (int)bitDepth, planar ? 1 : 0, out int ret));
            GC.KeepAlive(this);
            return ret != 0;
        }

        /// <summary>
        /// Sets a property in the VideoReader.
        /// </summary>
        /// <param name="propertyId">Property identifier.</param>
        /// <param name="propertyVal">The value to set.</param>
        /// <returns>True if the property is supported and set.</returns>
        public bool SetVideoReaderProps(VideoReaderProps propertyId, double propertyVal)
        {
            ThrowIfDisposed();
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_VideoReader_setVideoReaderProps(
                    RawPtr, (int)propertyId, propertyVal, out int ret));

            GC.KeepAlive(this);
            return ret != 0;
        }
    }
}