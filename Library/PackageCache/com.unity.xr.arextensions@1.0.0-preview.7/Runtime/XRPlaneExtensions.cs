using System;
using System.Collections.Generic;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// Provides extensions to the <c>XRPlaneSubsystem</c>.
    /// </summary>
    public static class XRPlaneExtensions
    {
        /// <summary>
        /// For internal use. Allows a plane provider to register for the
        /// <see cref="GetTrackingState(XRPlaneSubsystem, TrackableId)"/> extension.
        /// </summary>
        /// <param name="subsystemId">The string name associated with the plane provider to extend.</param>
        /// <param name="handler">A method that returns the <c>TrackingState</c> of the given <c>TrackableId</c>.</param>
        public static void RegisterGetTrackingStateHandler(string subsystemId, Func<XRPlaneSubsystem, TrackableId, TrackingState> handler)
        {
            s_GetTrackingStateDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// For internal use. Allows a plane provider to register for the
        /// <see cref="GetNativePtr(XRPlaneSubsystem, TrackableId)"/> extension.
        /// </summary>
        /// <param name="subsystemId">The string name associated with the plane provider to extend.</param>
        /// <param name="handler">A method that returns the <c>IntPtr</c> associated with a given <c>TrackableId</c>.</param>
        public static void RegisterGetNativePtrHandler(string subsystemId, Func<XRPlaneSubsystem, TrackableId, IntPtr> handler)
        {
            s_GetNativePtrDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// For internal use. Allows a plane provider to register for the
        /// <see cref="TrySetPlaneDetectionFlags(XRPlaneSubsystem, PlaneDetectionFlags)"/> extension.
        /// </summary>
        /// <param name="subsystemId">The string name associated with the plane provider to extend.</param>
        /// <param name="handler">A method that attempts to set the plane detection flags and returns <c>true</c>
        /// if successful, or <c>false</c> otherwise.</param>
        public static void RegisterTrySetPlaneDetectionFlagsHandler(
            string subsystemId,
            Func<XRPlaneSubsystem, PlaneDetectionFlags, bool> handler)
        {
            s_TrySetPlaneDetectionFlagsDelegates[subsystemId] = handler;
        }

        /// <summary>
        /// Retrieve the <c>TrackingState</c> of the given <paramref name="planeId"/>.
        /// </summary>
        /// <param name="planeSubsystem">The <c>XRPlaneSubsystem</c> being extended.</param>
        /// <param name="planeId">The <c>TrackableId</c> associated with this plane.</param>
        /// <returns>The <c>TrackingState</c> of the plane with id <paramref name="planeId"/>.</returns>
        public static TrackingState GetTrackingState(this XRPlaneSubsystem planeSubsystem, TrackableId planeId)
        {
            if (planeSubsystem == null)
                throw new ArgumentNullException("planeSubsystem");

            return s_GetTrackingStateDelegate(planeSubsystem, planeId);
        }

        static TrackingState DefaultGetTrackingState(this XRPlaneSubsystem planeSubsystem, TrackableId planeId)
        {
            return TrackingState.Unknown;
        }

        /// <summary>
        /// Retrieves a native <c>IntPtr</c> associated with a plane with <c>TrackableId</c>
        /// <paramref name="trackableId"/>.
        /// </summary>
        /// <param name="planeSubsystem">The <c>XRPlaneSubsystem</c> being extended.</param>
        /// <param name="trackableId">The <c>TrackableId</c> of a reference point.</param>
        /// <returns>An <c>IntPtr</c> associated with the reference point, or <c>IntPtr.Zero</c> if unavailable.</returns>
        public static IntPtr GetNativePtr(this XRPlaneSubsystem planeSubsystem,
            TrackableId planeId)
        {
            if (planeSubsystem == null)
                throw new ArgumentNullException("planeSubsystem");

            return s_GetNativePtrDelegate(planeSubsystem, planeId);
        }

        /// <summary>
        /// Attempt to set the <see cref="PlaneDetectionFlags"/>.
        /// </summary>
        /// <param name="planeSubsystem">The <c>XRPlaneSubsystem</c> being extended.</param>
        /// <param name="flags">The plane detection mode(s) to enable.</param>
        /// <returns><c>true</c> if the flags were successfully set, <c>false</c> otherwise.</returns>
        public static bool TrySetPlaneDetectionFlags(this XRPlaneSubsystem planeSubsystem, PlaneDetectionFlags flags)
        {
            if (planeSubsystem == null)
                throw new ArgumentNullException("planeSubsystem");

            return s_TrySetPlaneDetectionFlagsDelegate(planeSubsystem, flags);
        }

        static bool DefaultTrySetPlaneDetectionFlags(
            XRPlaneSubsystem planeSubsystem, PlaneDetectionFlags flags)
        {
            return false;
        }

        static IntPtr DefaultGetNativePtr(XRPlaneSubsystem referencePointSubsystem,
            TrackableId trackableId)
        {
            return IntPtr.Zero;
        }

        /// <summary>
        /// Sets the active subsystem whose extension methods should be used.
        /// </summary>
        /// <param name="planeSubsystem">The <c>XRPlaneSubsystem</c> being extended.</param>
        public static void ActivateExtensions(this XRPlaneSubsystem planeSubsystem)
        {
            if (planeSubsystem == null)
            {
                SetDefaultDelegates();
            }
            else
            {
                var id = planeSubsystem.SubsystemDescriptor.id;
                s_GetNativePtrDelegate = RegistrationHelper.GetValueOrDefault(s_GetNativePtrDelegates, id, DefaultGetNativePtr);
                s_GetTrackingStateDelegate = RegistrationHelper.GetValueOrDefault(s_GetTrackingStateDelegates, id, DefaultGetTrackingState);
                s_TrySetPlaneDetectionFlagsDelegate = RegistrationHelper.GetValueOrDefault(s_TrySetPlaneDetectionFlagsDelegates, id, DefaultTrySetPlaneDetectionFlags);
            }
        }

        static void SetDefaultDelegates()
        {
            s_GetNativePtrDelegate = DefaultGetNativePtr;
            s_GetTrackingStateDelegate = DefaultGetTrackingState;
            s_TrySetPlaneDetectionFlagsDelegate = DefaultTrySetPlaneDetectionFlags;
        }

        static XRPlaneExtensions()
        {
            SetDefaultDelegates();
        }

        static Func<XRPlaneSubsystem, TrackableId, IntPtr> s_GetNativePtrDelegate;

        static Func<XRPlaneSubsystem, TrackableId, TrackingState> s_GetTrackingStateDelegate;

        static Func<XRPlaneSubsystem, PlaneDetectionFlags, bool> s_TrySetPlaneDetectionFlagsDelegate;

        static Dictionary<string, Func<XRPlaneSubsystem, TrackableId, IntPtr>> s_GetNativePtrDelegates =
            new Dictionary<string, Func<XRPlaneSubsystem, TrackableId, IntPtr>>();

        static Dictionary<string, Func<XRPlaneSubsystem, TrackableId, TrackingState>> s_GetTrackingStateDelegates =
            new Dictionary<string, Func<XRPlaneSubsystem, TrackableId, TrackingState>>();

        static Dictionary<string, Func<XRPlaneSubsystem, PlaneDetectionFlags, bool>> s_TrySetPlaneDetectionFlagsDelegates =
            new Dictionary<string, Func<XRPlaneSubsystem, PlaneDetectionFlags, bool>>();
    }
}
