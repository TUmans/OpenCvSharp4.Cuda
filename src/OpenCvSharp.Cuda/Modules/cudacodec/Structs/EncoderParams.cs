using OpenCvSharp.Internal;
using System;
using System.Runtime.InteropServices;

namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Parameters for the NVIDIA hardware video encoder.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 56)]
    public struct EncoderParams
    {
        [FieldOffset(0)] public EncodePreset NvPreset;
        [FieldOffset(4)] public EncodeTuningInfo TuningInfo;
        [FieldOffset(8)] public EncodeProfile EncodingProfile;
        [FieldOffset(12)] public EncodeParamsRcMode RateControlMode;
        [FieldOffset(16)] public EncodeMultiPass MultiPassEncoding;

        // EncodeQp is 12 bytes (3 x uint32)
        [FieldOffset(20)] public EncodeQp ConstQp;

        [FieldOffset(32)] public int AverageBitRate;
        [FieldOffset(36)] public int MaxBitRate;

        // Byte 40: targetQuality (uint8_t)
        [FieldOffset(40)] public byte TargetQuality;

        // Bytes 41, 42, 43 are padding added by C++ to align the next int

        [FieldOffset(44)] public int GopLength;
        [FieldOffset(48)] public int IdrPeriod;

        // Byte 52: videoFullRangeFlag (bool / uint8_t)
        [FieldOffset(52)] private byte _videoFullRangeFlag;

        // Bytes 53, 54, 55 are trailing padding to make the total size 56

        /// <summary>
        /// Indicates if the black level, luma and chroma of the source are represented using the full or limited range.
        /// </summary>
        public bool VideoFullRangeFlag
        {
            get => _videoFullRangeFlag != 0;
            set => _videoFullRangeFlag = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Fetches the default encoder parameters from the native OpenCV constructor.
        /// </summary>
        public static EncoderParams Default()
        {
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_EncoderParams_GetNativeDefaults(out EncoderParams nativeDefaults));
            return nativeDefaults;
        }
    }
}