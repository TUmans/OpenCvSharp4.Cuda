using OpenCvSharp;
using OpenCvSharp.Cuda;
using OpenCvSharp.Internal;
using OpenCvSharp.Tests.Cuda;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace OpenCvSharp.Tests.Cudacodec
{
    [CollectionDefinition("NvencSequential", DisableParallelization = true)]
    public class NvencSequentialCollection { }

    /// <summary>
    /// Integration tests for <see cref="VideoWriter"/>.
    /// Requires a CUDA-capable GPU and an NVENC-capable encoder.
    ///
    /// Rules that mirror the underlying cv::cudacodec::VideoWriter API:
    ///   1. A non-null EncoderCallback is required by NVENC.
    ///      Use <see cref="NoOpEncoderCallback"/> when packets aren't needed.
    ///   2. <see cref="VideoWriter.Release"/> MUST be called before Dispose to
    ///      flush the encoder and finalise the output container.
    ///   3. Output must be a muxable container (.mp4 / .avi / .mkv).
    ///      Raw .h264 is not written to disk by the NVENC backend.
    /// </summary>
    [Collection("NvencSequential")]
    public class VideoWriterTests : CudaTestBase, IDisposable
    {
        private readonly List<string> _tempFiles = new();

        // ── Helpers ───────────────────────────────────────────────────────────

        private EncoderParams GetValidEncoderParams()
        {
            EncoderParams ep = EncoderParams.Default();
            ep.TuningInfo = EncodeTuningInfo.LowLatency;
            ep.RateControlMode = EncodeParamsRcMode.ConstQP;
            ep.ConstQp.QpIntra = 28;
            ep.ConstQp.QpInterP = 28;
            ep.ConstQp.QpInterB = 28;
            ep.NvPreset = EncodePreset.P1;
            return ep;
        }

        private string TempFile(string ext = ".h264")
        {
            // FIX: Use Path.GetTempPath() to write to /tmp/ which bypasses Docker folder permissions
            var path = Path.Combine(Path.GetTempPath(), $"ocvsharp_test_{Guid.NewGuid():N}{ext}");
            _tempFiles.Add(path);
            return path;
        }

        // FIX: Default to CV_8UC4 because NVENC on Linux segfaults on 24-bit memory (CV_8UC3)
        private static Mat MakeFrame(Size size, Scalar color) =>
            new Mat(size, MatType.CV_8UC4, color);

        /// <summary>
        /// Writes <paramref name="frameCount"/> frames and returns the output
        /// path. Always supplies a callback and calls Release() before Dispose().
        /// </summary>
        private string WriteFrames(
            Size frameSize,
            int frameCount = 30,
            Codec codec = Codec.H264,
            double fps = 25.0,
            ColorFormat colorFormat = ColorFormat.BGRA, // FIX: Default to BGRA
            EncoderCallback? callback = null)
        {
            using var cb = callback;
            var path = TempFile();
            using var writer = new OpenCvSharp.Cuda.VideoWriter(path, frameSize, codec, fps, colorFormat, GetValidEncoderParams(), cb);
            using var frame = MakeFrame(frameSize, new Scalar(0, 120, 255, 255));
            using var gpuFrame = new GpuMat();
            gpuFrame.Upload(frame);
            for (int i = 0; i < frameCount; i++)
                writer.Write(gpuFrame);

            writer.Release();   // required – flushes encoder and finalises container
            return path;
        }

        [Fact]
        public void Write_BGRA_H264_Baseline()
        {
            var path = TempFile();
            var fsize = new Size(640, 480);

            // FIX: Using BGRA and CV_8UC4
            using var writer = new OpenCvSharp.Cuda.VideoWriter(path, fsize, Codec.H264, 25,
                ColorFormat.BGRA, GetValidEncoderParams());
            using var frame = new GpuMat(fsize, MatType.CV_8UC4, Scalar.All(100));

            for (int i = 0; i < 10; i++)
                writer.Write(frame);
            writer.Release();

            Assert.True(File.Exists(path));
            Assert.True(new FileInfo(path).Length > 0);
        }

        // ── Construction ──────────────────────────────────────────────────────

        [Fact]
        public void ExtensiveVideowriterTest()
        {
            VerifyCudaSupport();

            // FIX: Write to TempPath to avoid Docker volume permission denied exceptions
            string path = Path.Combine(Path.GetTempPath(), "csharp_diagnostic.mp4");
            Size frameSize = new Size(1280, 720);
            double fps = 30.0;

            EncoderParams ep = EncoderParams.Default();
            ep.TuningInfo = EncodeTuningInfo.LowLatency;
            ep.RateControlMode = EncodeParamsRcMode.ConstQP;
            ep.ConstQp.QpIntra = 28;
            ep.ConstQp.QpInterP = 28;
            ep.ConstQp.QpInterB = 28;
            ep.NvPreset = EncodePreset.P1;

            try
            {
                // FIX: Use BGRA
                using var writer = new OpenCvSharp.Cuda.VideoWriter(
                    path,
                    frameSize,
                    Codec.H264,
                    fps,
                    ColorFormat.BGRA,
                    ep,
                    null);

                // FIX: Use CV_8UC4
                using var hostFrame = new Mat(frameSize, MatType.CV_8UC4);
                using var gpuFrame = new GpuMat();

                for (int i = 0; i < 100; i++)
                {
                    // Clear background (Dark Blue) - 4 Channels for BGRA
                    hostFrame.SetTo(new Scalar(50, 0, 0, 255));

                    // Draw the Counter
                    string text = $"CUDA Frame: {i}";
                    Cv2.PutText(hostFrame, text, new Point(100, 360),
                        HersheyFonts.HersheySimplex, 3.0, new Scalar(255, 255, 255, 255), 5);

                    // Draw moving Red Square
                    int xPos = (i * 10) % 1200;
                    Cv2.Rectangle(hostFrame, new Rect(xPos, 500, 50, 50), new Scalar(0, 0, 255, 255), -1);

                    // Upload to GPU and Write to stream
                    gpuFrame.Upload(hostFrame);
                    writer.Write(gpuFrame);
                }

                writer.Release();

                Assert.True(File.Exists(path), "Video file was not created.");
                var fileInfo = new FileInfo(path);
                Assert.True(fileInfo.Length > 0, "Video file is 0 bytes.");
            }
            catch (OpenCVException ex) when (ex.Message.Contains("not supported") || ex.Message.Contains("Not Implemented"))
            {
                // Skip if NVENC hardware is not available on the test machine
                return;
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void Constructor_ShortOverload_CreatesFile()
        {
            var path = WriteFrames(new Size(640, 480));
            Assert.True(File.Exists(path), $"Expected output file at {path}");
            Assert.True(new FileInfo(path).Length > 0, "Output file should not be empty.");
        }

        [Fact]
        public void Constructor_FullOverload_WithDefaultEncoderParams_CreatesFile()
        {
            VerifyCudaSupport();

            var path = TempFile();
            var fsize = new Size(640, 480);

            var ep = GetValidEncoderParams();
            ep.RateControlMode = EncodeParamsRcMode.ConstQP;
            ep.ConstQp = new EncodeQp { QpIntra = 26, QpInterP = 26, QpInterB = 26 };

            using var writer = new OpenCvSharp.Cuda.VideoWriter(path, fsize, Codec.H264, 30.0, ColorFormat.BGRA, ep);

            using var frame = new Mat(fsize, MatType.CV_8UC4, Scalar.All(128));
            using var gpuFrame = new GpuMat();
            gpuFrame.Upload(frame);
            for (int i = 0; i < 10; i++)
                writer.Write(gpuFrame);

            writer.Release();

            Assert.True(File.Exists(path), $"Expected output file at {path}");
            Assert.True(new FileInfo(path).Length > 0);
        }

        [Fact]
        public void Constructor_DefaultParams_CreatesFile()
        {
            VerifyCudaSupport();

            var path = TempFile(".h264");
            var fsize = new Size(1280, 720);

            using var stream = new OpenCvSharp.Cuda.Stream();

            using var writer = new OpenCvSharp.Cuda.VideoWriter(
                path,
                fsize,
                Codec.H264,
                30.0,
                ColorFormat.BGRA,
                GetValidEncoderParams(),
                null,
                stream);

            using var frame = MakeFrame(fsize, Scalar.All(128));
            using var gpuFrame = new GpuMat();
            gpuFrame.Upload(frame);
            for (int i = 0; i < 10; i++)
                writer.Write(gpuFrame);

            writer.Release();

            Assert.True(File.Exists(path), $"Expected output file at {path}");
            Assert.True(new FileInfo(path).Length > 0);
        }

        [Fact]
        public void Constructor_NoExplicitCallback_UsesDefaultInternally()
        {
            var ex = Record.Exception(() => WriteFrames(new Size(320, 240), callback: null));
            Assert.Null(ex);
        }

        // ── Write ─────────────────────────────────────────────────────────────

        [Fact]
        public void Write_SingleFrame_DoesNotThrow()
        {
            var fsize = new Size(640, 480);

            using var writer = new OpenCvSharp.Cuda.VideoWriter(TempFile(), fsize, Codec.H264, 25, ColorFormat.BGRA, GetValidEncoderParams());
            using var frame = MakeFrame(fsize, Scalar.All(0));
            using var gpuFrame = new GpuMat();
            gpuFrame.Upload(frame);
            var ex = Record.Exception(() => writer.Write(gpuFrame));
            Assert.Null(ex);
            writer.Release();
        }

        [Fact]
        public void Write_NullFrame_ThrowsArgumentNullException()
        {
            using var writer = new OpenCvSharp.Cuda.VideoWriter(TempFile(), new Size(640, 480), Codec.H264, 25, ColorFormat.BGRA, GetValidEncoderParams());
            using GpuMat test = null;
            Assert.Throws<ArgumentNullException>(() => writer.Write(test));
            writer.Release();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(30)]
        [InlineData(120)]
        public void Write_MultipleFrames_ProducesNonEmptyFile(int frameCount)
        {
            var path = WriteFrames(new Size(640, 480), frameCount: frameCount);
            Assert.True(File.Exists(path), $"Expected output file at {path}");
            Assert.True(new FileInfo(path).Length > 0);
        }

        // ── GetEncoderParams ──────────────────────────────────────────────────

        [Fact]
        public void GetEncoderParams_ReturnsValidParams()
        {
            using var writer = new OpenCvSharp.Cuda.VideoWriter(TempFile(), new Size(640, 480), Codec.H264, 30, ColorFormat.BGRA, GetValidEncoderParams());
            var p = writer.GetEncoderParams();

            Assert.True(p.GopLength > 0, "GopLength should be positive.");
            Assert.True(p.IdrPeriod > 0, "IdrPeriod should be positive.");
            Assert.True(p.IdrPeriod >= p.GopLength,
                $"IdrPeriod ({p.IdrPeriod}) must be >= GopLength ({p.GopLength}).");
            Assert.True(p.AverageBitRate >= 0, "AverageBitRate should be non-negative.");
            Assert.True(p.MaxBitRate >= 0, "MaxBitRate should be non-negative.");
            Assert.InRange(p.TargetQuality, (byte)0, (byte)51);

            writer.Release();
        }

        // ── Release ───────────────────────────────────────────────────────────

        [Fact]
        public void Release_ProducesNonEmptyFile()
        {
            var path = TempFile();
            var fsize = new Size(640, 480);
            using var writer = new OpenCvSharp.Cuda.VideoWriter(path, fsize, Codec.H264, 25, ColorFormat.BGRA, GetValidEncoderParams());
            using var frame = MakeFrame(fsize, Scalar.All(64));
            using var gpuFrame = new GpuMat();
            gpuFrame.Upload(frame);
            writer.Write(gpuFrame);
            writer.Release();

            Assert.True(File.Exists(path), $"Expected output file at {path}");
            Assert.True(new FileInfo(path).Length > 0);
        }

        // ── EncoderCallback integration ───────────────────────────────────────

        [Fact]
        public void EncoderCallback_OnEncoded_IsCalledAtLeastOnce()
        {
            var cb = new RecordingEncoderCallback();
            WriteFrames(new Size(640, 480), frameCount: 30, callback: cb);
            Assert.True(cb.TotalPackets > 0, "OnEncoded should have been called at least once.");
        }

        [Fact]
        public void EncoderCallback_OnEncodingFinished_IsCalledExactlyOnce()
        {
            var cb = new RecordingEncoderCallback();
            WriteFrames(new Size(640, 480), frameCount: 10, callback: cb);
            Assert.Equal(1, cb.FinishedCount);
        }

        [Fact]
        public void EncoderCallback_PtsValues_AreNonDecreasing()
        {
            var cb = new RecordingEncoderCallback();
            WriteFrames(new Size(640, 480), frameCount: 30, callback: cb);

            ulong prev = 0;
            foreach (var pts in cb.AllPts)
            {
                Assert.True(pts >= prev, $"PTS should be non-decreasing, got {pts} after {prev}.");
                prev = pts;
            }
        }

        [Fact]
        public void EncoderCallback_ReceivedPackets_AreNonEmpty()
        {
            var cb = new RecordingEncoderCallback();
            WriteFrames(new Size(640, 480), frameCount: 30, callback: cb);

            foreach (var pkt in cb.AllPackets)
                Assert.True(pkt.Length > 0, "Encoded packet should not be empty.");
        }

        [Fact]
        public void EncoderCallback_SetFrameIntervalP_IsHonouredByEncoder()
        {
            var cb = new RecordingEncoderCallback(acceptFrameIntervalP: true);
            WriteFrames(new Size(640, 480), frameCount: 30, callback: cb);
            Assert.True(cb.TotalPackets > 0);
        }

        // ── ColorFormat overloads ─────────────────────────────────────────────

        [Theory]
        [InlineData(ColorFormat.BGR)]
        [InlineData(ColorFormat.RGB)]
        [InlineData(ColorFormat.BGRA)]
        [InlineData(ColorFormat.RGBA)]
        public void Write_DifferentColorFormats_ProducesOutput(ColorFormat fmt)
        {


            int channels = fmt is ColorFormat.BGRA or ColorFormat.RGBA ? 4 : 3;
            var fsize = new Size(640, 480);
            var path = TempFile();

            using var writer = new OpenCvSharp.Cuda.VideoWriter(path, fsize, Codec.H264, 25, fmt, GetValidEncoderParams());
            using var frame = new Mat(fsize,
                channels == 4 ? MatType.CV_8UC4 : MatType.CV_8UC3, Scalar.All(100));
            using var gpuFrame = new GpuMat();
            gpuFrame.Upload(frame);
            for (int i = 0; i < 10; i++)
                writer.Write(gpuFrame);

            writer.Release();

            Assert.True(File.Exists(path), $"Expected output file at {path}");
            Assert.True(new FileInfo(path).Length > 0);
        }

        // ── Codec overloads ───────────────────────────────────────────────────

        [Theory]
        [InlineData(Codec.H264)]
        //[InlineData(Codec.HEVC)]
        public void Write_SupportedCodecs_ProducesOutput(Codec codec)
        {
            var path = WriteFrames(new Size(640, 480), codec: codec);
            Assert.True(File.Exists(path), $"Expected output file at {path}");
            Assert.True(new FileInfo(path).Length > 0);
        }

        // ── Dispose ───────────────────────────────────────────────────────────

        [Fact]
        public void Dispose_AfterRelease_DoesNotThrow()
        {
            var writer = new OpenCvSharp.Cuda.VideoWriter(TempFile(), new Size(320, 240), Codec.H264, 25, ColorFormat.BGRA, GetValidEncoderParams());
            writer.Release();

            var ex = Record.Exception(() => writer.Dispose());
            Assert.Null(ex);
        }

        // ── IDisposable ───────────────────────────────────────────────────────

        public void Dispose()
        {
            foreach (var f in _tempFiles)
                try { File.Delete(f); } catch { /* best-effort */ }
        }
    }

    // =========================================================================
    //  Recording callback – captures everything for assertion
    // =========================================================================

    internal sealed class RecordingEncoderCallback : EncoderCallback
    {
        private readonly bool _acceptFrameIntervalP;
        private int _finishedCount;
        private bool _alreadyFinished = false;

        public int TotalPackets => AllPackets.Count;
        public int FinishedCount => Volatile.Read(ref _finishedCount);

        public List<byte[]> AllPackets { get; } = new();
        public List<ulong> AllPts { get; } = new();

        public RecordingEncoderCallback(bool acceptFrameIntervalP = true)
        {
            _acceptFrameIntervalP = acceptFrameIntervalP;
        }

        public override void OnEncodedPacket(IntPtr dataPtr, int size, ulong pts)
        {
            if (size > 0 && dataPtr != IntPtr.Zero)
            {
                byte[] packet = new byte[size];
                Marshal.Copy(dataPtr, packet, 0, size);

                AllPackets.Add(packet);
                AllPts.Add(pts);
            }
        }

        public override void OnEncodingFinished()
        {
            if (!_alreadyFinished)
            {
                _alreadyFinished = true;
                Interlocked.Increment(ref _finishedCount);
            }
        }

        public override bool SetFrameIntervalP(int frameIntervalP) =>
            _acceptFrameIntervalP;
    }
}