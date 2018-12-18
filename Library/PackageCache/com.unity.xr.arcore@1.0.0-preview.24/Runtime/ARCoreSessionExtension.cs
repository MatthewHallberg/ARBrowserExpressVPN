using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARExtensions;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// For internal use. Provides ARCore-specific extensions to the XRSessionSubsystem.
    /// </summary>
    internal class ARCoreSessionExtension
    {
        /// <summary>
        /// For internal use. Use <c>XRSessionSubsystem.InstallAsync</c> instead.
        /// </summary>
        /// <remarks>
        /// Provides a means to install ARCore if the device supports it but needs an APK update.
        /// </remarks>
        /// <param name="sessionSubsystem">The <c>XRSessionSubsystem</c> which this method extends.</param>
        /// <param name="callback">A callback to invoke when the installation process has completed.</param>
        public static Promise<SessionInstallationStatus> InstallAsync(XRSessionSubsystem sessionSubsystem)
        {
            return ExecuteAsync<SessionInstallationStatus>((context) =>
            {
                Api.ArPresto_requestApkInstallation(true, OnApkInstallation, context);
            });
        }

        /// <summary>
        /// For internal use. Use <c>XRSessionSubsystem.GetAvailabilityAsync</c> instead.
        /// </summary>
        /// <param name="sessionSubsystem">The <c>XRSessionSubsystem</c> which this method extends.</param>
        /// <param name="callback">A callback to invoke when the availability has been determined.</param>
        public static Promise<SessionAvailability> GetAvailabilityAsync(XRSessionSubsystem sessionSubsystem)
        {
            return ExecuteAsync<SessionAvailability>((context) =>
            {
                Api.ArPresto_checkApkAvailability(OnCheckApkAvailability, context);
            });
        }

        /// <summary>
        /// For internal use. Use <c>XRSessionSubsystem.GetNativePtr</c> instead.
        /// </summary>
        /// <param name="sessionSubsystem">The <c>XRSessionSubsystem</c> which this method extends.</param>
        /// <returns>An <c>IntPtr</c> associated with the <paramref name="sessionSubsystem"/>.</returns>
        public static IntPtr GetNativePtr(XRSessionSubsystem sessionSubsystem)
        {
            return Api.UnityARCore_getNativeSessionPtr();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            XRSessionExtensions.RegisterGetAvailabilityAsyncHandler(k_SubsystemId, GetAvailabilityAsync);
            XRSessionExtensions.RegisterInstallAsyncHandler(k_SubsystemId, InstallAsync);
            XRSessionExtensions.RegisterGetNativePtrHandler(k_SubsystemId, GetNativePtr);
        }

        static Promise<T> ExecuteAsync<T>(Action<IntPtr> apiMethod)
        {
            var promise = new ARCorePromise<T>();
            GCHandle gch = GCHandle.Alloc(promise);
            apiMethod(GCHandle.ToIntPtr(gch));
            return promise;
        }

        [MonoPInvokeCallback(typeof(Action<Api.ArPrestoApkInstallStatus, IntPtr>))]
        static void OnApkInstallation(Api.ArPrestoApkInstallStatus status, IntPtr context)
        {
            ResolvePromise(context, status.AsSessionInstallationStatus());
        }

        [MonoPInvokeCallback(typeof(Action<Api.ArAvailability, IntPtr>))]
        static void OnCheckApkAvailability(Api.ArAvailability availability, IntPtr context)
        {
            ResolvePromise(context, availability.AsSessionAvailability());
        }

        static void ResolvePromise<T>(IntPtr context, T arg) where T : struct
        {
            GCHandle gch = GCHandle.FromIntPtr(context);
            var promise = (ARCorePromise<T>)gch.Target;
            if (promise != null)
                promise.Resolve(arg);
            gch.Free();
        }

        static readonly string k_SubsystemId = "ARCore-Session";
    }
}
