using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// Holds information related to an asynchronous camera image conversion request.
    /// Returned by <see cref="CameraImage.ConvertAsync(CameraImageConversionParams)"/>.
    /// </summary>
    public struct AsyncCameraImageConversion : IDisposable, IEquatable<AsyncCameraImageConversion>
    {
        /// <summary>
        /// The <see cref="CameraImageConversionParams"/> used during the conversion.
        /// </summary>
        public CameraImageConversionParams conversionParams { get; private set; }

        /// <summary>
        /// The status of the request.
        /// </summary>
        public AsyncCameraImageConversionStatus status
        {
            get
            {
                if (m_CameraImageApi == null)
                    return AsyncCameraImageConversionStatus.Disposed;

                return m_CameraImageApi.GetAsyncRequestStatus(m_RequestId);
            }
        }

        /// <summary>
        /// Get the raw image data. The returned <c>NativeArray</c> is a direct "view" into
        /// the native memory. The memory is only valid until this <see cref="AsyncCameraImageConversion"/>
        /// is disposed.
        /// 
        /// You should only call this method when <see cref="status"/> is equal to <see cref="AsyncCameraImageConversionStatus.Ready"/>.
        /// This method throws otherwise.
        /// </summary>
        /// <typeparam name="T">The type of data to return. No conversion is performed based on the type; this is merely for access convenience.</typeparam>
        /// <returns>A new <c>NativeArray</c> representing the raw image data. This method may fail; use <c>NativeArray.IsCreated</c> to determine
        /// the validity of the data.</returns>
        public unsafe NativeArray<T> GetData<T>() where T : struct
        {
            if (status != AsyncCameraImageConversionStatus.Ready)
                throw new InvalidOperationException("Async request is not ready.");

            IntPtr dataPtr;
            int dataLength;
            if (m_CameraImageApi.TryGetAsyncRequestData(m_RequestId, out dataPtr, out dataLength))
            {
                int stride = UnsafeUtility.SizeOf<T>();
                var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(
                    (void*)dataPtr, dataLength / stride, Allocator.None);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, m_SafetyHandle);
#endif
                return array;
            }
            else
            {
                throw new InvalidOperationException("The AsyncCameraImageConversion is not valid.");
            }
        }

        /// <summary>
        /// Dispose native resources associated with this request, including the raw image data.
        /// The <c>NativeArray</c> returned by <see cref="GetData{T}"/> is invalidated immediately
        /// after calling <c>Dispose</c>.
        /// </summary>
        public void Dispose()
        {
            if (m_CameraImageApi == null || m_RequestId == 0)
                return;

            m_CameraImageApi.DisposeAsyncRequest(m_RequestId);
            m_CameraImageApi = null;
            m_RequestId = 0;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_SafetyHandle);
#endif
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = conversionParams.GetHashCode();
                hash = hash * 486187739 + m_RequestId.GetHashCode();
                if (m_CameraImageApi != null)
                    hash = hash * 486187739 + m_CameraImageApi.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AsyncCameraImageConversion))
                return false;

            return Equals((AsyncCameraImageConversion)obj);
        }

        public override string ToString()
        {
            return string.Format(
                "ConversionParams: {0}", conversionParams);
        }

        public bool Equals(AsyncCameraImageConversion other)
        {
            return
                (conversionParams.Equals(other.conversionParams)) &&
                (m_RequestId == other.m_RequestId) &&
                (m_CameraImageApi == other.m_CameraImageApi);
        }

        public static bool operator ==(AsyncCameraImageConversion lhs, AsyncCameraImageConversion rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(AsyncCameraImageConversion lhs, AsyncCameraImageConversion rhs)
        {
            return !lhs.Equals(rhs);
        }

        internal AsyncCameraImageConversion(ICameraImageApi api, int nativeHandle, CameraImageConversionParams conversionParams)
        {
            m_CameraImageApi = api;
            m_RequestId = m_CameraImageApi.ConvertAsync(nativeHandle, conversionParams);
            this.conversionParams = conversionParams;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_SafetyHandle = AtomicSafetyHandle.Create();
#endif
        }

        ICameraImageApi m_CameraImageApi;
        int m_RequestId;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle m_SafetyHandle;
#endif
    }
}
