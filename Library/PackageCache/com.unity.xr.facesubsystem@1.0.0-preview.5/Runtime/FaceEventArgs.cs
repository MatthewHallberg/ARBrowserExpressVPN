using System;

namespace UnityEngine.XR.FaceSubsystem
{
    /// <summary>
    /// Holds data about face added events triggered by a <see cref="XRFaceSubsystem"/>.
    /// </summary>
    public struct FaceAddedEventArgs : IEquatable<FaceAddedEventArgs>
    {
        /// <summary>
        /// A reference to the <see cref="XRFace"/> that was added.
        /// </summary>
        public XRFace xrFace { get; internal set; }

        /// <summary>
        /// The <see cref="XRFaceSubsystem"/> that triggered this event.
        /// </summary>
        public XRFaceSubsystem xrFaceSubsystem { get; internal set; }

        public bool Equals(FaceAddedEventArgs other)
        {
            return xrFace.Equals(other.xrFace) && Equals(xrFaceSubsystem, other.xrFaceSubsystem);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            
            return obj is FaceAddedEventArgs && Equals((FaceAddedEventArgs)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (xrFace.GetHashCode() * 486187739) + (xrFaceSubsystem != null ? xrFaceSubsystem.GetHashCode() : 0);
            }
        }

        public static bool operator==(FaceAddedEventArgs lhs, FaceAddedEventArgs rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(FaceAddedEventArgs lhs, FaceAddedEventArgs rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct FaceRemovedEventArgs : IEquatable<FaceRemovedEventArgs>
    {
        /// <summary>
        /// A reference to the <see cref="XRFace"/> that was removed.
        /// </summary>
        public XRFace xrFace { get; internal set; }

        /// <summary>
        /// The <see cref="XRFaceSubsystem"/> that triggered this event.
        /// </summary>
        public XRFaceSubsystem xrFaceSubsystem { get; internal set; }

        public bool Equals(FaceRemovedEventArgs other)
        {
            return xrFace.Equals(other.xrFace) && Equals(xrFaceSubsystem, other.xrFaceSubsystem);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            
            return obj is FaceRemovedEventArgs && Equals((FaceRemovedEventArgs)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (xrFace.GetHashCode() * 486187739) + (xrFaceSubsystem != null ? xrFaceSubsystem.GetHashCode() : 0);
            }
        }

        public static bool operator==(FaceRemovedEventArgs left, FaceRemovedEventArgs right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(FaceRemovedEventArgs left, FaceRemovedEventArgs right)
        {
            return !left.Equals(right);
        }
    }

    public struct FaceUpdatedEventArgs : IEquatable<FaceUpdatedEventArgs>
    {
        /// <summary>
        /// A reference to the <see cref="XRFace"/> that was updated.
        /// </summary>
        public XRFace xrFace { get; internal set; }

        /// <summary>
        /// The <see cref="XRFaceSubsystem"/> that triggered this event.
        /// </summary>
        public XRFaceSubsystem xrFaceSubsystem { get; internal set; }

        public bool Equals(FaceUpdatedEventArgs other)
        {
            return xrFace.Equals(other.xrFace) && Equals(xrFaceSubsystem, other.xrFaceSubsystem);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            
            return obj is FaceUpdatedEventArgs && Equals((FaceUpdatedEventArgs)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (xrFace.GetHashCode() * 486187739) + (xrFaceSubsystem != null ? xrFaceSubsystem.GetHashCode() : 0);
            }
        }

        public static bool operator==(FaceUpdatedEventArgs left, FaceUpdatedEventArgs right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(FaceUpdatedEventArgs left, FaceUpdatedEventArgs right)
        {
            return !left.Equals(right);
        }
    }
}
