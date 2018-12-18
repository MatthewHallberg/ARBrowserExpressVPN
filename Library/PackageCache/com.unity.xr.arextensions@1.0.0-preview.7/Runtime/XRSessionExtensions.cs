using System;
using System.Collections.Generic;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// Provides extensions to the <c>XRSessionSubsystem</c>.
    /// </summary>
    public static class XRSessionExtensions
    {
        /// <summary>
        /// A <c>delegate</c> used for asynchronous operations that retrieve data of type <c>T</c>.
        /// </summary>
        /// <typeparam name="T">The type of data to operation retrieves.</typeparam>
        /// <param name="sessionSubsystem">The <c>XRSessionSubsystem</c> being extended.</param>
        /// <returns>A <see cref="Promise{T}"/> used to determine status and result of the asynchronous operation.</returns>
        public delegate Promise<T> AsyncDelegate<T>(XRSessionSubsystem sessionSubsystem);

        /// <summary>
        /// Registers a handler for retrieving a platform-specific <c>IntPtr</c> associated with the session.
        /// </summary>
        /// <remarks>
        /// A platform-specific package should implement a method to get a native ptr and register it using this method.
        /// </remarks>
        /// <param name="subsystemId">The string name associated with the package's session subsystem.</param>
        /// <param name="handler">The delegate which returns an <c>IntPtr</c> associated with the session.</param>
        public static void RegisterGetNativePtrHandler(string subsystemId, Func<XRSessionSubsystem, IntPtr> handler)
        {
            s_GetNativePtrDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// Registers a handler for asynchronous AR software installation. See <see cref="InstallAsync(XRSessionSubsystem)"/>.
        /// </summary>
        /// <remarks>
        /// A platform-specific package should implement an installation handler and register it using this method.
        /// </remarks>
        /// <param name="subsystemId">The string name associated with the package's session subsystem.</param>
        /// <param name="handler">An <see cref="AsyncDelegate{T}"/> to handle the request.</param>
        public static void RegisterInstallAsyncHandler(string subsystemId, AsyncDelegate<SessionInstallationStatus> handler)
        {
            s_InstallAsyncDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// Registers a handler for an asynchronous AR availability check. See <see cref="GetAvailabilityAsync(XRSessionSubsystem)"/>.
        /// </summary>
        /// <remarks>
        /// A platform-specific package should implement an installation handler and register it using this method.
        /// </remarks>
        /// <param name="subsystemId">The string name associated with the package's session subsystem.</param>
        /// <param name="handler">An <see cref="AsyncDelegate{T}"/> to handle the request.</param>
        public static void RegisterGetAvailabilityAsyncHandler(string subsystemId, AsyncDelegate<SessionAvailability> handler)
        {
            s_GetAvailabilityAsyncDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// Asynchronously retrieves the <see cref="SessionAvailability"/>. Used to determine whether
        /// the current device supports AR and if the necessary software is installed.
        /// </summary>
        /// <remarks>
        /// This platform-agnostic method is typically implemented by a platform-specific package.
        /// </remarks>
        /// <param name="sessionSubsystem">The <c>XRSessionSubsystem</c> to extend.</param>
        /// <returns>A <see cref="Promise{SessionAvailability}"/> which can be used to determine when the
        /// availability has been determined and retrieve the result.</returns>
        public static Promise<SessionAvailability> GetAvailabilityAsync(this XRSessionSubsystem sessionSubsystem)
        {
            if (sessionSubsystem == null)
                throw new ArgumentNullException("sessionSubsystem");

            return s_GetAvailabilityAsyncDelegate(sessionSubsystem);
        }

        static Promise<SessionAvailability> DefaultGetAvailabilityAsync(this XRSessionSubsystem sessionSubsystem)
        {
            return Promise<SessionAvailability>.CreateResolvedPromise(
                SessionAvailability.Supported | SessionAvailability.Installed);
        }

        /// <summary>
        /// Asynchronously attempts to install AR software on the current device.
        /// </summary>
        /// <remarks>
        /// This platform-agnostic method is typically implemented by a platform-specific package.
        /// </remarks>
        /// <param name="sessionSubsystem">The <c>XRSessionSubsystem</c> to extend.</param>
        /// <returns>A <see cref="Promise{SessionInstallationStatus}"/> which can be used to determine when the
        /// installation completes and retrieve the result.</returns>
        public static Promise<SessionInstallationStatus> InstallAsync(this XRSessionSubsystem sessionSubsystem)
        {
            if (sessionSubsystem == null)
                throw new ArgumentNullException("sessionSubsystem");

            return s_InstallAsyncDelegate(sessionSubsystem);
        }

        static Promise<SessionInstallationStatus> DefaultInstallAsync(this XRSessionSubsystem sessionSubsystem)
        {
            return Promise<SessionInstallationStatus>.CreateResolvedPromise(
                SessionInstallationStatus.ErrorInstallNotSupported);
        }

        /// <summary>
        /// Retrieves a native <c>IntPtr</c> associated with the <c>XRSessionSubsystem</c>.
        /// Note: This is for advanced usage or access to platform-specific features
        /// not exposed by the existing API.
        /// </summary>
        /// <param name="sessionSubsystem">The <c>XRSessionSubsystem</c> to extend.</param>
        /// <returns>An <c>IntPtr</c> associated with the platform-specific session.</returns>
        public static IntPtr GetNativePtr(this XRSessionSubsystem sessionSubsystem)
        {
            if (sessionSubsystem == null)
                throw new ArgumentNullException("sessionSubsystem");

            return s_GetNativePtrDelegate(sessionSubsystem);
        }

        static IntPtr DefaultGetNativePtr(XRSessionSubsystem sessionSubsystem)
        {
            return IntPtr.Zero;
        }

        /// <summary>
        /// For internal use. Sets the active subsystem whose extension methods should be used.
        /// </summary>
        /// <param name="sessionSubsystem">The <c>XRSessionSubsystem</c> being extended.</param>
        public static void ActivateExtensions(this XRSessionSubsystem sessionSubsystem)
        {
            if (sessionSubsystem == null)
            {
                SetDefaultDelegates();
            }
            else
            {
                var id = sessionSubsystem.SubsystemDescriptor.id;
                s_InstallAsyncDelegate = RegistrationHelper.GetValueOrDefault(s_InstallAsyncDelegates, id, DefaultInstallAsync);
                s_GetAvailabilityAsyncDelegate = RegistrationHelper.GetValueOrDefault(s_GetAvailabilityAsyncDelegates, id, DefaultGetAvailabilityAsync);
                s_GetNativePtrDelegate = RegistrationHelper.GetValueOrDefault(s_GetNativePtrDelegates, id, DefaultGetNativePtr);
            }
        }

        static void SetDefaultDelegates()
        {
            s_InstallAsyncDelegate = DefaultInstallAsync;
            s_GetAvailabilityAsyncDelegate = DefaultGetAvailabilityAsync;
            s_GetNativePtrDelegate = DefaultGetNativePtr;
        }

        static XRSessionExtensions()
        {
            SetDefaultDelegates();
        }

        static AsyncDelegate<SessionInstallationStatus> s_InstallAsyncDelegate;
        static AsyncDelegate<SessionAvailability> s_GetAvailabilityAsyncDelegate;
        static Func<XRSessionSubsystem, IntPtr> s_GetNativePtrDelegate;

        static Dictionary<string, AsyncDelegate<SessionInstallationStatus>> s_InstallAsyncDelegates =
            new Dictionary<string, AsyncDelegate<SessionInstallationStatus>>();

        static Dictionary<string, AsyncDelegate<SessionAvailability>> s_GetAvailabilityAsyncDelegates =
            new Dictionary<string, AsyncDelegate<SessionAvailability>>();

        static Dictionary<string, Func<XRSessionSubsystem, IntPtr>> s_GetNativePtrDelegates =
            new Dictionary<string, Func<XRSessionSubsystem, IntPtr>>();
    }
}
