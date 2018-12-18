using System;
using UnityEngine.XR.ARExtensions;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// For internal use. Provides ARCore-specific extensions to the XRSessionSubsystem.
    /// </summary>
    internal class ARCorePlaneExtension
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            XRPlaneExtensions.RegisterGetTrackingStateHandler(k_SubsystemId, GetTrackingState);
            XRPlaneExtensions.RegisterGetNativePtrHandler(k_SubsystemId, GetNativePtr);
            XRPlaneExtensions.RegisterTrySetPlaneDetectionFlagsHandler(k_SubsystemId, TrySetPlaneDetectionFlags);
        }

        static bool TrySetPlaneDetectionFlags(XRPlaneSubsystem planeSubsystem, PlaneDetectionFlags flags)
        {
            return Api.UnityARCore_trySetPlaneDetectionFlags(flags);
        }

        static TrackingState GetTrackingState(XRPlaneSubsystem planeSubsystem, TrackableId planeId)
        {
            return Api.UnityARCore_getAnchorTrackingState(planeId);
        }

        static IntPtr GetNativePtr(XRPlaneSubsystem planeSubsystem, TrackableId planeId)
        {
            return Api.UnityARCore_getNativePlanePtr(planeId);
        }

        static readonly string k_SubsystemId = "ARCore-Plane";
    }
}
