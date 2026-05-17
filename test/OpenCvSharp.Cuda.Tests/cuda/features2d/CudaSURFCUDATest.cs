using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp.Cuda;
using Xunit;

namespace OpenCvSharp.Tests.Cuda.features2d;

public class CudaSURFCUDATest : CudaTestBase
{
    [Fact]
    public void SURFCUDA()
    {
        VerifyCudaSupport();

        try
        {
            // INCREASE IMAGE SIZE FROM 100x100 TO 256x256
            using var cpuImg = new Mat(256, 256, MatType.CV_8UC1, new Scalar(0));
            // Draw a larger rectangle so there are features to detect
            Cv2.Rectangle(cpuImg, new Rect(50, 50, 100, 100), new Scalar(255), -1);

            using var gpuImg = new GpuMat(); gpuImg.Upload(cpuImg);
            using var gpuKeypoints = new GpuMat();
            using var gpuDescriptors = new GpuMat();

            using var surf = SURF_CUDA.Create(hessianThreshold: 100);

            using var emptyMask = new GpuMat(); // Using your new fix!
            surf.DetectWithDescriptors(gpuImg, emptyMask, gpuKeypoints, gpuDescriptors);

            KeyPoint[] kps = surf.DownloadKeypoints(gpuKeypoints);
            float[] descriptors = surf.DownloadDescriptors(gpuDescriptors);

            Assert.True(kps.Length > 0);
            Assert.Equal(kps.Length * surf.DescriptorSize, descriptors.Length);
        }
        catch (OpenCVException ex) when (ex.Message.Contains("Set OPENCV_ENABLE_NONFREE CMake"))
        {
            Assert.Skip(ex.Message);
        }
    }
}
