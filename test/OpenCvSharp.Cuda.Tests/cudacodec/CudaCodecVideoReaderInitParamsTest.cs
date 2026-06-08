using OpenCvSharp.Cuda;
using OpenCvSharp.Internal;
using OpenCvSharp.Tests.Cuda;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCvSharp.Tests.Cudacodec
{
    public class CudaCodecVideoReaderInitParamsTest : CudaTestBase
    {
        [Fact]
        public void VideoReaderInitParams_Default_Test()
        {
            // Act: Get defaults from C++
            var p = VideoReaderInitParams.Default();

            // Assert: Verify all constructor defaults
            Assert.False(p.UdpSource);
            Assert.False(p.AllowFrameDrop);
            Assert.Equal(0, p.MinNumDecodeSurfaces);
            Assert.False(p.RawMode);

            // Size and Rects should be empty/zero
            Assert.Equal(0, p.TargetSz.Width);
            Assert.Equal(0, p.SrcRoi.Width);

            Assert.False(p.EnableHistogram);
            Assert.Equal(0, p.FirstFrameIdx);

            // Test Round-trip logic
            p.RawMode = true;
            Assert.True(p.RawMode);
        }

    }
}
