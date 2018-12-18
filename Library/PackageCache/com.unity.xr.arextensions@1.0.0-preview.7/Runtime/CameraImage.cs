using AOT;
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// Represents a single, raw image from a device camera. Provides access to the raw image plane data, as well
    /// as conversion methods to convert to color and grayscale formats. See <see cref="Convert(CameraImageConversionParams, IntPtr, int)"/>
    /// and <see cref="ConvertAsync(CameraImageConversionParams)"/>.
    /// Use <see cref="XRCameraExtensions.TryGetLatestImage(Experimental.XR.XRCameraSubsystem, out CameraImage)"/> to get a <c>CameraImage</c>. 
    /// 
    /// <c>CameraImage</c>s must be explicitly disposed. Failing to do so will leak resources and could prevent future camera image access.
    /// </summary>
    public struct CameraImage : IDisposable, IEquatable<CameraImage>
    {
        /// <summary>
        /// The dimensions (width and height) of the image.
        /// </summary>
        public Vector2Int dimensions { get; private set; }

        /// <summary>
        /// The image width.
        /// </summary>
        public int width { get { return dimensions.x; } }

        /// <summary>
        /// The image height.
        /// </summary>
        public int height { get { return dimensions.y; } }

        /// <summary>
        /// The number of image planes. A "plane" in this context refers to a channel in the raw video format,
        /// not a physical surface.
        /// </summary>
        public int planeCount { get; private set; }

        /// <summary>
        /// The format used by the image planes. You will only need this if you plan to interpret the raw plane data.
        /// </summary>
        public CameraImageFormat format { get; private set; }

        /// <summary>
        /// The timestamp, in seconds, associated with this camera image
        /// </summary>
        public double timestamp { get; private set; }

        /// <summary>
        /// Returns <c>true</c> if this <c>CameraImage</c> represents a valid image (i.e., not Disposed).
        /// </summary>
        public bool valid
        {
            get
            {
                if (m_CameraImageApi == null)
                    return false;

                return m_CameraImageApi.NativeHandleValid(m_NativeHandle);
            }
        }

        /// <summary>
        /// Determines whether the given <c>TextureFormat</c> is supported for conversion.
        /// </summary>
        /// <param name="format">A <c>TextureFormat</c> to test.
        /// These grayscale and RGB texture formats are supported:
        /// - <c>TextureFormat.R8</c>
        /// - <c>TextureFormat.Alpha8</c>
        /// - <c>TextureFormat.RGB24</c>
        /// - <c>TextureFormat.RGBA32</c>
        /// - <c>TextureFormat.ARGBA32</c>
        /// - <c>TextureFormat.BGRA32</c>
        /// </param>
        /// <returns><c>true</c> if the format is supported by the various conversion methods.</returns>
        public static bool FormatSupported(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Alpha8:
                case TextureFormat.R8:
                case TextureFormat.RGB24:
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                case TextureFormat.BGRA32:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get an image "plane". A "plane" in this context refers to a channel in the raw video format,
        /// not a physical surface. Throws if the plane is invalid.
        /// </summary>
        /// <param name="planeIndex">The index of the plane to get.</param>
        /// <returns>A <see cref="CameraImagePlane"/> describing the plane.</returns>
        public unsafe CameraImagePlane GetPlane(int planeIndex)
        {
            ValidateNativeHandleAndThrow();
            if (planeIndex < 0 || planeIndex >= planeCount)
                throw new ArgumentOutOfRangeException("planeIndex",
                    string.Format("planeIndex must be in the range 0 to {0}", planeCount - 1));

            int rowStride;
            int pixelStride;
            IntPtr ptr;
            int length;
            if (!m_CameraImageApi.TryGetPlane(m_NativeHandle, planeIndex, out rowStride, out pixelStride, out ptr, out length))
                throw new InvalidOperationException("The requested plane is not valid for this CameraImage.");

            var data = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(
                (void*)ptr, length, Allocator.None);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref data, m_SafetyHandle);
#endif

            return new CameraImagePlane
            {
                rowStride = rowStride,
                pixelStride = pixelStride,
                data = data
            };
        }

        /// <summary>
        /// Get the number of bytes required to store a converted image with the given parameters.
        /// Throws if the inputs are invalid.
        /// </summary>
        /// <param name="dimensions">The dimensions of the converted image.</param>
        /// <param name="format">The <c>TextureFormat</c> for the converted image. See <see cref="FormatSupported(TextureFormat)"/>.</param>
        /// <returns>The number of bytes required to store the converted image.</returns>
        public int GetConvertedDataSize(Vector2Int dimensions, TextureFormat format)
        {
            ValidateNativeHandleAndThrow();
            if (dimensions.x > this.dimensions.x)
                throw new ArgumentOutOfRangeException("width",
                    string.Format("Converted image width must be less than or equal to native image width. {0} > {1}", dimensions.x, this.dimensions.x));
            if (dimensions.y > this.dimensions.y)
                throw new ArgumentOutOfRangeException("height",
                    string.Format("Converted image height must be less than or equal to native image height. {0} > {1}", dimensions.y, this.dimensions.y));
            if (!FormatSupported(format))
                throw new ArgumentException("Invalid texture format.", "format");

            int size;
            if (!m_CameraImageApi.TryGetConvertedDataSize(m_NativeHandle, dimensions, format, out size))
                throw new InvalidOperationException("CameraImage is not valid.");

            return size;
        }

        /// <summary>
        /// Get the number of bytes required to store a converted image with the given parameters.
        /// </summary>
        /// <param name="conversionParams">The <see cref="CameraImageConversionParams"/> to use for the conversion.</param>
        /// <returns>The number of bytes required to store the converted image.</returns>
        public int GetConvertedDataSize(CameraImageConversionParams conversionParams)
        {
            return GetConvertedDataSize(
                conversionParams.outputDimensions,
                conversionParams.outputFormat);
        }

        /// <summary>
        /// Convert the <c>CameraImage</c> to one of the supported formats (see <see cref="FormatSupported(TextureFormat)"/>)
        /// using the specified <paramref name="conversionParams"/>.
        /// </summary>
        /// <param name="conversionParams">The <see cref="CameraImageConversionParams"/> to use for the conversion.</param>
        /// <param name="destinationBuffer">A pointer to memory to write the converted image to.</param>
        /// <param name="bufferLength">The number of bytes pointed to by <paramref name="destinationBuffer"/>. Must be greater than or equal to the
        /// value returned by <see cref="GetConvertedDataSize(CameraImageConversionParams)"/>.</param>
        public void Convert(CameraImageConversionParams conversionParams, IntPtr destinationBuffer, int bufferLength)
        {
            ValidateNativeHandleAndThrow();
            ValidateConversionParamsAndThrow(conversionParams);
            int requiredDataSize = GetConvertedDataSize(conversionParams);
            if (bufferLength < requiredDataSize)
                throw new InvalidOperationException(string.Format(
                    "Conversion requires {0} bytes but only provided {1} bytes.", requiredDataSize, bufferLength));

            if (!m_CameraImageApi.TryConvert(m_NativeHandle, conversionParams, destinationBuffer, bufferLength))
                throw new InvalidOperationException("Conversion failed.");
        }

        /// <summary>
        /// Convert the <c>CameraImage</c> to one of the supported formats (see <see cref="FormatSupported(TextureFormat)"/>)
        /// using the specified <paramref name="conversionParams"/>. The conversion is performed asynchronously. Use the returned
        /// <see cref="AsyncCameraImageConversion"/> to check for the status of the conversion, and retrieve the data when complete.
        /// It is safe to <c>Dispose</c> the <c>CameraImage</c> before the asynchronous operation has completed.
        /// </summary>
        /// <param name="conversionParams">The <see cref="CameraImageConversionParams"/> to use for the conversion.</param>
        /// <returns>A new <see cref="AsyncCameraImageConversion"/> which can be used to check the status of the conversion operation
        /// and get the resulting data.</returns>
        public AsyncCameraImageConversion ConvertAsync(CameraImageConversionParams conversionParams)
        {
            ValidateNativeHandleAndThrow();
            ValidateConversionParamsAndThrow(conversionParams);

            return new AsyncCameraImageConversion(m_CameraImageApi, m_NativeHandle, conversionParams);
        }

        /// <summary>
        /// Convert the <c>CameraImage</c> to one of the supported formats (see <see cref="FormatSupported(TextureFormat)"/>)
        /// using the specified <paramref name="conversionParams"/>. The conversion is performed asynchronously, and <paramref name="onComplete"/>
        /// is invoked when the conversion is complete, whether successful or not.
        /// 
        /// The <c>NativeArray</c> provided in the <paramref name="onComplete"/> delegate is only valid during the invocation and is disposed
        /// immediately upon return.
        /// </summary>
        /// <param name="conversionParams">The <see cref="CameraImageConversionParams"/> to use for the conversion.</param>
        /// <param name="onComplete">A delegate to invoke when the conversion operation completes. The delegate is always invoked.</param>
        public void ConvertAsync(
            CameraImageConversionParams conversionParams,
            Action<AsyncCameraImageConversionStatus, CameraImageConversionParams, NativeArray<byte>> onComplete)
        {
            ValidateNativeHandleAndThrow();
            ValidateConversionParamsAndThrow(conversionParams);

            var handle = GCHandle.Alloc(onComplete);
            var context = GCHandle.ToIntPtr(handle);
            m_CameraImageApi.ConvertAsync(m_NativeHandle, conversionParams, s_OnAsyncConversionComplete, context);
        }

        [MonoPInvokeCallback(typeof(XRCameraExtensions.OnImageRequestCompleteDelegate))]
        static unsafe void OnAsyncConversionComplete(
            AsyncCameraImageConversionStatus status, CameraImageConversionParams conversionParams, IntPtr dataPtr, int dataLength, IntPtr context)
        {
            var handle = GCHandle.FromIntPtr(context);
            var onComplete = (Action<AsyncCameraImageConversionStatus, CameraImageConversionParams, NativeArray<byte>>)handle.Target;

            if (onComplete != null)
            {
                var data = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(
                    (void*)dataPtr, dataLength, Allocator.None);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                var safetyHandle = AtomicSafetyHandle.Create();
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref data, safetyHandle);
#endif

                onComplete(status, conversionParams, data);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.Release(safetyHandle);
#endif
            }

            handle.Free();
        }

        void ValidateNativeHandleAndThrow()
        {
            if (!valid)
                throw new InvalidOperationException("CameraImage is not valid.");
        }

        void ValidateConversionParamsAndThrow(CameraImageConversionParams conversionParams)
        {
            if ((conversionParams.inputRect.x + conversionParams.inputRect.width > width) ||
                (conversionParams.inputRect.y + conversionParams.inputRect.height > height))
            {
                throw new ArgumentOutOfRangeException(
                    "conversionParams.inputRect",
                    "Input rect must be completely within the original image.");
            }

            if ((conversionParams.outputDimensions.x > conversionParams.inputRect.width) ||
                (conversionParams.outputDimensions.y > conversionParams.inputRect.height))
            {
                throw new InvalidOperationException(string.Format(
                    "Output dimensions must be less than or equal to the inputRect's dimensions: ({0}x{1} > {2}x{3}).",
                    conversionParams.outputDimensions.x, conversionParams.outputDimensions.y,
                    conversionParams.inputRect.width, conversionParams.inputRect.height));
            }

            if (!FormatSupported(conversionParams.outputFormat))
                throw new ArgumentException("TextureFormat not supported.", "conversionParams.format");
        }

        /// <summary>
        /// Dispose native resources associated with this request, including the raw image data.
        /// Any <see cref="CameraImagePlane"/>s returned by <see cref="GetPlane(int)"/> are invalidated immediately
        /// after calling <c>Dispose</c>.
        /// </summary>
        public void Dispose()
        {
            if (m_CameraImageApi == null || m_NativeHandle == 0)
                return;

            m_CameraImageApi.DisposeImage(m_NativeHandle);
            m_NativeHandle = 0;
            m_CameraImageApi = null;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_SafetyHandle);
