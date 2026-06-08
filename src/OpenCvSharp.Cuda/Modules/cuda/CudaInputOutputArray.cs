using System.Diagnostics.CodeAnalysis;

namespace OpenCvSharp.Cuda;

/// <summary>
/// Proxy data type for passing Mat's and vector&lt;&gt;'s as input parameters.
/// Synonym for OutputArray.
/// </summary>
public class CudaInputOutputArray : CudaOutputArray
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mat"></param>
    internal CudaInputOutputArray(GpuMat mat)
        : base(mat)
    {
    }

    /// <summary>
    /// Creates a proxy class of the specified Mat
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    public new static CudaInputOutputArray Create(GpuMat mat)
    {
        return new(mat);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="mat"></param>
    [SuppressMessage("Microsoft.Design", "CA2225: Operator overloads have named alternates")]
    public static implicit operator CudaInputOutputArray(GpuMat mat)
    {
        return new(mat);
    }

}