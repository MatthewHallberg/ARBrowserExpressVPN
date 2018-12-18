using System;
using UnityEngine.XR.ARExtensions;

namespace UnityEngine.XR.ARCore
{
    internal class ARCoreCameraConfigApi : ICameraConfigApi
    {
        public int GetConfigurationCount()
        {
            return Api.UnityARCore_cameraImage_getConfigurationCount();
        }

        public CameraConfiguration GetConfiguration(int index)
        {
            CameraConfiguration configuration;
            if (!Api.UnityARCore_cameraImage_tryGetConfiguration(index, out configuration))
                throw new IndexOutOfRangeException(string.Format(
                    "Configuration index {0} is out of range", index));

            return configuration;
        }

        public CameraConfiguration currentConfiguration
        {
            get
            {
                CameraConfiguration configuration;
                if (!Api.UnityARCore_cameraImage_tryGetCurrentConfiguration(out configuration))
                    throw new InvalidOperationException("Configuration could not be retrieved");

                return configuration;
            }
            set
            {
                switch (Api.UnityARCore_cameraImage_trySetConfiguration(value))
                {
                    case Api.SetCameraConfigurationResult.ErrorInvalidConfiguration:
                        throw new InvalidOperationException("Invalid camera image configuration.");
                    case Api.SetCameraConfigurationResult.ErrorCameraImagesNotDisposed:
                        throw new InvalidOperationException("You must dispose of all CameraImages (and all asynchronous conversion jobs must complete) before setting the camera image configuration.");
                    case Api.SetCameraConfigurationResult.Success:
                        break;
                }
            }
        }
    }
}
