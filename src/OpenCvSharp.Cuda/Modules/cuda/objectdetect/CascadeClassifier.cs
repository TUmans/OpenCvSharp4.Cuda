using System;
using System.Runtime.InteropServices;
using OpenCvSharp.Internal;
using OpenCvSharp.Internal.Vectors;

namespace OpenCvSharp.Cuda
{
    /// <summary>
    /// Cascade classifier class for object detection.
    /// </summary>
    public class CascadeClassifier : Algorithm
    {
        protected CascadeClassifier(IntPtr smartPtr, IntPtr rawPtr)
            : base(smartPtr, rawPtr, p => NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_delete(p)))
        {
        }

        public static CascadeClassifier Create(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_createCascadeClassifier(filename, out var smartPtr));

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_CascadeClassifier_get(smartPtr, out var rawPtr));

            return new CascadeClassifier(smartPtr, rawPtr);
        }

        /// <summary>
        /// Detects objects of different sizes in the input image. The detected objects are returned as a GpuMat buffer.
        /// </summary>
        public virtual void DetectMultiScale(OpenCvSharp.Cuda.CudaInputArray image, OpenCvSharp.Cuda.CudaOutputArray objects, OpenCvSharp.Cuda.Stream? stream = null)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (objects is null) throw new ArgumentNullException(nameof(objects));

            image.ThrowIfDisposed();
            objects.ThrowIfNotReady();
            ThrowIfDisposed();

            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_CascadeClassifier_detectMultiScale(RawPtr, image.CvPtr, objects.CvPtr, stream?.CvPtr??IntPtr.Zero));

            objects.Fix();
            GC.KeepAlive(this);
            GC.KeepAlive(stream);
            GC.KeepAlive(image);
        }

        /// <summary>
        /// Converts the GpuMat buffer returned by DetectMultiScale into a C# Rect array.
        /// </summary>
        public virtual Rect[] Convert(OpenCvSharp.Cuda.CudaOutputArray gpuObjects)
        {
            if (gpuObjects is null) throw new ArgumentNullException(nameof(gpuObjects));
            gpuObjects.ThrowIfNotReady();
            ThrowIfDisposed();
            using var rectVec = new VectorOfRect();
            NativeMethods.HandleException(
                NativeMethods_cuda.cuda_CascadeClassifier_convert(RawPtr, gpuObjects.CvPtr, rectVec.CvPtr));

            GC.KeepAlive(this);
            GC.KeepAlive(gpuObjects);

            return rectVec.ToArray();
        }

        /// <summary>
        /// Convenience method: Automatically detects and converts to Rect[] array.
        /// </summary>
        public Rect[] DetectMultiScale(GpuMat image)
        {
            using var gpuObjects = new GpuMat();
            DetectMultiScale(image, gpuObjects);
            return Convert(gpuObjects);
        }
        public Size ClassifierSize
        {
            get { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_getClassifierSize(RawPtr, out Size val)); 
                GC.KeepAlive(this); 
                return val; 
            }
        }

        public bool FindLargestObject
        {
            get { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_getFindLargestObject(RawPtr, out int val)); 
                GC.KeepAlive(this); 
                return val != 0; 
            }
            set { 
                ThrowIfDisposed();
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_setFindLargestObject(RawPtr, value ? 1 : 0));
                GC.KeepAlive(this); 
            }
        }

        public int MaxNumObjects
        {
            get { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_getMaxNumObjects(RawPtr, out int val)); 
                GC.KeepAlive(this);
                return val; 
            }
            set { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_setMaxNumObjects(RawPtr, value)); 
                GC.KeepAlive(this); 
            }
        }

        public Size MaxObjectSize
        {
            get { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_getMaxObjectSize(RawPtr, out Size val)); 
                GC.KeepAlive(this);
                return val; 
            }
            set { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_setMaxObjectSize(RawPtr, value)); 
                GC.KeepAlive(this); 
            }
        }

        public int MinNeighbors
        {
            get { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_getMinNeighbors(RawPtr, out int val)); 
                GC.KeepAlive(this);
                return val; 
            }
            set { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_setMinNeighbors(RawPtr, value)); 
                GC.KeepAlive(this);
            }
        }

        public Size MinObjectSize
        {
            get { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_getMinObjectSize(RawPtr, out Size val)); 
                GC.KeepAlive(this);
                return val;
            }
            set { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_setMinObjectSize(RawPtr, value)); 
                GC.KeepAlive(this);
            }
        }

        public double ScaleFactor
        {
            get { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_getScaleFactor(RawPtr, out double val)); 
                GC.KeepAlive(this);
                return val; 
            }
            set { 
                ThrowIfDisposed(); 
                NativeMethods.HandleException(NativeMethods_cuda.cuda_CascadeClassifier_setScaleFactor(RawPtr, value)); 
                GC.KeepAlive(this); 
            }
        }
    }
}