using OpenCvSharp.Internal;
using System.Runtime.InteropServices;

namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Struct providing information about video metadata and hardware decoder configuration.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 140)]
    public struct FormatInfo
    {
        [FieldOffset(0)] public Codec Codec;
        [FieldOffset(4)] public ChromaFormat ChromaFormat;
        [FieldOffset(8)] public SurfaceFormat SurfaceFormat;
        [FieldOffset(12)] public int NBitDepthMinus8;
        [FieldOffset(16)] public int NBitDepthChromaMinus8;
        [FieldOffset(20)] public int UlWidth;
        [FieldOffset(24)] public int UlHeight;
        [FieldOffset(28)] public int Width;
        [FieldOffset(32)] public int Height;
        [FieldOffset(36)] public int UlMaxWidth;
        [FieldOffset(40)] public int UlMaxHeight;
        [FieldOffset(44)] public Rect DisplayArea; // 16 bytes

        // Boolean 1: valid (at offset 60)
        [FieldOffset(60)] private byte _valid;

        // Double: fps (MUST be at offset 64 to align with 8-byte boundary)
        [FieldOffset(64)] public double Fps;

        [FieldOffset(72)] public int UlNumDecodeSurfaces;
        [FieldOffset(76)] public DeinterlaceMode DeinterlaceMode;
        [FieldOffset(80)] public Size TargetSz; // 8 bytes
        [FieldOffset(88)] public Rect SrcRoi;   // 16 bytes
        [FieldOffset(104)] public Rect TargetRoi; // 16 bytes

        // Boolean 2: videoFullRangeFlag (at offset 120)
        [FieldOffset(120)] private byte _videoFullRangeFlag;

        // Enum: colorSpaceStandard (at offset 124)
        [FieldOffset(124)] public ColorSpaceStandard ColorSpaceStandard;

        // Boolean 3: enableHistogram (at offset 128)
        [FieldOffset(128)] private byte _enableHistogram;

        [FieldOffset(132)] public int NCounterBitDepth;
        [FieldOffset(136)] public int NMaxHistogramBins;

        // --- Logic Properties ---

        public bool Valid
        {
            get => _valid != 0;
            set => _valid = (byte)(value ? 1 : 0);
        }

        public bool VideoFullRangeFlag
        {
            get => _videoFullRangeFlag != 0;
            set => _videoFullRangeFlag = (byte)(value ? 1 : 0);
        }

        public bool EnableHistogram
        {
            get => _enableHistogram != 0;
            set => _enableHistogram = (byte)(value ? 1 : 0);
        }

        public static FormatInfo Default()
        {
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_FormatInfo_GetNativeDefaults(out FormatInfo nativeDefaults));
            return nativeDefaults;
        }
    }
}