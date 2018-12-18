using System;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARExtensions;

namespace UnityEngine.XR.ARCore
{
    internal static class Api
    {
        public enum ArStatus
        {
            /// The operation was successful.
            AR_SUCCESS = 0,

            /// One of the arguments was invalid, either null or not appropriate for the
            /// operation requested.
            AR_ERROR_INVALID_ARGUMENT = -1,

            /// An internal error occurred that the application should not attempt to
            /// recover from.
            AR_ERROR_FATAL = -2,

            /// An operation was attempted that requires the session be running, but the
            /// session was paused.
            AR_ERROR_SESSION_PAUSED = -3,

            /// An operation was attempted that requires the session be paused, but the
            /// session was running.
            AR_ERROR_SESSION_NOT_PAUSED = -4,

            /// An operation was attempted that the session be in the TRACKING state,
            /// but the session was not.
            AR_ERROR_NOT_TRACKING = -5,

            /// A texture name was not set by calling ArSession_setCameraTextureName()
            /// before the first call to ArSession_update()
            AR_ERROR_TEXTURE_NOT_SET = -6,

            /// An operation required GL context but one was not available.
            AR_ERROR_MISSING_GL_CONTEXT = -7,

            /// The configuration supplied to ArSession_configure() was unsupported.
            /// To avoid this error, ensure that Session_checkSupported() returns true.
            AR_ERROR_UNSUPPORTED_CONFIGURATION = -8,

            /// The android camera permission has not been granted prior to calling
            /// ArSession_resume()
            AR_ERROR_CAMERA_PERMISSION_NOT_GRANTED = -9,

            /// Acquire failed because the object being acquired is already released.
            /// For example, this happens if the application holds an ::ArFrame beyond
            /// the next call to ArSession_update(), and then tries to acquire its point
            /// cloud.
            AR_ERROR_DEADLINE_EXCEEDED = -10,

            /// There are no available resources to complete the operation.  In cases of
            /// @c acquire methods returning this error, This can be avoided by
            /// releasing previously acquired objects before acquiring new ones.
            AR_ERROR_RESOURCE_EXHAUSTED = -11,

            /// Acquire failed because the data isn't available yet for the current
            /// frame. For example, acquire the image metadata may fail with this error
            /// because the camera hasn't fully started.
            AR_ERROR_NOT_YET_AVAILABLE = -12,

            /// The android camera has been reallocated to a higher priority app or is
            /// otherwise unavailable.
            AR_ERROR_CAMERA_NOT_AVAILABLE = -13,

            /// The ARCore APK is not installed on this device.
            AR_UNAVAILABLE_ARCORE_NOT_INSTALLED = -100,

            /// The device is not currently compatible with ARCore.
            AR_UNAVAILABLE_DEVICE_NOT_COMPATIBLE = -101,

            /// The ARCore APK currently installed on device is too old and needs to be
            /// updated.
            AR_UNAVAILABLE_APK_TOO_OLD = -103,

            /// The ARCore APK currently installed no longer supports the ARCore SDK
            /// that the application was built with.
            AR_UNAVAILABLE_SDK_TOO_OLD = -104,

            /// The user declined installation of the ARCore APK during this run of the
            /// application and the current request was not marked as user-initiated.
            AR_UNAVAILABLE_USER_DECLINED_INSTALLATION = -105
        }

        public enum ArInstallStatus
        {
            /// The requested resource is already installed.
            AR_INSTALL_STATUS_INSTALLED = 0,
            /// Installation of the resource was requested. The current activity will be
            /// paused.
            AR_INSTALL_STATUS_INSTALL_REQUESTED = 1
        }

        public enum ArAvailability
        {
            AR_AVAILABILITY_UNKNOWN_ERROR = 0,
            /// ARCore is not installed, and a query has been issued to check if ARCore
            /// is is supported.
            AR_AVAILABILITY_UNKNOWN_CHECKING = 1,
            /// ARCore is not installed, and the query to check if ARCore is supported
            /// timed out. This may be due to the device being offline.
            AR_AVAILABILITY_UNKNOWN_TIMED_OUT = 2,
            /// ARCore is not supported on this device.
            AR_AVAILABILITY_UNSUPPORTED_DEVICE_NOT_CAPABLE = 100,
            /// The device and Android version are supported, but the ARCore APK is not
            /// installed.
            AR_AVAILABILITY_SUPPORTED_NOT_INSTALLED = 201,
            /// The device and Android version are supported, and a version of the
            /// ARCore APK is installed, but that ARCore APK version is too old.
            AR_AVAILABILITY_SUPPORTED_APK_TOO_OLD = 202,
            /// ARCore is supported, installed, and available to use.
            AR_AVAILABILITY_SUPPORTED_INSTALLED = 203
        }

