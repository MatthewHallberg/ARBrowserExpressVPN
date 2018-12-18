using System;
using System.Collections.Generic;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.ARExtensions
{
    class DefaultCameraImageApi : ICameraImageApi
    {
        public AsyncCameraImageConversionStatus GetAsyncRequestStatus(int requestId)
        {
            return AsyncCameraImageConversionStatus.Disposed;
        }

        public void DisposeImage(int nativeHandle)
        { }

        public void DisposeAsyncRequest(int requestId)
        { }

        public bool TryGetPlane(int nativeHandle, int planeIndex, out int rowStride, out int pixelStride, out IntPtr dataPtr, out int dataLength)
        {
            rowStride = default(int);
            pixelStride = default(int);
            dataPtr = default(IntPtr);
            dataLength = default(int);
            return false;
        }

        public bool TryAcquireLatestImage(out int nativeHandle, out Vector2Int dimensions, out int planeCount, out double timestamp, out CameraImageFormat format)
        {
            nativeHandle = default(int);
            dimensions = default(Vector2Int);
            planeCount = default(int);
            timestamp = default(double);
            format = default(CameraImageFormat);
            return false;
        }

        public bool NativeHandleValid(int nativeHandle)
        {
            return false;
        }

        public bool TryGetConvertedDataSize(int nativeHandle, Vector2Int dimensions, TextureFormat format, out int size)
        {
            size = default(int);
            return false;
        }

        public bool TryConvert(int nativeHandle, CameraImageConversionParams conversionParams, IntPtr destinationBuffer, int bufferLength)
        {
            return false;
        }

        public int ConvertAsync(int nativeHandle, CameraImageConversionParams conversionParams)
        {
            return 0;
        }

        public bool TryGetAsyncRequestData(int requestId, out IntPtr dataPtr, out int dataLength)
        {
            dataPtr = default(IntPtr);
            dataLength = default(int);
            return false;
        }

        public void ConvertAsync(
            int nativeHandle,
            CameraImageConversionParams conversionParams,
            XRCameraExtensions.OnImageRequestCompleteDelegate callback,
            IntPtr context)
        {
            callback(AsyncCameraImageConversionStatus.Disposed, conversionParams, IntPtr.Zero, 0, context);
        }
    }
}
