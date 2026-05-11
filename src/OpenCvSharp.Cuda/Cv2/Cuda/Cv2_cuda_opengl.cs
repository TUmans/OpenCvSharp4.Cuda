using OpenCvSharp.Internal;

namespace OpenCvSharp;

public static partial class Cv2Cuda
{
    #region OpenGL Device
    /// <summary>
    /// Sets a CUDA device and initializes it for the current thread with OpenGL interoperability.
    /// This must be called before any other CUDA calls on the current thread.
    /// </summary>
    /// <param name="device">Device ID.</param>
    public static void SetGlDevice(int device = 0)
    {
        NativeMethods.HandleException(
            NativeMethods_cuda.cuda_setGlDevice(device));
    }

    #endregion
}