        public enum ArPrestoApkInstallStatus
        {
            ARPRESTO_APK_INSTALL_UNINITIALIZED = 0,
            ARPRESTO_APK_INSTALL_REQUESTED = 1,

            ARPRESTO_APK_INSTALL_SUCCESS = 100,

            ARPRESTO_APK_INSTALL_ERROR = 200,
            ARPRESTO_APK_INSTALL_ERROR_DEVICE_NOT_COMPATIBLE = 201,
            ARPRESTO_APK_INSTALL_ERROR_USER_DECLINED = 203,
        }

        public enum SetCameraConfigurationResult
        {
            /// <summary>
            /// The given <see cref="CameraConfiguration"/> is not supported.
            /// </summary>
            ErrorInvalidConfiguration,

            /// <summary>
            /// Not all <see cref="CameraImage"/>s have been disposed prior to calling <see cref="ICameraImageApi.TrySetConfiguration(CameraConfiguration)"/>.
            /// </summary>
            ErrorCameraImagesNotDisposed,

            /// <summary>
            /// The <see cref="CameraConfiguration"/> was set successfully.
            /// </summary>
            Success
        }

        internal delegate void CameraPermissionRequestProvider(CameraPermissionsResultCallback resultCallback, IntPtr context);

        internal delegate void CameraPermissionsResultCallback(bool granted, IntPtr context);

#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport("UnityARCore")]
        static internal extern void ArPresto_checkApkAvailability(
            Action<ArAvailability, IntPtr> on_result, IntPtr context);

        [DllImport("UnityARCore")]
        static internal extern void ArPresto_requestApkInstallation(
            bool userRequested, Action<ArPrestoApkInstallStatus, IntPtr> on_result, IntPtr context);

        [DllImport("UnityARCore")]
        static internal extern void ArPresto_update();

        [DllImport("UnityARCore")]
        static internal extern TrackingState UnityARCore_getAnchorTrackingState(TrackableId id);

        [DllImport("UnityARCore")]
        static internal extern void UnityARCore_setCameraPermissionProvider(CameraPermissionRequestProvider provider);

        [DllImport("UnityARCore")]
        static internal extern TrackableId UnityARCore_attachReferencePoint(TrackableId trackableId, Pose pose);

        [DllImport("UnityARCore")]
        static internal extern IntPtr UnityARCore_getNativePlanePtr(TrackableId planeId);

        [DllImport("UnityARCore")]
        static internal extern IntPtr UnityARCore_getNativeReferencePointPtr(TrackableId planeId);

        [DllImport("UnityARCore")]
        static internal extern IntPtr UnityARCore_getNativeSessionPtr();

        [DllImport("UnityARCore")]
        static internal extern IntPtr UnityARCore_getNativeFramePtr();

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_tryGetColorCorrection(out float red, out float green, out float blue, out float alpha);

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_cameraImage_tryAcquireLatestImage(
            out int nativeHandle, out Vector2Int dimensions, out int planeCount, out double timestamp, out CameraImageFormat format);

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_cameraImage_tryGetConvertedDataSize(
            int nativeHandle, Vector2Int dimensions, TextureFormat format, out int size);

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_cameraImage_tryConvert(
            int nativeHandle, CameraImageConversionParams conversionParams,
            IntPtr buffer, int bufferLength);

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_cameraImage_tryGetPlane(
            int nativeHandle, int planeIndex,
            out int rowStride, out int pixelStride, out IntPtr dataPtr, out int dataLength);

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_cameraImage_handleValid(
            int nativeHandle);

        [DllImport("UnityARCore")]
        static internal extern void UnityARCore_cameraImage_disposeImage(
            int nativeHandle);

        [DllImport("UnityARCore")]
        static internal extern int UnityARCore_cameraImage_createAsyncConversionRequest(
            int nativeHandle, CameraImageConversionParams conversionParams);

