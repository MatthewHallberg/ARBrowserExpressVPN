using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// An enumerable for <see cref="CameraConfiguration"/>s.
    /// 
    /// See also <see cref="XRCameraExtensions.Configurations(XRCameraSubsystem)"/>.
    /// </summary>
    public struct CameraConfigurationCollection : IEnumerable<CameraConfiguration>, IEnumerable
    {
        /// <summary>
        /// Get an enumerator for this enumerable.
        /// </summary>
        /// <returns>An enumerator for <see cref="CameraConfiguration"/>.</returns>
        public ConfigEnumerator GetEnumerator()
        {
            return new ConfigEnumerator(m_CameraConfigApi);
        }

        /// <summary>
        /// Constructs an enumerable from an <c>XRCameraSubsystem</c>
        /// </summary>
        /// <param name="api">The <see cref="ICameraConfigApi"/> required to enumerate the supported configurations.</param>
        public CameraConfigurationCollection(ICameraConfigApi api)
        {
            if (api == null)
                throw new ArgumentNullException("api");

            m_CameraConfigApi = api;
        }

        /// <summary>
        /// Returns the number of configurations in this collection.
        /// </summary>
        public int count
        {
            get
            {
                if (m_CameraConfigApi == null)
                    throw new InvalidOperationException("This collection has not been initialized.");

                return m_CameraConfigApi.GetConfigurationCount();
            }
        }

        /// <summary>
        /// Get a configuration by <paramref name="index"/>. Throws if index is out of bounds.
        /// </summary>
        /// <param name="index">The index of the configuration to retrieve. Must be greater
        /// than or equal to zero and less than the return value of <see cref="count"/>.</param>
        /// <returns>The <see cref="CameraConfiguration"/> at <paramref name="index"/>.</returns>
        public CameraConfiguration this[int index]
        {
            get
            {
                if (m_CameraConfigApi == null)
                    throw new InvalidOperationException("This collection has not been initialized.");

                if (index < 0)
                    throw new IndexOutOfRangeException(string.Format("{0} is not a valid camera configuration index.", index));

                return m_CameraConfigApi.GetConfiguration(index);
            }
        }

        IEnumerator<CameraConfiguration> IEnumerable<CameraConfiguration>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ICameraConfigApi m_CameraConfigApi;

        /// <summary>
        /// A custom enumerator for <see cref="CameraConfiguration"/>s.
        /// </summary>
        public struct ConfigEnumerator : IEnumerator, IEnumerator<CameraConfiguration>
        {
            /// <summary>
            /// Constructs an enumerator from an <c>XRCameraSubsystem</c>
            /// </summary>
            /// <param name="api">The <see cref="ICameraConfigApi"/> required to enumerate the supported configurations.</param>
            public ConfigEnumerator(ICameraConfigApi api)
            {
                if (api == null)
                    throw new ArgumentNullException("api");

                m_CameraConfigApi = api;
                m_Index = -1;
                m_Count = m_CameraConfigApi.GetConfigurationCount();
            }

            /// <summary>
            /// Move to the next supported configuration.
            /// </summary>
            /// <returns><c>true</c> if there are more configurations, <c>false</c> otherwise.</returns>
            public bool MoveNext()
            {
                return (++m_Index < m_Count);
            }

            /// <summary>
            /// Get the current <see cref="CameraConfiguration"/>. Throws if there are no more
            /// configurations, i.e., if <see cref="MoveNext"/> has returned false.
            /// </summary>
            public CameraConfiguration Current
            {
                get
                {
                    if (m_Index >= m_Count)
                        throw new IndexOutOfRangeException("No more configurations in list.");

                    return m_CameraConfigApi.GetConfiguration(m_Index);
                }
            }

            /// <summary>
            /// Dispose the enumerator. Provided for compatibility with <c>IEnumerator</c>.
            /// </summary>
            public void Dispose() { }

            object IEnumerator.Current { get { return Current; } }

            void IEnumerator.Reset()
            {
                m_Index = -1;
            }

            int m_Index;

            int m_Count;

            ICameraConfigApi m_CameraConfigApi;
        }
    }
}
