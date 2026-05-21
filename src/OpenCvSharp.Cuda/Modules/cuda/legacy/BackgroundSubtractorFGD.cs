using System.Runtime.InteropServices;
using OpenCvSharp.Internal;
using OpenCvSharp.Internal.Vectors;

namespace OpenCvSharp.Cuda;

public class BackgroundSubtractorFGD : BackgroundSubtractor
{
    protected BackgroundSubtractorFGD(IntPtr smartPtr, IntPtr rawPtr)
         : base(smartPtr, rawPtr, p=> NativeMethods.HandleException(NativeMethods_cuda.BackgroundSubtractorFGD_delete(p)))
    {
    }

    /// <summary>
    /// Creates an FGD Background Subtractor using default parameters.
    /// </summary>
    /// <returns></returns>
    public static BackgroundSubtractorFGD Create()
    {
        NativeMethods.HandleException(
            NativeMethods_cuda.createBackgroundSubtractorFGD(out var smartPtr));

        NativeMethods.HandleException(NativeMethods_cuda.BackgroundSubtractorFGD_get(smartPtr, out IntPtr rawPtr));

        return new BackgroundSubtractorFGD(smartPtr, rawPtr);
    }

    /// <summary>
    /// Creates an FGD Background Subtractor using custom parameters.
    /// </summary>
    /// <param name="params"></param>
    /// <returns></returns>
    public static BackgroundSubtractorFGD Create(FGDParams? @params = null)
    {
        IntPtr smartPtr;
        if (@params.HasValue)
        {
            NativeMethods.HandleException(
                NativeMethods_cuda.createBackgroundSubtractorFGD_withParams(@params.Value, out smartPtr));
        }
        else
        {
            // Use your existing cuda_createBackgroundSubtractorFGD (no params)
            NativeMethods.HandleException(
                NativeMethods_cuda.createBackgroundSubtractorFGD(out smartPtr));
        }

        NativeMethods.HandleException(NativeMethods_cuda.BackgroundSubtractorFGD_get(smartPtr, out IntPtr rawPtr));
        return new BackgroundSubtractorFGD(smartPtr, rawPtr);
    }

    /// <summary>
    /// Updates the background model and computes the foreground mask, with CUDA Stream support.
    /// </summary>
    public virtual void Apply(OpenCvSharp.Cuda.CudaInputArray image, OpenCvSharp.Cuda.CudaOutputArray fgmask, double learningRate = -1)
    {
        if (image is null) 
            throw new ArgumentNullException(nameof(image));
        if (fgmask is null) 
            throw new ArgumentNullException(nameof(fgmask));

        image.ThrowIfDisposed();
        fgmask.ThrowIfNotReady();
        ThrowIfDisposed();

        NativeMethods.HandleException(
            NativeMethods_cuda.BackgroundSubtractorFGD_apply(
                RawPtr, image.CvPtr, fgmask.CvPtr, learningRate));

        fgmask.Fix();
        GC.KeepAlive(this);
        GC.KeepAlive(image);
    }

    /// <summary>
    /// Returns the foreground regions found by the algorithm.
    /// </summary>
    /// <returns>An array of CPU Mat objects containing the foreground regions.</returns>
    public Mat[] GetForegroundRegions()
    {
        ThrowIfDisposed();
        using var matVec = new  VectorOfMat();

        NativeMethods.HandleException(
            NativeMethods_cuda.BackgroundSubtractorFGD_getForegroundRegions(
                RawPtr, matVec.CvPtr));

        GC.KeepAlive(this);
        return matVec.ToArray();
    }
}