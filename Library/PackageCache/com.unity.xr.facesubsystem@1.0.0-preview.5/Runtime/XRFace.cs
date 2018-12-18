using System;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.FaceSubsystem
{
    /// <summary>
    /// A struct describing face data that is stored in the <see cref="XRFaceSubsystem"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XRFace : IEquatable<XRFace>
    {
        // Fields to marshall/serialize from native code
        TrackableId m_TrackableId;
        Pose m_Pose;
        int m_WasUpdated;
        int m_IsTracked;

        /// <summary>
        /// The unique <see cref="TrackableId"/> of the face as a trackable within the <see cref="XRFaceSubsystem"/>.
        /// </summary>
        /// <remarks>
        /// With this, you are able to extract more data about this particular face from the <see cref="XRFaceSubsystem"/>. </remarks>
        public TrackableId trackableId
        {
            get { return m_TrackableId; }
        }

        /// <summary>
        /// The <see cref="pose"/> of the face describes its position and rotation in device space
        /// </summary>
        public Pose pose
        {
            get { return m_Pose; }
        }

        /// <summary>
        /// Keeps track of whether this <see cref="XRFace"/> has been updated since it was last added.
        /// </summary>
        public bool wasUpdated
        {
            get { return m_WasUpdated != 0; }
        }

        /// <summary>
        /// Updated value that specifies whether this face is still being tracked or not.
        /// </summary>
        public bool isTracked
        {
            get { return m_IsTracked != 0; }
        }

        //IEquatable boilerplate
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is XRFace && Equals((XRFace)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = trackableId.GetHashCode();
                hashCode = (hashCode * 486187739) + pose.GetHashCode();
                hashCode = (hashCode * 486187739) + wasUpdated.GetHashCode();
                hashCode = (hashCode * 486187739) + isTracked.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator==(XRFace lhs, XRFace rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(XRFace lhs, XRFace rhs)
        {
            return !lhs.Equals(rhs);
        }

        public bool Equals(XRFace other)
        {
            return trackableId.Equals(other.trackableId) && pose.Equals(other.pose) && wasUpdated == other.wasUpdated && isTracked == other.isTracked;
        }
    };
}
