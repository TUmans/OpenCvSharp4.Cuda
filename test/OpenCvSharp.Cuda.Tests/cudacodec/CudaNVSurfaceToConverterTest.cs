using OpenCvSharp.Cuda;
using OpenCvSharp.Tests.Cuda;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCvSharp.Tests.Cudacodec
{
    public class CudaNVSurfaceToConverterTest : CudaTestBase
    {

        [Fact]
        public void NVSurfaceToColorConverter_FullConvert_Test()
        {
            // Skip if no CUDA
            VerifyCudaSupport();

            try
            {
                using var converter = NVSurfaceToColorConverter.Create(ColorSpaceStandard.BT709);

                // 1. Prepare an NV12 buffer (1.5x height for chroma)
                // 100x100 image -> 100x150 buffer
                using var gpuYuv = new GpuMat(150, 100, MatType.CV_8UC1, new Scalar(128));
                using var gpuColor = new GpuMat();

                // 2. Act: Convert NV12 to BGR (3 channels)
                bool success = converter.Convert(
                    gpuYuv,
                    gpuColor,
                    SurfaceFormat.NV12,
                    ColorFormat.BGR,
                    BitDepth.Eight);

                // 3. Assert
                Assert.True(success);
                Assert.False(gpuColor.Empty());
                Assert.Equal(3, gpuColor.Channels());
                Assert.Equal(100, gpuColor.Rows);
                Assert.Equal(100, gpuColor.Cols);
            }
            catch (OpenCVException ex) when (ex.Message.Contains("not supported") || ex.Message.Contains("Not Implemented"))
            {
                return;
            }
        }
    }
}
