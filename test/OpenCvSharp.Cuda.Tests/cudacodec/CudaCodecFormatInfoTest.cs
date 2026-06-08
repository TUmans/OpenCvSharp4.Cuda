using OpenCvSharp.Cuda;
using OpenCvSharp.Internal;
using OpenCvSharp.Tests.Cuda;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCvSharp.Tests.Cudacodec
{
    public class CudaCodecFormatInfoTest : CudaTestBase
    {
        [Fact]
        public void FormatInfo_Default_Verification_Test()
        {
            // Skip if no CUDA
            VerifyCudaSupport();

            // Act: Fetch defaults from the actual C++ constructor
            var info = FormatInfo.Default();

            // Assert: Verify against official OpenCV source code defaults

            // Check initial integers
            Assert.Equal(-1, info.NBitDepthMinus8);
            Assert.Equal(0, info.UlWidth);
            Assert.Equal(0, info.Width);
            Assert.Equal(0, info.Height);

            // Check Boolean (stored as byte internally)
            Assert.False(info.Valid);

            // Check Double (Verify 8-byte alignment)
            Assert.Equal(0.0, info.Fps);

            // Check Enum (Verify 4-byte alignment)
            Assert.Equal(ColorSpaceStandard.BT601, info.ColorSpaceStandard);

            // Check Histogram settings
            Assert.False(info.EnableHistogram);
            Assert.Equal(0, info.NCounterBitDepth);
            Assert.Equal(0, info.NMaxHistogramBins);
        }
    }
}
