using System;
using System.Collections.Generic;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// Provides extensions to the <c>XRCameraSubsystem</c>.
    /// </summary>
    public static class XRCameraExtensions
    {
        /// <summary>
        /// For internal use. Defines a delegate that a platform-specific camera provider can implement
        /// to provide color correction data.
        /// </summary>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        /// <param name="color">A <c>Color</c> representing the color correction data.</param>
        /// <returns><c>True</c> if the color correction was retrieved, otherwise, <c>False</c>.</returns>
        public delegate bool TryGetColorCorrectionDelegate(
            XRCameraSubsystem cameraSubsystems,
            out Color color);

        /// <summary>
        /// A delegate which implementors of <see cref="AsyncCameraImageConversion"/> must invoke when
        /// asynchronous camera image requests are made using the callback variant. See <see cref="CameraImage.ConvertAsync(CameraImageConversionParams, Action{AsyncCameraImageConversionStatus, CameraImageConversionParams, Unity.Collections.NativeArray{byte}})"/>.
        /// Consumers of the <c>XRCameraSubsystem</c> API should use <see cref="CameraImage.ConvertAsync(CameraImageConversionParams, Action{AsyncCameraImageConversionStatus, CameraImageConversionParams, Unity.Collections.NativeArray{byte}})"/>.
        /// </summary>
        /// <param name="status">The status of the request.</param>
        /// <param name="dataPtr">A pointer to the image data. Must be valid for the duration of the invocation, but may be destroyed immediately afterwards.</param>
        /// <param name="dataLength">The number of bytes pointed to by <paramref name="dataPtr"/>.</param>
        /// <param name="context">An <c>IntPtr</c> which is supplied to the API and must be passed back unaltered to this delegate.</param>
        public delegate void OnImageRequestCompleteDelegate(
            AsyncCameraImageConversionStatus status,
            CameraImageConversionParams conversionParams,
            IntPtr dataPtr,
            int dataLength,
            IntPtr context);

        /// <summary>
        /// For internal use. Allows a camera provider to register for the IsPermissionGranted extension.
        /// </summary>
        /// <param name="subsystemId">The string name associated with the camera provider to extend.</param>
        /// <param name="handler">A method that returns true if permission is granted.</param>
        public static void RegisterIsPermissionGrantedHandler(string subsystemId, Func<XRCameraSubsystem, bool> handler)
        {
            s_IsPermissionGrantedDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// For internal use. Allows a camera provider to register for the TryGetColorCorrection extension.
        /// </summary>
        /// <param name="subsystemId">The string name associated with the camera provider to extend.</param>
        /// <param name="handler">A method that returns true if color correction is available.</param>
        public static void RegisterTryGetColorCorrectionHandler(string subsystemId, TryGetColorCorrectionDelegate handler)
        {
            s_TryGetColorCorrectionDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// For internal use. Allows a camera provider to register for the GetNativePtr extension.
        /// </summary>
        /// <param name="subsystemId">The string name associated with the camera provider to extend.</param>
        /// <param name="handler">A method that returns an <c>IntPtr</c> associated with a <c>XRCameraSubsystem</c>.</param>
        public static void RegisterGetNativePtrHandler(string subsystemId, Func<XRCameraSubsystem, IntPtr> handler)
        {
            s_GetNativePtrDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// Allows a camera provider to register support for <see cref="ICameraImageApi"/>.
        /// This is typically only used by platform-specific packages.
        /// </summary>
        /// <param name="subsystemId">The string name associated with the camera provider to extend.</param>
        /// <param name="api">The API to use to perform provider-specific calls.</param>
        public static void RegisterCameraImageApi(string subsystemId, ICameraImageApi api)
        {
            s_CameraImageApis[subsystemId] = api;
        }

        /// <summary>
        /// Allows a camera provider to register support for <see cref="ICameraConfigApi"/>.
        /// This is typically only used by platform-specific packages.
        /// </summary>
        /// <param name="subsystemId">The string name associated with the camera provider to extend.</param>
        /// <param name="api">The API to use to perform provider-specific calls.</param>
        public static void RegisterCameraConfigApi(string subsystemId, ICameraConfigApi api)
        {
            s_CameraConfigApis[subsystemId] = api;
        }

        /// <summary>
        /// Allows a camera provider to register support for <see cref="TrySetFocusMode"/>.
        /// This is typically only used by platform-specific packages.
        /// </summary>
        /// <param name="subsystemId">The string name associated with the camera provider to extend.</param>
        /// <param name="handler">A method to set whether auto focus is enabled and returns <c>true</c> is successful,
        /// or <c>false</c> otherwise.</param>
        public static void RegisterTrySetFocusModeHandler(
            string subsystemId,
            Func<XRCameraSubsystem, CameraFocusMode, bool> handler)
        {
            s_TrySetFocusModeDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// Attempts to retrieve color correction data for the extended <c>XRCameraSubsystem</c>.
        /// The color correction data represents the scaling factors used for color correction.
        /// The RGB scale factors are used to match the color of the light
        /// in the scene. The alpha channel value is platform-specific.
        /// </summary>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        /// <param name="color">The <c>Color</c> representing the color correction value.</param>
        /// <returns><c>True</c> if the data is available, otherwise <c>False</c>.</returns>
        public static bool TryGetColorCorrection(this XRCameraSubsystem cameraSubsystem, out Color color)
        {
            if (cameraSubsystem == null)
                throw new ArgumentNullException("cameraSubsystem");

            return s_TryGetColorCorrectionDelegate(cameraSubsystem, out color);
        }

        /// <summary>
        /// Allows you to determine whether camera permission has been granted.
        /// </summary>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        /// <returns>True if camera permission has been granted for this app, false otherwise.</returns>
        public static bool IsPermissionGranted(this XRCameraSubsystem cameraSubsystem)
        {
            if (cameraSubsystem == null)
                throw new ArgumentNullException("cameraSubsystem");

            return s_IsPermissionGrantedDelegate(cameraSubsystem);
        }

        /// <summary>
        /// Retrieve a native <c>IntPtr</c> associated with the <c>XRCameraSubsystem</c>.
        /// </summary>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        /// <returns>An <c>IntPtr</c> associated with <paramref name="cameraSubsystem"/>.</returns>
        public static IntPtr GetNativePtr(this XRCameraSubsystem cameraSubsystem)
        {
            if (cameraSubsystem == null)
                throw new ArgumentNullException("cameraSubsystem");

            return s_GetNativePtrDelegate(cameraSubsystem);
        }

        /// <summary>
        /// Attempt to get the latest camera image. This provides directly access to the raw
        /// pixel data, as well as utilities to convert to RGB and Grayscale formats.
        /// The <see cref="CameraImage"/> must be disposed to avoid resource leaks.
        /// </summary>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        /// <param name="cameraImage">A valid <see cref="CameraImage"/> if this method returns <c>true</c>.</param>
        /// <returns><c>true</c> if the image was acquired; <c>false</c> otherwise.</returns>
        public static bool TryGetLatestImage(
            this XRCameraSubsystem cameraSubsystem,
            out CameraImage cameraImage)
        {
            if (cameraSubsystem == null)
                throw new ArgumentNullException("cameraSubsystem");

            int nativeHandle;
            Vector2Int dimensions;
            int planeCount;
            double timestamp;
            CameraImageFormat format;
            if (s_CameraImageApi.TryAcquireLatestImage(out nativeHandle, out dimensions, out planeCount, out timestamp, out format))
            {
                cameraImage = new CameraImage(s_CameraImageApi, nativeHandle, dimensions, planeCount, timestamp, format);
                return true;
            }
            else
            {
                cameraImage = default(CameraImage);
                return false;
            }
        }

        /// <summary>
        /// Get the current <see cref="CameraConfiguration"/>. If camera configurations
        /// are not supported, or if the session is not active, this method may throw
        /// <c>InvalidOperationException</c>.
        /// 
        /// Camera configurations are unsupported if
        /// <see cref="Configurations()"/> returns a collection with <see cref="CameraConfigurationCollection.count"/> of zero.
        /// </summary>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        /// <returns>The currently active <see cref="CameraConfiguration"/>.</returns>
        public static CameraConfiguration GetCurrentConfiguration(
            this XRCameraSubsystem cameraSubsystem)
        {
            if (cameraSubsystem == null)
                throw new ArgumentNullException("cameraSubsystem");

            return s_CameraConfigApi.currentConfiguration;
        }

        /// <summary>
        /// Set a <see cref="CameraConfiguration"/> to be the active one. Throws if the
        /// camera configuration is not supported by the device.
        /// You can enumerate the available configurations with
        /// <seealso cref="Configurations(XRCameraSubsystem)"/>.
        /// 
        /// The camera image configuration affects the resolution of the <see cref="CameraImage"/>
        /// provided by <see cref="TryGetLatestImage(XRCameraSubsystem, out CameraImage)"/>.
        /// It may also affect the camera framerate. See <see cref="CameraConfiguration.framerate"/>.
        /// 
        /// Setting this value may cause the device to relocalize, similar to stopping and restarting
        /// the session.
        /// 
        /// All <see cref="CameraImage"/>s should be disposed and all asynchronous conversion operations
        /// completed prior to setting the configuration or the this method may throw
        /// <c>InvalidOperationException</c>, depending on the platform.
        /// </summary>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        /// <param name="configuration">A configuration to use. This is typically one of the
        /// configurations returned by <see cref="GetConfiguration(XRCameraSubsystem, int)"/>.</param>
        public static void SetCurrentConfiguration(
            this XRCameraSubsystem cameraSubsystem,
            CameraConfiguration configuration)
        {
            if (cameraSubsystem == null)
                throw new ArgumentNullException("cameraSubsystem");

            s_CameraConfigApi.currentConfiguration = configuration;
        }

        /// <summary>
        /// Enumerate the <see cref="CameraConfiguration"/>s supported by the device.
        /// </summary>
        /// <example>
        /// You can use <see cref="Configurations(XRCameraSubsystem)"/> in a <c>foreach</c> statement:
        /// <code>
        /// foreach (var config in cameraSubsystem.Configurations())
        ///     Debug.Log(config);
        /// </code>
        /// </example>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        /// <returns>A <see cref="CameraConfigurationCollection"/>, which allows you to
        /// enumerate the supported configurations.</returns>
        public static CameraConfigurationCollection Configurations(
            this XRCameraSubsystem cameraSubsystem)
        {
            if (cameraSubsystem == null)
                throw new ArgumentNullException("cameraSubsystem");

            return new CameraConfigurationCollection(s_CameraConfigApi);
        }

        /// <summary>
        /// Attempt to set the <see cref="CameraFocusMode"/>.
        /// </summary>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        /// <param name="mode">The <see cref="CameraFocusMode"/> to use.</param>
        /// <returns><c>true</c> if the focus mode was successfully set; <c>false</c> otherwise.</returns>
        public static bool TrySetFocusMode(this XRCameraSubsystem cameraSubsystem, CameraFocusMode mode)
        {
            if (cameraSubsystem == null)
                throw new ArgumentNullException("cameraSubsystem");

            return s_TrySetFocusModeDelegate(cameraSubsystem, mode);
        }

        /// <summary>
        /// For internal use. Sets the active subsystem whose extension methods should be used.
        /// </summary>
        /// <param name="cameraSubsystem">The <c>XRCameraSubsystem</c> being extended.</param>
        public static void ActivateExtensions(this XRCameraSubsystem cameraSubsystem)
        {
            if (cameraSubsystem == null)

            {
                SetDefaultDelegates();
            }
            else
            {
                var id = cameraSubsystem.SubsystemDescriptor.id;
                s_IsPermissionGrantedDelegate = RegistrationHelper.GetValueOrDefault(s_IsPermissionGrantedDelegates, id, DefaultIsPermissionGranted);
                s_TryGetColorCorrectionDelegate = RegistrationHelper.GetValueOrDefault(s_TryGetColorCorrectionDelegates, id, DefaultTryGetColorCorrection);
                s_GetNativePtrDelegate = RegistrationHelper.GetValueOrDefault(s_GetNativePtrDelegates, id, DefaultGetNativePtr);
                s_CameraImageApi = RegistrationHelper.GetValueOrDefault(s_CameraImageApis, id, s_DefaultCameraImageApi);
                s_CameraConfigApi = RegistrationHelper.GetValueOrDefault(s_CameraConfigApis, id, s_DefaultCameraConfigApi);
                s_TrySetFocusModeDelegate = RegistrationHelper.GetValueOrDefault(s_TrySetFocusModeDelegates, id, s_DefaultTrySetFocusMode);
            }
        }

        static bool DefaultIsPermissionGranted(XRCameraSubsystem cameraSubsystem)
        {
            return true;
        }

        static bool DefaultTryGetColorCorrection(XRCameraSubsystem cameraSubsystem, out Color color)
        {
            color = default(Color);
            return false;
        }

        static IntPtr DefaultGetNativePtr(XRCameraSubsystem cameraSubsystem)
        {
            return IntPtr.Zero;
        }

        static bool s_DefaultTrySetFocusMode(XRCameraSubsystem cameraSubsystem, CameraFocusMode mode)
        {
            return false;
        }

        static void SetDefaultDelegates()
        {
            s_IsPermissionGrantedDelegate = DefaultIsPermissionGranted;
            s_TryGetColorCorrectionDelegate = DefaultTryGetColorCorrection;
            s_GetNativePtrDelegate = DefaultGetNativePtr;
            s_CameraImageApi = s_DefaultCameraImageApi;
            s_CameraConfigApi = s_DefaultCameraConfigApi;
            s_TrySetFocusModeDelegate = s_DefaultTrySetFocusMode;
        }

        static XRCameraExtensions()
        {
            s_DefaultCameraImageApi = new DefaultCameraImageApi();
            s_DefaultCameraConfigApi = new DefaultCameraConfigApi();
            SetDefaultDelegates();
        }

        static Func<XRCameraSubsystem, bool> s_IsPermissionGrantedDelegate;
        static TryGetColorCorrectionDelegate s_TryGetColorCorrectionDelegate;
        static Func<XRCameraSubsystem, IntPtr> s_GetNativePtrDelegate;
        static ICameraImageApi s_CameraImageApi;
        static DefaultCameraImageApi s_DefaultCameraImageApi;
        static ICameraConfigApi s_CameraConfigApi;
        static DefaultCameraConfigApi s_DefaultCameraConfigApi;
        static Func<XRCameraSubsystem, CameraFocusMode, bool> s_TrySetFocusModeDelegate;

        static Dictionary<string, Func<XRCameraSubsystem, bool>> s_IsPermissionGrantedDelegates =
            new Dictionary<string, Func<XRCameraSubsystem, bool>>();

        static Dictionary<string, TryGetColorCorrectionDelegate> s_TryGetColorCorrectionDelegates =
            new Dictionary<string, TryGetColorCorrectionDelegate>();

        static Dictionary<string, Func<XRCameraSubsystem, IntPtr>> s_GetNativePtrDelegates =
            new Dictionary<string, Func<XRCameraSubsystem, IntPtr>>();

        static Dictionary<string, ICameraImageApi> s_CameraImageApis =
            new Dictionary<string, ICameraImageApi>();

        static Dictionary<string, ICameraConfigApi> s_CameraConfigApis =
            new Dictionary<string, ICameraConfigApi>();

        static Dictionary<string, Func<XRCameraSubsystem, CameraFocusMode, bool>> s_TrySetFocusModeDelegates =
            new Dictionary<string, Func<XRCameraSubsystem, CameraFocusMode, bool>>();
    }
}
