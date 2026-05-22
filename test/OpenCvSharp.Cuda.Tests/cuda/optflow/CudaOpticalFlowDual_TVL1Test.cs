using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp.Cuda;
using Xunit;

namespace OpenCvSharp.Tests.Cuda.optflow;

public class CudaOpticalFlowDual_TVL1Test : CudaTestBase
{
    [Fact]
    public void DualTVL1_PropertiesAndCalc()
    {
        VerifyCudaSupport();

        try
        {
            using var tvl1 = OpenCvSharp.Cuda.OpticalFlowDual_TVL1.Create(iterations: 100);

            tvl1.Tau = 0.2;
            Assert.Equal(0.2, tvl1.Tau);

            tvl1.NumScales = 3;
            Assert.Equal(3, tvl1.NumScales);

            tvl1.UseInitialFlow = false;

            using var cpu1 = new Mat(256, 256, MatType.CV_8UC1, Scalar.Black);
            Cv2.Rectangle(cpu1, new Rect(60, 60, 80, 80), Scalar.White, -1);
            Cv2.GaussianBlur(cpu1, cpu1, new Size(5, 5), 0);

            using var cpu2 = new Mat(256, 256, MatType.CV_8UC1, Scalar.Black);
            Cv2.Rectangle(cpu2, new Rect(70, 60, 80, 80), Scalar.White, -1);
            Cv2.GaussianBlur(cpu2, cpu2, new Size(5, 5), 0);

            using var gpu1 = new GpuMat();
            using var gpu2 = new GpuMat();

            gpu1.Upload(cpu1);
            gpu2.Upload(cpu2);

            using var flow = new GpuMat();

            tvl1.Calc(gpu1, gpu2, flow);

            using var flowCpu = new Mat();
            flow.Download(flowCpu);

            int positive = 0;
            int total = 0;

            for (int y = 80; y < 120; y++)
            {
                for (int x = 80; x < 120; x++)
                {
                    Vec2f v = flowCpu.At<Vec2f>(y, x);

                    if (!float.IsNaN(v.Item0))
                    {
                        if (v.Item0 > 0)
                            positive++;

                        total++;
                    }
                }
            }

            Assert.True((float)positive / total > 0.7f);
        }
        catch (OpenCVException ex) when (
            ex.Message.Contains("disabled") ||
            ex.Message.Contains("Not Implemented"))
        {
            return;
        }
    }
}