#endif
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = width.GetHashCode();
                hash = hash * 486187739 + height.GetHashCode();
                hash = hash * 486187739 + planeCount.GetHashCode();
                hash = hash * 486187739 + m_NativeHandle.GetHashCode();
                hash = hash * 486187739 + format.GetHashCode();
                if (m_CameraImageApi != null)
                    hash = hash * 486187739 + m_CameraImageApi.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CameraImage))
                return false;

            return Equals((CameraImage)obj);
        }

        public override string ToString()
        {
            return string.Format(
                "(Width: {0}, Height: {1}, PlaneCount: {2}, Format: {3})",
                width, height, planeCount, format);
        }

        public bool Equals(CameraImage other)
        {
            return
                (width == other.width) &&
                (height == other.height) &&
                (planeCount == other.planeCount) &&
                (format == other.format) &&
                (m_NativeHandle == other.m_NativeHandle) &&
                (m_CameraImageApi == other.m_CameraImageApi);
        }

        public static bool operator ==(CameraImage lhs, CameraImage rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CameraImage lhs, CameraImage rhs)
        {
            return !lhs.Equals(rhs);
        }

        static CameraImage()
        {
            s_OnAsyncConversionComplete = new XRCameraExtensions.OnImageRequestCompleteDelegate(OnAsyncConversionComplete);
        }

        internal CameraImage(
            ICameraImageApi cameraImageApi,
            int nativeHandle,
            Vector2Int dimensions,
            int planeCount,
            double timestamp,
            CameraImageFormat format)
        {
            m_CameraImageApi = cameraImageApi;
            m_NativeHandle = nativeHandle;
            this.dimensions = dimensions;
            this.planeCount = planeCount;
            this.timestamp = timestamp;
            this.format = format;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_SafetyHandle = AtomicSafetyHandle.Create();
#endif
        }

        int m_NativeHandle;

        ICameraImageApi m_CameraImageApi;

        static XRCameraExtensions.OnImageRequestCompleteDelegate s_OnAsyncConversionComplete;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle m_SafetyHandle;
#endif
    }
}