        [DllImport("UnityARCore")]
        static internal extern void UnityARCore_cameraImage_createAsyncConversionRequestWithCallback(
            int nativeHandle, CameraImageConversionParams conversionParams,
            XRCameraExtensions.OnImageRequestCompleteDelegate callback, IntPtr context);

        [DllImport("UnityARCore")]
        static internal extern AsyncCameraImageConversionStatus
            UnityARCore_cameraImage_getAsyncRequestStatus(int requestId);

        [DllImport("UnityARCore")]
        static internal extern void UnityARCore_cameraImage_disposeAsyncRequest(
            int requestHandle);

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_cameraImage_tryGetAsyncRequestData(
            int requestHandle, out IntPtr dataPtr, out int dataLength);

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_trySetFocusMode(
            CameraFocusMode mode);

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_trySetPlaneDetectionFlags(PlaneDetectionFlags flags);

        [DllImport("UnityARCore")]
        static internal extern int UnityARCore_cameraImage_getConfigurationCount();

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_cameraImage_tryGetConfiguration(
            int index,
            out CameraConfiguration configuration);

        [DllImport("UnityARCore")]
        static internal extern bool UnityARCore_cameraImage_tryGetCurrentConfiguration(
            out CameraConfiguration configuration);

        [DllImport("UnityARCore")]
        static internal extern SetCameraConfigurationResult UnityARCore_cameraImage_trySetConfiguration(
            CameraConfiguration configuration);
#else
        static internal bool UnityARCore_cameraImage_tryGetCurrentConfiguration(
            out CameraConfiguration configuration)
        {
            configuration = default(CameraConfiguration);
            return false;
        }

        static internal int UnityARCore_cameraImage_getConfigurationCount()
        {
            return 0;
        }

        static internal bool UnityARCore_cameraImage_tryGetConfiguration(
            int index,
            out CameraConfiguration configuration)
        {
            configuration = default(CameraConfiguration);
            return false;
        }

        static internal SetCameraConfigurationResult UnityARCore_cameraImage_trySetConfiguration(
            CameraConfiguration configuration)
        {
            return SetCameraConfigurationResult.ErrorInvalidConfiguration;
        }

        static internal bool UnityARCore_trySetFocusMode(
            CameraFocusMode mode)
        {
            return false;
        }

        static internal bool UnityARCore_trySetPlaneDetectionFlags(PlaneDetectionFlags flags)
        {
            return false;
        }

        static internal bool UnityARCore_cameraImage_tryAcquireLatestImage(
            out int nativeHandle, out Vector2Int dimensions, out int planeCount, out double timestamp, out CameraImageFormat format)
        {
            nativeHandle = 0;
            dimensions = default(Vector2Int);
            planeCount = default(int);
            timestamp = default(double);
            format = default(CameraImageFormat);
            return false;
        }

        static internal bool UnityARCore_cameraImage_tryGetConvertedDataSize(
            int nativeHandle, Vector2Int dimensions, TextureFormat format, out int size)
        {
            size = default(int);
            return false;
        }

        static internal bool UnityARCore_cameraImage_tryConvert(
            int nativeHandle, CameraImageConversionParams conversionParams,
            IntPtr buffer, int bufferLength)
        {
            return false;
        }

        static internal bool UnityARCore_cameraImage_tryGetPlane(
            int nativeHandle, int planeIndex,
            out int rowStride, out int pixelStride, out IntPtr dataPtr, out int dataLength)
        {
            rowStride = default(int);
            pixelStride = default(int);
            dataPtr = default(IntPtr);
            dataLength = default(int);
            return false;
        }

        static internal bool UnityARCore_cameraImage_handleValid(
            int nativeHandle)
        {
            return false;
        }

        static internal void UnityARCore_cameraImage_disposeImage(
            int nativeHandle)
        { }

        static internal int UnityARCore_cameraImage_createAsyncConversionRequest(
            int nativeHandle, CameraImageConversionParams conversionParams)
        {
            return 0;
        }

        static internal void UnityARCore_cameraImage_createAsyncConversionRequestWithCallback(
            int nativeHandle, CameraImageConversionParams conversionParams,
            XRCameraExtensions.OnImageRequestCompleteDelegate callback, IntPtr context)
        {
            callback(AsyncCameraImageConversionStatus.Disposed, conversionParams, IntPtr.Zero, 0, context);
        }

