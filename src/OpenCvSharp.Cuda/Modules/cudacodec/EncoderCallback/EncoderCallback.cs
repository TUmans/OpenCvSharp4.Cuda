using OpenCvSharp.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Managed mirror of <c>cv::cudacodec::EncoderCallback</c>.
    /// Derive from this class and override the three abstract members to receive
    /// encoder events from OpenCV's CUDA video codec pipeline.
    /// </summary>
    public abstract unsafe class EncoderCallback : CvObject
    {
        private readonly NativeMethods_cuda.OnEncodedCallback _nativeOnEncoded;
        private readonly NativeMethods_cuda.OnEncodingFinishedCallback _nativeOnEncodingFinished;
        private readonly NativeMethods_cuda.SetFrameIntervalPCallback _nativeSetFrameIntervalP;

        // Roots 'this' so the GC cannot collect or move it while native code
        // holds the cookie in userdata.
        private GCHandle _selfHandle;

        protected EncoderCallback()
        {
            // Must be allocated BEFORE the delegates are created and passed to
            // native code, so userdata is valid the instant the first callback fires.
            _selfHandle = GCHandle.Alloc(this);

            _nativeOnEncoded = OnEncodedThunk;
            _nativeOnEncodingFinished = OnEncodingFinishedThunk;
            _nativeSetFrameIntervalP = SetFrameIntervalPThunk;

            NativeMethods.HandleException(
                NativeMethods_cuda.EncoderCallbackBridge_create(
                    _nativeOnEncoded,
                    _nativeOnEncodingFinished,
                    _nativeSetFrameIntervalP,
                    GCHandle.ToIntPtr(_selfHandle),   
                    out IntPtr smartPtr));

            SetSafeHandle(new OpenCvCudaPtrSafeHandle(smartPtr, ownsHandle: true,
                static h => NativeMethods_cuda.EncoderCallbackBridge_destroy(h)));
        }

        public IntPtr RawPtr
        {
            get
            {
                NativeMethods.HandleException(
                    NativeMethods_cuda.EncoderCallbackBridge_get(CvPtr, out IntPtr raw));
                return raw;
            }
        }

        /// <summary>
        /// Triggered for each encoded packet (NAL unit) produced by the hardware encoder.
        /// </summary>
        /// <param name="dataPtr">Unmanaged pointer to the raw packet bytes.</param>
        /// <param name="size">The size of the packet in bytes.</param>
        /// <param name="pts">Presentation timestamp.</param>
        public abstract void OnEncodedPacket(IntPtr dataPtr, int size, ulong pts);

        /// <summary>
        /// Triggered when the encoder has finished processing the stream.
        /// </summary>
        public abstract void OnEncodingFinished();

        /// <summary>
        /// Optionally set the GOP pattern.
        /// </summary>
        public virtual bool SetFrameIntervalP(int frameIntervalP) => true;


        private static unsafe void OnEncodedThunk(
           byte** packetData,
           int* packetSizes,
           int packetCount,
           ulong* pts,
           IntPtr userdata)
        {
            if (userdata == IntPtr.Zero || packetCount <= 0) return;
            var self = (EncoderCallback)GCHandle.FromIntPtr(userdata).Target!;

            try
            {
                for (int i = 0; i < packetCount; i++)
                {
                    IntPtr dataPtr = (IntPtr)packetData[i];
                    int size = packetSizes[i];
                    ulong timestamp = pts[i];

                    if (size > 0 && dataPtr != IntPtr.Zero)
                    {
                        // Hand the raw unmanaged pointer directly to the implementer
                        self.OnEncodedPacket(dataPtr, size, timestamp);
                    }
                }
            }
            catch (Exception ex)
            {
                // Must swallow exceptions to prevent tearing down the C++ runtime
                Console.Error.WriteLine("Error in OnEncoded callback: " + ex.Message);
            }
        }

        private static void OnEncodingFinishedThunk(IntPtr userdata)
        {
            if (userdata == IntPtr.Zero) return;
            var self = (EncoderCallback)GCHandle.FromIntPtr(userdata).Target!;
            try { self.OnEncodingFinished(); }
            catch { }
        }

        private static int SetFrameIntervalPThunk(int frameIntervalP, IntPtr userdata)
        {
            if (userdata == IntPtr.Zero) return 1;
            var self = (EncoderCallback)GCHandle.FromIntPtr(userdata).Target!;
            try { return self.SetFrameIntervalP(frameIntervalP) ? 1 : 0; }
            catch { return 1; }
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            if (_selfHandle.IsAllocated)
                _selfHandle.Free();
        }
    }
}