namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// This interface is typically implemented by platform-specific implementations of the
    /// <c>XRCameraSubsystem</c> to support <see cref="CameraConfiguration"/>s.
    /// End users do not need to implement this. Use <see cref="XRCameraExtensions.Configurations(Experimental.XR.XRCameraSubsystem)"/>.
    /// to get a <see cref="CameraConfigurationCollection"/>.
    /// </summary>
    public interface ICameraConfigApi
    {
        /// <summary>
        /// Gets the number of supported <see cref="CameraConfiguration"/>s.
        /// </summary>
        /// <returns>The number of supported <see cref="CameraConfiguration"/>s</returns>
        int GetConfigurationCount();

        /// <summary>
        /// Get a specific <see cref="CameraConfiguration"/>.
        /// </summary>
        /// <param name="index">The index of the configuration to retrieve. If index less than zero or greater
        /// than or equal to the number of supported configurations, this method should throw <c>IndexOutOfRangeException</c>.</param>
        /// <returns>The <see cref="CameraConfiguration"/> at <paramref name="index"/>.</returns>
        CameraConfiguration GetConfiguration(int index);

        /// <summary>
        /// Get or set the current <see cref="CameraConfiguration"/>. May throw if camera configurations
        /// are not supported, i.e., if <see cref="GetConfiguration(int)"/> returns zero, or if
        /// you attempt to set an unsupported configuration.
        /// </summary>
        /// <returns>The currently active <see cref="CameraConfiguration"/>.</returns>
        CameraConfiguration currentConfiguration { get; set; }
    }
}