        static internal AsyncCameraImageConversionStatus
            UnityARCore_cameraImage_getAsyncRequestStatus(int requestId)
        {
            return AsyncCameraImageConversionStatus.Disposed;
        }

        static internal void UnityARCore_cameraImage_disposeAsyncRequest(
            int requestHandle)
        { }

        static internal bool UnityARCore_cameraImage_tryGetAsyncRequestData(
            int requestHandle, out IntPtr dataPtr, out int dataLength)
        {
            dataPtr = default(IntPtr);
            dataLength = default(int);
            return false;
        }

        static internal void ArPresto_checkApkAvailability(
            Action<ArAvailability, IntPtr> on_result, IntPtr context)
        {
            on_result(ArAvailability.AR_AVAILABILITY_UNKNOWN_ERROR, context);
        }

        static internal void ArPresto_requestApkInstallation(
            bool userRequested, Action<ArPrestoApkInstallStatus, IntPtr> on_result, IntPtr context)
        {
            on_result(ArPrestoApkInstallStatus.ARPRESTO_APK_INSTALL_ERROR, context);
        }

        static internal void ArPresto_update() { }

        static internal TrackingState UnityARCore_getAnchorTrackingState(TrackableId id)
        {
            return TrackingState.Unknown;
        }

        static internal void UnityARCore_setCameraPermissionProvider(CameraPermissionRequestProvider provider) { }

        static internal TrackableId UnityARCore_attachReferencePoint(TrackableId trackableId, Pose pose)
        {
            return TrackableId.InvalidId;
        }

        static internal IntPtr UnityARCore_getNativePlanePtr(TrackableId trackableId)
        {
            return IntPtr.Zero;
        }

        static internal IntPtr UnityARCore_getNativeReferencePointPtr(TrackableId planeId)
        {
            return IntPtr.Zero;
        }

        static internal IntPtr UnityARCore_getNativeSessionPtr()
        {
            return IntPtr.Zero;
        }

        static internal IntPtr UnityARCore_getNativeFramePtr()
        {
            return IntPtr.Zero;
        }

        static internal bool UnityARCore_tryGetColorCorrection(out float red, out float green, out float blue, out float alpha)
        {
            red = 0f;
            green = 0f;
            blue = 0f;
            alpha = 0f;
            return false;
        }
#endif
    }

    internal static class ARCoreEnumExtensions
    {
        public static SessionAvailability AsSessionAvailability(this Api.ArAvailability arCoreAvailability)
        {
            switch (arCoreAvailability)
            {
                case Api.ArAvailability.AR_AVAILABILITY_SUPPORTED_NOT_INSTALLED:
                case Api.ArAvailability.AR_AVAILABILITY_SUPPORTED_APK_TOO_OLD:
                    return SessionAvailability.Supported;

                case Api.ArAvailability.AR_AVAILABILITY_SUPPORTED_INSTALLED:
                    return SessionAvailability.Supported | SessionAvailability.Installed;

                default:
                    return SessionAvailability.None;
            }
        }

        public static SessionInstallationStatus AsSessionInstallationStatus(this Api.ArPrestoApkInstallStatus arCoreStatus)
        {
            switch (arCoreStatus)
            {
                case Api.ArPrestoApkInstallStatus.ARPRESTO_APK_INSTALL_ERROR_DEVICE_NOT_COMPATIBLE:
                    return SessionInstallationStatus.ErrorDeviceNotCompatible;

                case Api.ArPrestoApkInstallStatus.ARPRESTO_APK_INSTALL_ERROR_USER_DECLINED:
                    return SessionInstallationStatus.ErrorUserDeclined;

                case Api.ArPrestoApkInstallStatus.ARPRESTO_APK_INSTALL_REQUESTED:
                    // This shouldn't happen
                    return SessionInstallationStatus.Error;

                case Api.ArPrestoApkInstallStatus.ARPRESTO_APK_INSTALL_SUCCESS:
                    return SessionInstallationStatus.Success;

                case Api.ArPrestoApkInstallStatus.ARPRESTO_APK_INSTALL_ERROR:
                default:
                    return SessionInstallationStatus.Error;
            }
        }
    }
}
