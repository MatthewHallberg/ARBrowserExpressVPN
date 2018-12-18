using System;
using Unity.Collections;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// This interface is typically implemented by platform-specific implementations of the <c>XRCameraSubsystem</c> to support <see cref="CameraImage"/>.
    /// End users do not need to implement this. Use <see cref="XRCameraExtensions.TryGetLatestImage(Experimental.XR.XRCameraSubsystem, out CameraImage)"/>
    /// to get a <see cref="CameraImage"/>.
    /// </summary>
    public interface ICameraImageApi
    {
        /// <summary>
        /// Get the status of an existing asynchronous conversion request. See <see cref="ConvertAsync(int, CameraImageConversionParams)"/>.
        /// </summary>
        /// <param name="requestId">The unique identifier associated with a request.</param>
        /// <returns>The state of the request.</returns>
        AsyncCameraImageConversionStatus GetAsyncRequestStatus(int requestId);

        /// <summary>
        /// Dispose an existing <see cref="CameraImage"/> identified by <paramref name="nativeHandle"/>.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for this camera image (see <see cref="TryAcquireLatestImage(out int, out int, out int, out int)"/>).</param>
        void DisposeImage(int nativeHandle);

        /// <summary>
        /// Dispose an existing async conversion request. See <see cref="ConvertAsync(int, CameraImageConversionParams)"/>.
        /// </summary>
        /// <param name="requestId">A unique identifier for the request.</param>
        void DisposeAsyncRequest(int requestId);

        /// <summary>
        /// Attempt to get information about an image plane from a <see cref="CameraImage"/> by index.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for this camera image (see <see cref="TryAcquireLatestImage(out int, out int, out int, out int)"/>).</param>
        /// <param name="planeIndex">The index of the plane to get.</param>
        /// <param name="rowStride">The number of bytes per row.</param>
        /// <param name="pixelStride">The number of bytes between pixels.</param>
        /// <param name="dataPtr">A pointer to the native memory.</param>
        /// <param name="dataLength">The number of bytes pointed to by <paramref name="dataPtr"/>.</param>
        /// <returns></returns>
        bool TryGetPlane(
            int nativeHandle,
            int planeIndex,
            out int rowStride,
            out int pixelStride,
            out IntPtr dataPtr,
            out int dataLength);

        /// <summary>
        /// Attempt to acquire the latest camera image. The camera image should be retained until
        /// <see cref="DisposeImage(int)"/> is called.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for the latest camera image.</param>
        /// <param name="dimensions">The dimensions of the image.</param>
        /// <param name="planeCount">The number of planes in the image.</param>
        /// <param name="timestamp">The timestamp associated with this camera image.</param>
        /// <returns><c>true</c> if the image was successfully acquired and all out parameters filled out.</returns>
        bool TryAcquireLatestImage(
            out int nativeHandle,
            out Vector2Int dimensions,
            out int planeCount,
            out double timestamp,
            out CameraImageFormat format);

        /// <summary>
        /// Determine whether a native image handle returned by <see cref="TryAcquireLatestImage(out int, out int, out int, out int)"/> is currently valid.
        /// An image may become invalid if it has been disposed (<see cref="DisposeImage(int)"/>).
        /// 
        /// If a handle is valid, <see cref="TryConvert(int, CameraImageConversionParams, IntPtr, int)"/> and <see cref="TryGetConvertedDataSize(int, int, int, TextureFormat, out int)"/>
        /// should not fail.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for the camera image in question.</param>
        /// <returns><c>true</c> if it is a valid handle, <c>false</c> otherwise.</returns>
        bool NativeHandleValid(
            int nativeHandle);

        /// <summary>
        /// Get the number of bytes required to store an image with the given dimensions and <c>TextureFormat</c>.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for the camera image to convert.</param>
        /// <param name="dimensions">The dimensions of the output image.</param>
        /// <param name="format">The <c>TextureFormat</c> for the image.</param>
        /// <param name="size">The number of bytes required to store the converted image.</param>
        /// <returns><c>true</c> if the output <paramref name="size"/> was set.</returns>
        bool TryGetConvertedDataSize(
            int nativeHandle,
            Vector2Int dimensions,
            TextureFormat format,
            out int size);

        /// <summary>
        /// Convert the image with handle <paramref name="nativeHandle"/> using the provided <see cref="CameraImageConversionParams"/>.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for the camera image to convert.</param>
        /// <param name="conversionParams">The <see cref="CameraImageConversionParams"/> to use during the conversion.</param>
        /// <param name="destinationBuffer">A buffer to write the converted image to.</param>
        /// <param name="bufferLength">The number of bytes available in the buffer.</param>
        /// <returns><c>true</c> if the image was converted and stored in <paramref name="destinationBuffer"/>.</returns>
        bool TryConvert(
            int nativeHandle,
            CameraImageConversionParams conversionParams,
            IntPtr destinationBuffer,
            int bufferLength);

        /// <summary>
        /// Create an asynchronous request to convert a camera image, similar to <see cref="TryConvert(int, CameraImageConversionParams, IntPtr, int)"/> except
        /// the conversion should happen on a thread other than the calling (main) thread.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for the camera image to convert.</param>
        /// <param name="conversionParams">The <see cref="CameraImageConversionParams"/> to use during the conversion.</param>
        /// <returns>A unique identifier for this request.</returns>
        int ConvertAsync(
            int nativeHandle,
            CameraImageConversionParams conversionParams);

        /// <summary>
        /// Get a pointer to the image data from a completed asynchronous request.
        /// This method should only succeed if <see cref="GetAsyncRequestStatus(int)"/>
        /// returns <see cref="AsyncCameraImageConversionStatus.Ready"/>.
        /// </summary>
        /// <param name="requestId">The unique identifier associated with a request.</param>
        /// <param name="dataPtr">A pointer to the native buffer containing the data.</param>
        /// <param name="dataLength">The number of bytes in <paramref name="dataPtr"/>.</param>
        /// <returns><c>true</c> if <paramref name="dataPtr"/> and <paramref name="dataLength"/> were set and point to the image data.</returns>
        bool TryGetAsyncRequestData(int requestId, out IntPtr dataPtr, out int dataLength);

        /// <summary>
        /// Similar to <see cref="ConvertAsync(int, CameraImageConversionParams)"/> but takes
        /// a delegate to invoke when the request is complete, rather than returning a request id.
        /// 
        /// If the first parameter to <paramref name="callback"/> is <see cref="AsyncCameraImageConversionStatus.Ready"/>
        /// then the <c>dataPtr</c> parameter must be valid for the duration of the invocation. The data may be destroyed
        /// immediately upon return. The <paramref name="context"/> parameter must be passed back to the <paramref name="callback"/>.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for the camera image to convert.</param>
        /// <param name="conversionParams">The <see cref="CameraImageConversionParams"/> to use during the conversion.</param>
        /// <param name="callback">A delegate which must be invoked when the request is complete, whether successfully or not.</param>
        /// <param name="context">An <c>IntPtr</c> which must be passed back unaltered to <paramref name="callback"/>.</param>
        void ConvertAsync(
            int nativeHandle, 
            CameraImageConversionParams conversionParams,
            XRCameraExtensions.OnImageRequestCompleteDelegate callback,
            IntPtr context);
    }
}
