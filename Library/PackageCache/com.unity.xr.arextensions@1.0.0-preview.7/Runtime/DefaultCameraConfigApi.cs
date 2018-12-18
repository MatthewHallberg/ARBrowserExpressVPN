using System;

namespace UnityEngine.XR.ARExtensions
{
    internal class DefaultCameraConfigApi : ICameraConfigApi
    {
        public int GetConfigurationCount()
        {
            return 0;
        }

        public CameraConfiguration GetConfiguration(int index)
        {
            throw new NotSupportedException("This platform does not support camera configurations.");
        }

        public CameraConfiguration currentConfiguration
        {
            get
            {
                throw new NotSupportedException("This platform does not support camera configurations.");
            }
            set
            {
                throw new NotSupportedException("This platform does not support camera configurations.");
            }
        }
    }
}
