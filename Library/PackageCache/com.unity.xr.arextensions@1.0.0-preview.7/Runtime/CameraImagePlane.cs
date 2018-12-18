using System;
using Unity.Collections;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// Information about the camera image planes. An image "plane" refers to an image channel used in video encoding.
    /// </summary>
    public struct CameraImagePlane : IEquatable<CameraImagePlane>
    {
        /// <summary>
        /// The number of bytes per row for this plane.
        /// </summary>
        public int rowStride { get; internal set; }

        /// <summary>
        /// The number of bytes per pixel for this plane.
        /// </summary>
        public int pixelStride { get; internal set; }

        /// <summary>
        /// A "view" into the platform-specific plane data. It is an error to access <c>data</c> after
        /// the owning <see cref="CameraImage"/> has been disposed.
        /// </summary>
        public NativeArray<byte> data { get; internal set; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = data.GetHashCode();
                hash = hash * 486187739 + rowStride.GetHashCode();
                hash = hash * 486187739 + pixelStride.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CameraImagePlane))
                return false;

            return Equals((CameraImagePlane)obj);
        }

        public override string ToString()
        {
            return string.Format("(Data: {0}, Row Stride: {1}, Pixel Stride: {2})",
                data.ToString(), rowStride, pixelStride);
        }

        public bool Equals(CameraImagePlane other)
        {
            return
                (data.Equals(other.data)) &&
                (rowStride == other.rowStride) &&
                (pixelStride == other.pixelStride);
        }

        public static bool operator ==(CameraImagePlane lhs, CameraImagePlane rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CameraImagePlane lhs, CameraImagePlane rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
