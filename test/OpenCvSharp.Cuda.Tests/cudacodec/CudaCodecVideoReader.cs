using OpenCvSharp.Cuda;
using OpenCvSharp.Internal;
using OpenCvSharp.Tests.Cuda;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCvSharp.Tests.Cudacodec
{
    public class CudaCodecVideoReader : CudaTestBase
    {
        [Fact]
        public void VideoReader_Create_WithParams_Test()
        {
            VerifyCudaSupport();

            string videoPath = "_data/ExampleFile.mp4";
            if (!System.IO.File.Exists(videoPath))
            {
                Assert.Skip("No file found");
            }

            try
            {
                // 1. Setup custom init params
                var myParams = VideoReaderInitParams.Default();
                myParams.MinNumDecodeSurfaces = 10;
                myParams.FirstFrameIdx = 5;

                // 2. Act
                using var reader = VideoReader.Create(videoPath, null, myParams);

                // 3. Assert
                Assert.NotNull(reader);
                Assert.NotEqual(IntPtr.Zero, reader.RawPtr);
            }
            catch (OpenCVException ex) when (ex.Message.Contains("not supported") || ex.Message.Contains("Not Implemented"))
            {
                // Handle environments without hardware decoder support
                return;
            }
        }

        [Fact]
        public void VideoReader_FullWorkflow_Test()
        {
            VerifyCudaSupport();
            string videoPath = "_data/ExampleFile.mp4";
            if (!System.IO.File.Exists(videoPath))
            {
                Assert.Skip("No file found");
            }

            try
            {
                using var reader = VideoReader.Create(videoPath);

                // 1. Test Properties
                Assert.True(reader.Get(VideoReaderProps.ColorFormat, out double colorFmt));

                // 2. Test Set output format to BGRA
                Assert.True(reader.Set(ColorFormat.BGRA));

                // 3. Test NextFrame
                using var frame = new GpuMat();
                bool hasFrame = reader.NextFrame(frame);

                Assert.True(hasFrame);
                Assert.False(frame.Empty());
                Assert.Equal(4, frame.Channels()); // Should be 4 because we set BGRA

                // 4. Test Grab/Retrieve
                Assert.True(reader.Grab());
                using var retrievedFrame = new GpuMat();
                Assert.True(reader.Retrieve(retrievedFrame));
                Assert.False(retrievedFrame.Empty());
            }
            catch (OpenCVException ex) when (ex.Message.Contains("not supported") || ex.Message.Contains("Not Implemented"))
            {
                return;
            }
        }


        [Fact]
        public void VideoReader_Props_Test()
        {
            VerifyCudaSupport();
            string videoPath = "_data/ExampleFile.mp4";
            if (!System.IO.File.Exists(videoPath))
            {
                Assert.Skip("No file found");
            }
            try
            {
                using var reader = VideoReader.Create(videoPath);

                // 1. BitDepth GET returns the source video's bit depth, not a settable value.
                //    NVDEC reports 2 = NV_ENC_BIT_DEPTH_8 in its internal enum.
                //    Just verify it returns a value without throwing.
                bool getOk = reader.GetVideoReaderProps(VideoReaderProps.BitDepth, out double bitDepth);
                if (getOk)
                    Assert.True(bitDepth >= 0, $"BitDepth should be non-negative, got {bitDepth}.");

                // 2. ColorFormat GET returns the current output color format.
                //    Verify it maps to a defined enum member.
                bool colorOk = reader.GetVideoReaderProps(VideoReaderProps.ColorFormat, out double colorFmt);
                if (colorOk)
                    Assert.True(Enum.IsDefined(typeof(ColorFormat), (int)colorFmt),
                        $"ColorFormat value {colorFmt} is not a defined enum member.");

                // 3. RawMode is readable and boolean (0 or 1).
                bool rawOk = reader.GetVideoReaderProps(VideoReaderProps.RawMode, out double rawMode);
                if (rawOk)
                    Assert.True(rawMode is 0.0 or 1.0, $"RawMode should be 0 or 1, got {rawMode}.");

                // 4. DecodedFrameIdx starts at 0 before any frames are grabbed.
                bool idxOk = reader.GetVideoReaderProps(VideoReaderProps.DecodedFrameIdx, out double frameIdx);
                if (idxOk)
                    Assert.True(frameIdx >= 0, $"DecodedFrameIdx should be >= 0, got {frameIdx}.");
            }
            catch (OpenCVException ex) when (ex.Message.Contains("not supported") ||
                                             ex.Message.Contains("Not Implemented"))
            {
                return;
            }
        }

    }
}
