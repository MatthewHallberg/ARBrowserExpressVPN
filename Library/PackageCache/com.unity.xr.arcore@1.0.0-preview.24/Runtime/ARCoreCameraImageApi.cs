using System;
using UnityEngine.XR.ARExtensions;

namespace UnityEngine.XR.ARCore
{
    internal class ARCoreCameraImageApi : ICameraImageApi
    {
        public bool TryAcquireLatestImage(out int nativeHandle, out Vector2Int dimensions, out int planeCount, out double timestamp, out CameraImageFormat format)
        {
            return Api.UnityARCore_cameraImage_tryAcquireLatestImage(out nativeHandle, out dimensions, out planeCount, out timestamp, out format);
        }

        public bool TryConvert(int nativeHandle, CameraImageConversionParams conversionParams, IntPtr destinationBuffer, int bufferLength)
        {
            return Api.UnityARCore_cameraImage_tryConvert(
                nativeHandle, conversionParams, destinationBuffer, bufferLength);
        }

        public int ConvertAsync(int nativeHandle, CameraImageConversionParams conversionParams)
        {
            return Api.UnityARCore_cameraImage_createAsyncConversionRequest(nativeHandle, conversionParams);
        }

        public void ConvertAsync(int nativeHandle, CameraImageConversionParams conversionParams, XRCameraExtensions.OnImageRequestCompleteDelegate callback, IntPtr context)
        {
            Api.UnityARCore_cameraImage_createAsyncConversionRequestWithCallback(
                nativeHandle, conversionParams, callback, context);
        }

        public void DisposeImage(int nativeHandle)
        {
            Api.UnityARCore_cameraImage_disposeImage(nativeHandle);
        }

        public void DisposeAsyncRequest(int requestId)
        {
            Api.UnityARCore_cameraImage_disposeAsyncRequest(requestId);
        }

        public AsyncCameraImageConversionStatus GetAsyncRequestStatus(int requestId)
        {
            return Api.UnityARCore_cameraImage_getAsyncRequestStatus(requestId);
        }

        public bool TryGetConvertedDataSize(int nativeHandle, Vector2Int dimensions, TextureFormat format, out int size)
        {
            return Api.UnityARCore_cameraImage_tryGetConvertedDataSize(nativeHandle, dimensions, format, out size);
        }

        public bool NativeHandleValid(int nativeHandle)
        {
            return Api.UnityARCore_cameraImage_handleValid(nativeHandle);
        }

        public bool TryGetAsyncRequestData(int requestId, out IntPtr dataPtr, out int dataLength)
        {
            return Api.UnityARCore_cameraImage_tryGetAsyncRequestData(requestId, out dataPtr, out dataLength);
        }

        public bool TryGetPlane(int nativeHandle, int planeIndex, out int rowStride, out int pixelStride, out IntPtr dataPtr, out int dataLength)
        {
            return Api.UnityARCore_cameraImage_tryGetPlane(nativeHandle, planeIndex, out rowStride, out pixelStride, out dataPtr, out dataLength);
        }
    }
}
