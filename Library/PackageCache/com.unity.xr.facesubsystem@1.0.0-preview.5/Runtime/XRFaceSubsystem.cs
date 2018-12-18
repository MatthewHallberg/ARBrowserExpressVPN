using System.Collections.Generic;
using System;
using UnityEngine.Experimental;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.FaceSubsystem
{
    /// <summary>
    /// An abstract class that provides a generic API for low-level face tracking features.
    /// </summary>
    /// <remarks>
    /// This class can be used to access face tracking features in your app via accessing the generic API.
    /// It can also be extended to provide an implementation of a provider which provides the face tracking data
    /// to the higher level code.
    /// </remarks>
    public abstract class XRFaceSubsystem : Subsystem<XRFaceSubsystemDescriptor>
    {
        static List<FaceAddedEventArgs> s_FacesAddedThisFrame;
        static List<FaceUpdatedEventArgs> s_FacesUpdatedThisFrame;
        static List<FaceRemovedEventArgs> s_FacesRemovedThisFrame;

        /// <summary>
        /// This event is invoked with a <see cref="FaceAddedEventArgs"/> whenever the underlying subsystem detects and adds a face.
        /// </summary>
        public event Action<FaceAddedEventArgs> faceAdded;

        /// <summary>
        /// This event is invoked with a <see cref="FaceUpdatedEventArgs"/> whenever the underlying subsystem updates a face.
        /// </summary>
        public event Action<FaceUpdatedEventArgs> faceUpdated;

        /// <summary>
        /// This event is invoked with a <see cref="FaceRemovedEventArgs"/> whenever the underlying subsystem has been told to remove a face.
        /// </summary>
        public event Action<FaceRemovedEventArgs> faceRemoved;

        protected XRFaceSubsystem()
        {
            s_FacesAddedThisFrame = new List<FaceAddedEventArgs>();
            s_FacesUpdatedThisFrame = new List<FaceUpdatedEventArgs>();
            s_FacesRemovedThisFrame = new List<FaceRemovedEventArgs>();
        }

        /// <summary>
        /// Get all currently tracked <see cref="XRFace"/>s.
        /// </summary>
        /// <param name="facesOut"> Replaces the contents with the current list of faces.</param>
        /// <returns>True if current faces were successfully populated.</returns>
        public virtual bool TryGetAllFaces(List<XRFace> facesOut)
        {
            if (facesOut == null)
                throw new ArgumentNullException("facesOut");

            //default implementation clears list to indicate none were returned
            facesOut.Clear();
            return false;
        }

        /// <summary>
        /// Remove the <see cref="XRFace"/> that corresponds to a specific <see cref="TrackableId"/>.
        /// </summary>
        /// <param name="faceId"><see cref="TrackableId"/> of the face.</param>
        /// <returns>True if face was removed.</returns>
        public virtual bool TryRemoveFace(TrackableId faceId)
        {
            //default implementation does not have any faces to remove
            return false;
        }

        /// <summary>
        /// Get the mesh vertices of the face that corresponds to a specific <see cref="TrackableId"/>.
        /// </summary>
        /// <param name="faceId"><see cref="TrackableId"/> of the face whose mesh we need.</param>
        /// <param name="verticesOut"> Replaces the content with the list of <see cref="Vector3"/> vertices for this face mesh.</param>
        /// <returns>True if face mesh vertices were successfully populated.</returns>
        public virtual bool TryGetFaceMeshVertices(TrackableId faceId, List<Vector3> verticesOut)
        {
            if (verticesOut == null)
                throw new ArgumentNullException("verticesOut");

            //default implementation clears list to indicate none were returned
            verticesOut.Clear();
            return false;
        }

        /// <summary>
        /// Get the mesh texture coordinates of the face that corresponds to a specific <see cref="TrackableId"/>.
        /// </summary>
        /// <param name="faceId"><see cref="TrackableId"/> of the face whose mesh we need.</param>
        /// <param name="uvsOut"> Replaces the content with the list of <see cref="Vector2"/> texture coordinates for this face mesh.</param>
        /// <returns>True if face mesh UVs were successfully populated.</returns>
        public virtual bool TryGetFaceMeshUVs(TrackableId faceId, List<Vector2> uvsOut)
        {
            if (uvsOut == null)
                throw new ArgumentNullException("uvsOut");

            //default implementation clears list to indicate none were returned
            uvsOut.Clear();
            return false;
        }

        /// <summary>
        /// Get the mesh triangle indices of the face that corresponds to a specific <see cref="TrackableId"/>.
        /// </summary>
        /// <param name="faceId"><see cref="TrackableId"/> of the face whose mesh we need.</param>
        /// <param name="indicesOut"> Replaces the content with the list of integer triangle indices for this face mesh.</param>
        /// <returns>True if face mesh triangle indices were successfully populated.</returns>
        public virtual bool TryGetFaceMeshIndices(TrackableId faceId, List<int> indicesOut)
        {
            if (indicesOut == null)
                throw new ArgumentNullException("indicesOut");

            //default implementation clears list to indicate none were returned
            indicesOut.Clear();
            return false;
        }

        /// <summary>
        /// Get the <see cref="TrackingState"/> of the face that corresponds to a specific <see cref="TrackableId"/>.
        /// </summary>
        /// <param name="faceId"><see cref="TrackableId"/> of the face whose <see cref="TrackingState"/> we need.</param>
        /// <returns><see cref="TrackingState"/> of the face that corresponds to <see cref="TrackableId"/></returns>
        public virtual TrackingState GetTrackingState(TrackableId faceId)
        {
            return TrackingState.Unknown;
        }

        /// <summary>
        /// Derived classes will call this method to inform the subsystem of each face added this frame
        /// </summary>
        /// <param name="xrFace"><see cref="XRFace"/> that was added.</param>
        /// <param name="faceSubsystem"><see cref="XRFaceSubsystem"/> this face belongs to.</param>
        protected static void InvokeFaceAddedCallback(XRFace xrFace, XRFaceSubsystem faceSubsystem)
        {
            var faceAddedEventArgs = new FaceAddedEventArgs
            {
                xrFace = xrFace,
                xrFaceSubsystem = faceSubsystem
            };
            s_FacesAddedThisFrame.Add(faceAddedEventArgs);
        }

        /// <summary>
        /// Derived classes will call this method to inform the subsystem of each face updated this frame
        /// </summary>
        /// <param name="xrFace"><see cref="XRFace"/> that was updated.</param>
        /// <param name="faceSubsystem"><see cref="XRFaceSubsystem"/> this face belongs to.</param>
        protected static void InvokeFaceUpdatedCallback(XRFace xrFace, XRFaceSubsystem faceSubsystem)
        {
            var faceUpdatedEventArgs = new FaceUpdatedEventArgs
            {
                xrFace = xrFace,
                xrFaceSubsystem = faceSubsystem
            };
            s_FacesUpdatedThisFrame.Add(faceUpdatedEventArgs);
        }

        /// <summary>
        /// Derived classes will call this method to inform the subsystem of each face removed this frame
        /// </summary>
        /// <param name="xrFace"><see cref="XRFace"/> that was removed.</param>
        /// <param name="faceSubsystem"><see cref="XRFaceSubsystem"/> this face belongs to.</param>
        protected static void InvokeFaceRemovedCallback(XRFace xrFace, XRFaceSubsystem faceSubsystem)
        {
            var faceRemovedEventArgs = new FaceRemovedEventArgs
            {
                xrFace = xrFace,
                xrFaceSubsystem = faceSubsystem
            };
            s_FacesRemovedThisFrame.Add(faceRemovedEventArgs);
        }

        /// <summary>
        /// Derived classes will call this method at the beginning of a Unity frame so that the Subsystem
        /// can feed the face events up to their subscribers at the right time in the Unity frame.
        /// </summary>
        protected static void OnBeginFrame()
        {
            foreach (var faceAddedEventArgs in s_FacesAddedThisFrame)
            {
                XRFaceSubsystem faceSubsystem = faceAddedEventArgs.xrFaceSubsystem;
                if (faceSubsystem.faceAdded != null)
                {
                    faceSubsystem.faceAdded.Invoke(faceAddedEventArgs);
                }
            }

            foreach (var faceUpdatedEventArgs in s_FacesUpdatedThisFrame)
            {
                XRFaceSubsystem faceSubsystem = faceUpdatedEventArgs.xrFaceSubsystem;
                if (faceSubsystem.faceUpdated != null)
                {
                    faceSubsystem.faceUpdated.Invoke(faceUpdatedEventArgs);
                }
            }

            foreach (var faceRemovedEventArgs in s_FacesRemovedThisFrame)
            {
                XRFaceSubsystem faceSubsystem = faceRemovedEventArgs.xrFaceSubsystem;
                if (faceSubsystem.faceRemoved != null)
                {
                    faceSubsystem.faceRemoved.Invoke(faceRemovedEventArgs);
                }
            }

            s_FacesAddedThisFrame.Clear();
            s_FacesUpdatedThisFrame.Clear();
            s_FacesRemovedThisFrame.Clear();
        }
    }
}
