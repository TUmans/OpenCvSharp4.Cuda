using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using OpenCvSharp.Internal;
using OpenCvSharp.Internal.Vectors;

namespace OpenCvSharp.Cuda;

public class CudaOutputArray : CvObject
{
    private readonly object obj;

    #region Init & Disposal

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mat"></param>
    internal CudaOutputArray(GpuMat mat)
    {
        if (mat is null)
            throw new ArgumentNullException(nameof(mat));
        NativeMethods.HandleException(NativeMethods_cuda.cuda_OutputArray_new_byGpuMat(mat.CvPtr, out var p));
        GC.KeepAlive(mat);
        obj = mat;
        InitSafeHandle(p);
    }

    /// <summary>
    /// Releases unmanaged resources
    /// </summary>

    private void InitSafeHandle(IntPtr p, bool ownsHandle = true)
    {
        SetSafeHandle(new OpenCvCudaPtrSafeHandle(p, ownsHandle,
            static h => NativeMethods.HandleException(NativeMethods.core_OutputArray_delete(h))));
    }

    #endregion

    #region Cast

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    public static implicit operator CudaOutputArray(GpuMat mat)
    {
        return new CudaOutputArray(mat);
    }


    #endregion

    #region Methods

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsGpuMat()
    {
        return obj is GpuMat;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual Mat GetGpuMat()
    {
        return obj as GpuMat;
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void AssignResult()
    {
        if (!IsReady())
            throw new NotSupportedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Fix()
    {
        AssignResult();
        Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsReady()
    {
        return
            ptr != IntPtr.Zero &&
            !IsDisposed && IsGpuMat();

    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public void ThrowIfNotReady()
    {
        if (!IsReady())
            throw new OpenCvSharpException("Invalid OutputArray");
    }

    /// <summary>
    /// Creates a proxy class of the specified matrix
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    public static CudaOutputArray Create(GpuMat mat)
    {
        return new CudaOutputArray(mat);
    }

    #endregion
}
