# XR Face Subsystem Definition

The purpose of this `com.unity.xr.facesubsystem` package is provide a definition of the generic API for a XR Subsystem that deals with Face Tracking data.

## Installing XR Face Subsystem

This package is normally not installed by a user, but rather as a dependency defined in other packages.

## Package structure

```none
<root>
  ├── package.json
  ├── README.md
  ├── CHANGELOG.md
  ├── LICENSE.md
  ├── QAReport.md
  ├── Runtime
  │   ├── Unity.XR.FaceSubsystem.asmdef
  │   ├── XRFaceSubsystem.cs
  │   ├── FaceEventArgs.cs
  │   ├── XRFaceSubsystemDescriptors.cs
  │   ├── XRFace.cs
  │   └── XRFaceFeatureCoefficient.cs
  └── Documentation~
      └── com.unity.xr.facesubsystem.md
```

## Package usage

This package is used as a dependency for other packages in two scenarios:

1. The other package wants to use the API of the Face Subsystem that is defined here.  Packages in this category include:
[ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/)

1. The other package wants to extend the API and implement a provider that provides data to the Face Subsystem via this API.  Packages in this category include:
[ARKit XR Plugin](https://docs.unity3d.com/Packages/com.unity.xr.arkit@1.0/)

## Documentation

* [Script API](Runtime/) <update?>
* [Manual](Documentation~/com.unity.xr.facesubsystem.md)
