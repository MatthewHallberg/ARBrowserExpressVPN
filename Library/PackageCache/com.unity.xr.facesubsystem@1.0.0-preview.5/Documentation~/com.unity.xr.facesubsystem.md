# About XR Face Subsystem 

This package provides a generic API to the AR Face Tracking data in the form of an XR Subsystem.  It is used both by client frameworks like [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/) for a generic way to work with Face Tracking data, as well as by SDKs that provide AR Face Tracking data like [ARKit XR Plugin](https://docs.unity3d.com/Packages/com.unity.xr.arkit@1.0/).



## Face Subsystem API

See [XR FaceSubsystem](https://docs.unity3d.com/Packages/com.unity.xr.facesubsystem@1.0/)

## How to use this package

If you are a client application, you would initialize the **Face Subsystem** and check that the features you want are supported by the **Face Subsystem** provider that has been selected.  You will then access the generic API above to use the **Face Subsystem** features in your app.  See [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/) for an example of how to do this.

If you want to provide AR Face Tracking data to the XR Subsystem ecosystem, create a plugin package that derives from FaceSubsystem class and register your SubsystemDescriptor that describes features you support when you load up the plugin.  Afterward, whenever anyone calls one of the APIs that you have overridden, you provide the data from your SDK in the expected format.  See [ARKit XR Plugin](https://docs.unity3d.com/Packages/com.unity.xr.arkit@1.0/) for an example of this.