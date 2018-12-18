using System;
using UnityEngine.Experimental;

namespace UnityEngine.XR.FaceSubsystem
{
    namespace Providing
    {
        [Flags]
        public enum FaceSubsystemCapabilities
        {
            None = 0,
            Pose = 1 << 0,
            MeshVerticesAndIndices = 1 << 1,
            MeshUVs = 1 << 2,
        }
    
        /// <summary>
        /// This struct is an initializer for the creation of a <see cref="XRFaceSubsystemDescriptor"/>.
        /// </summary>
        /// <remarks>
        /// Face Tracking data provider should create during <c>InitializeOnLoad<c> a descriptor using
        /// the params here to specify which of the XRFaceSubsystem features it supports.
        /// </remarks>
        public struct FaceSubsystemParams : IEquatable<FaceSubsystemParams>
        {
            public string id;
            public Type implementationType;
    
            public bool supportsFacePose
            {
                get {  return (capabilities & FaceSubsystemCapabilities.Pose) != 0;  }
                set
                {
                    if (value)
                    {
                        capabilities |= FaceSubsystemCapabilities.Pose;
                    }
                    else
                    {
                        capabilities &= ~FaceSubsystemCapabilities.Pose;
                    }
                }
            }
    
            public bool supportsFaceMeshVerticesAndIndices
            {
                get {  return (capabilities & FaceSubsystemCapabilities.MeshVerticesAndIndices) != 0;  }
                set
                {
                    if (value)
                    {
                        capabilities |= FaceSubsystemCapabilities.MeshVerticesAndIndices;
                    }
                    else
                    {
                        capabilities &= ~FaceSubsystemCapabilities.MeshVerticesAndIndices;
                    }
                }
            }
    
            public bool supportsFaceMeshUVs
            {
                get {  return (capabilities & FaceSubsystemCapabilities.MeshUVs) != 0;  }
                set
                {
                    if (value)
                    {
                        capabilities |= FaceSubsystemCapabilities.MeshUVs;
                    }
                    else
                    {
                        capabilities &= ~FaceSubsystemCapabilities.MeshUVs;
                    }
                }
            }
    
            FaceSubsystemCapabilities capabilities { get; set; }
    
            //IEquatable boilerplate
            public bool Equals(FaceSubsystemParams other)
            {
                return capabilities == other.capabilities && id.Equals(other.id) && implementationType == other.implementationType;
            }
    
            public override bool Equals(object obj)
            {
                if (!(obj is FaceSubsystemParams))
                {
                    return false;
                }
    
                return Equals((FaceSubsystemParams)obj);
            }
    
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = id.GetHashCode();
                    hashCode = (hashCode * 486187739) + implementationType.GetHashCode();
                    hashCode = (hashCode * 486187739) + capabilities.GetHashCode();
                    return hashCode;
                }
            }
    
            public static bool operator==(FaceSubsystemParams lhs, FaceSubsystemParams rhs)
            {
                return lhs.Equals(rhs);
            }
    
            public static bool operator!=(FaceSubsystemParams lhs, FaceSubsystemParams rhs)
            {
                return !lhs.Equals(rhs);
            }
        }

    }
    
    /// <summary>
    /// The descriptor of the <see cref="XRFaceSubsystem"/> that shows which face tracking features are available on that XRSubsystem.
    /// </summary>
    /// <remarks>
    /// You use the <c>Create<c> factory method along with <see cref="FaceSubsystemParams"/> struct to construct and
    /// register one of these from each face tracking data provider.
    /// </remarks>
    public class XRFaceSubsystemDescriptor : SubsystemDescriptor<XRFaceSubsystem>
    {
        XRFaceSubsystemDescriptor(Providing.FaceSubsystemParams descriptorParams)
        {
            id = descriptorParams.id;
            subsystemImplementationType = descriptorParams.implementationType;
            supportsFacePose = descriptorParams.supportsFacePose;
            supportsFaceMeshVerticesAndIndices = descriptorParams.supportsFaceMeshVerticesAndIndices;
            supportsFaceMeshUVs = descriptorParams.supportsFaceMeshUVs;
        }

        public bool supportsFacePose { get; }
        public bool supportsFaceMeshVerticesAndIndices { get; }
        public bool supportsFaceMeshUVs { get; }

        public static void Create(Providing.FaceSubsystemParams descriptorParams)
        {
            var descriptor = new XRFaceSubsystemDescriptor(descriptorParams);
            SubsystemRegistration.CreateDescriptor(descriptor);
        }
    }
}
