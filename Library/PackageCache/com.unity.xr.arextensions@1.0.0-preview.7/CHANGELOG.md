# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0-preview.7] - 2018-12-13
### Improvements
- Add support for x86 and ARM64 Android architectures.
- Plane detection modes: Add ability to selectively enable detection for horizontal, vertical, or both types of planes.
- Add support for setting the camera focus mode.
- Add support for enumerating and setting a camera image configuration, which affects the resolution and possibly framerate of the hardware camera used for AR.
- Add Android x86_64 support.

### Fixes
- Remove use of deprecated `PlayerSettings.strippingLevel` API which threw a warning in 2018.3.

## [1.0.0-preview.6] - 2018-11-06
### Fixes
- Fix crash when repeatedly enabling and disabling AR

### Improvements
- Add native pointer support in several subsystems.
- Switch default session availability check to return `SessionAvailability.None` instead of supported. This was previously there for backwards compatibility before all platforms implemented this method. Now it can cause confusion if the platform is not correctly installed.
- Add IL2CPP linker validation utility for use in platform-specific packages to prevent runtime assembly stripping.

## [1.0.0-preview.5] - 2018-10-10
- Added new API to `XRCameraExtensions` to access the raw camera image data on the CPU. See the [ARFoundation manual documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/cpu-camera-image.html) for more information.

## [1.0.0-preview.3] - 2018-06-11
- Add extensions to the XRReferencePointSubsystem
    - Add support for "attaching" reference points to other trackables, like planes.

## [1.0.0-preview.2] - 2018-05-23
Move WIP face subsystem to ignored folder

## [1.0.0-preview.1] - 2018-05-23
Published to staging