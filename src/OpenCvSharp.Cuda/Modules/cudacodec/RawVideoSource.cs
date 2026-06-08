using OpenCvSharp.Internal;
using System;
using System.Runtime.InteropServices;

namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Interface for video sources that provide raw encoded packets.
    /// </summary>
    public class RawVideoSource : DisposableGpuObject
    {
        internal RawVideoSource(IntPtr ptr)
        {
            ThrowIfNotAvailable();
            if (ptr == IntPtr.Zero)
                throw new OpenCvSharpException("Native object address is NULL");
            InitSafeHandle(ptr);
        }

        private void InitSafeHandle(IntPtr p, bool ownsHandle = true)
        {
            SetSafeHandle(new OpenCvCudaPtrSafeHandle(p, ownsHandle,
                static h => NativeMethods_cuda.cuda_RawVideoSource_delete(h)));
        }

        /// <summary>
        /// Returns information about video file format.
        /// </summary>
        public FormatInfo Format
        {
            get
            {
                ThrowIfDisposed();
                NativeMethods.HandleException(
                    NativeMethods_cuda.cuda_RawVideoSource_format(ptr, out var ret));
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
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_RawVideoSource_get(ptr, propertyId, out propertyVal, out int ret));
            GC.KeepAlive(this);
            return ret != 0;
        }

        /// <summary>
        /// Returns any extra data associated with the video source (e.g. PPS/SPS).
        /// </summary>
        public void GetExtraData(Mat extraData)
        {
            if (extraData == null) throw new ArgumentNullException(nameof(extraData));
            ThrowIfDisposed();
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_RawVideoSource_getExtraData(ptr, extraData.CvPtr));
            GC.KeepAlive(this);
            GC.KeepAlive(extraData);
        }

        /// <summary>
        /// Retrieve the index of the first frame that will returned after construction.
        /// </summary>
        public int FirstFrameIdx
        {
            get
            {
                ThrowIfDisposed();
                NativeMethods.HandleException(
                    NativeMethods_cuda.cuda_RawVideoSource_getFirstFrameIdx(ptr, out int ret));
                GC.KeepAlive(this);
                return ret;
            }
        }

        /// <summary>
        /// Returns next packet with RAW video frame.
        /// </summary>
        /// <param name="data">Pointer to the output packet data.</param>
        /// <param name="size">Size of the output packet.</param>
        /// <returns>False if no more packets are available.</returns>
        public bool GetNextPacket(out IntPtr data, out ulong size)
        {
            ThrowIfDisposed();
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_RawVideoSource_getNextPacket(ptr, out data, out var sizePtr, out int ret));
            size = (ulong)sizePtr;
            GC.KeepAlive(this);
            return ret != 0;
        }

        /// <summary>
        /// Returns true if the last packet contained a key frame.
        /// </summary>
        public bool LastPacketContainsKeyFrame
        {
            get
            {
                ThrowIfDisposed();
                NativeMethods.HandleException(
                    NativeMethods_cuda.cuda_RawVideoSource_lastPacketContainsKeyFrame(ptr, out int ret));
                GC.KeepAlive(this);
                return ret != 0;
            }
        }

        /// <summary>
        /// Updates the coded width and height inside format.
        /// </summary>
        public void UpdateFormat(FormatInfo videoFormat)
        {
            ThrowIfDisposed();
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_RawVideoSource_updateFormat(ptr, ref videoFormat));
            GC.KeepAlive(this);
        }
    }
}