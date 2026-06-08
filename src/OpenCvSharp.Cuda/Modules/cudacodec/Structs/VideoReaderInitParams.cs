using OpenCvSharp.Internal;
using System.Runtime.InteropServices;

namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Initial parameters for the VideoReader.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct VideoReaderInitParams
    {
        [FieldOffset(0)] private byte _udpSource;
        [FieldOffset(1)] private byte _allowFrameDrop;
        [FieldOffset(4)] public int MinNumDecodeSurfaces;
        [FieldOffset(8)] private byte _rawMode;
        [FieldOffset(12)] public Size TargetSz; 
        [FieldOffset(20)] public Rect SrcRoi;   
        [FieldOffset(36)] public Rect TargetRoi;
        [FieldOffset(52)] private byte _enableHistogram;
        [FieldOffset(56)] public int FirstFrameIdx;

        public bool UdpSource
        {
            get => _udpSource != 0;
            set => _udpSource = (byte)(value ? 1 : 0);
        }

        public bool AllowFrameDrop
        {
            get => _allowFrameDrop != 0;
            set => _allowFrameDrop = (byte)(value ? 1 : 0);
        }

        public bool RawMode
        {
            get => _rawMode != 0;
            set => _rawMode = (byte)(value ? 1 : 0);
        }

        public bool EnableHistogram
        {
            get => _enableHistogram != 0;
            set => _enableHistogram = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Returns the default parameters initialized by the native OpenCV constructor.
        /// </summary>
        public static VideoReaderInitParams Default()
        {
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_VideoReaderInitParams_GetNativeDefaults(out var nativeDefaults));
            return nativeDefaults;
        }
    }
}