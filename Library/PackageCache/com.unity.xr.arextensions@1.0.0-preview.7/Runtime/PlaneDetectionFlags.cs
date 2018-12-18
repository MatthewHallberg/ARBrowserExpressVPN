using System;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// Used to configure the types of planes to detect.
    /// </summary>
    [Flags]
    public enum PlaneDetectionFlags
    {
        /// <summary>
        /// Plane detection is disabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// Plane detection will only detect horizontal planes.
        /// </summary>
        Horizontal = 1 << 0,

        /// <summary>
        /// Plane detection will only detect vertical planes.
        /// </summary>
        Vertical = 1 << 1
    }

    /// <summary>
    /// An attribute that can be placed on <c>MonoBehaviour</c> fields to 
    /// generate the correct UI for the <see cref="PlaneDetectionFlags"/>
    /// enum.
    /// </summary>
    public class PlaneDetectionFlagsMaskAttribute : PropertyAttribute { }
}
