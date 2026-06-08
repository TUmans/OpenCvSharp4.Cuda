using OpenCvSharp.Cuda;
using OpenCvSharp.Internal;
using OpenCvSharp.Tests.Cuda;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCvSharp.Tests.Cudacodec
{
    public class CudaEncoderParamsTest : CudaTestBase
    {
        [Fact]
        public void EncoderParams_Default_Verification_Test()
        {
            // Skip if no CUDA
            VerifyCudaSupport();

            // Act: Always fetch the defaults directly from the OpenCV C++ constructor
            // through your new Default() method.
            var p = EncoderParams.Default();

            // Assert: Verify against the official OpenCV 4.13.0 source defaults.
            // These assertions prove that your [StructLayout] and padding are correct.

            Assert.Equal(EncodePreset.P3, p.NvPreset);
            Assert.Equal(EncodeTuningInfo.HighQuality, p.TuningInfo);
            Assert.Equal(EncodeProfile.AutoSelect, p.EncodingProfile);
            Assert.Equal(EncodeParamsRcMode.VBR, p.RateControlMode);
            Assert.Equal(EncodeMultiPass.Disabled, p.MultiPassEncoding);

            // Verify EncodeQp (P, B, Intra) - default is all 0
            Assert.Equal(0u, p.ConstQp.QpInterP);
            Assert.Equal(0u, p.ConstQp.QpInterB);
            Assert.Equal(0u, p.ConstQp.QpIntra);

            Assert.Equal(0, p.AverageBitRate);
            Assert.Equal(0, p.MaxBitRate);

            // This check is critical for verifying your TargetQuality padding bytes!
            Assert.Equal(30, p.TargetQuality);

            Assert.Equal(250, p.GopLength);
            Assert.Equal(250, p.IdrPeriod);

            // Verify your bool/byte property mapping
            Assert.False(p.VideoFullRangeFlag);
        }

        [Fact]
        public void EncoderParams_Default_RoundTrip()
        {
            var ep = EncoderParams.Default();

            // These are the hardcoded defaults from the OpenCV constructor
            Assert.Equal(EncodePreset.P3, ep.NvPreset);
            Assert.Equal(EncodeTuningInfo.HighQuality, ep.TuningInfo);
            Assert.Equal(EncodeProfile.AutoSelect, ep.EncodingProfile);
            Assert.Equal(EncodeParamsRcMode.VBR, ep.RateControlMode);
            Assert.Equal(EncodeMultiPass.Disabled, ep.MultiPassEncoding);
            Assert.Equal(0u, ep.ConstQp.QpInterP);
            Assert.Equal(0, ep.AverageBitRate);
            Assert.Equal(0, ep.MaxBitRate);
            Assert.Equal((byte)30, ep.TargetQuality);
            Assert.Equal(250, ep.GopLength);
            Assert.Equal(250, ep.IdrPeriod);
            Assert.False(ep.VideoFullRangeFlag);
        }
    }
}
