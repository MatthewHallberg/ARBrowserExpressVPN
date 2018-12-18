#if UNITY_ANDROID
#if UNITY_2018_1
using System.Reflection;
#endif
using System.Xml;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.ARExtensions;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.XR.ARCore
{
    internal class ARCorePreprocessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
                return;

            EnsureMultithreadedRenderingDisabled(report);
            EnsureARCoreSupportedIsNotChecked();
            EnsureGoogleARCoreIsNotPresent();
            EnsureMinSdkVersion();
            EnsureOnlyOpenGLES3IsUsed();
            EnsureGradleIsUsed();

            if (LinkerUtility.AssemblyStrippingEnabled(report.summary.platformGroup))
                LinkerUtility.EnsureLinkXmlExistsFor("ARCore");
        }

        void EnsureGradleIsUsed()
        {
            if (EditorUserBuildSettings.androidBuildSystem != AndroidBuildSystem.Gradle)
                throw new BuildFailedException("ARCore XR Plugin requires the Gradle build system. See File > Build Settings... > Android");
        }

        void EnsureMinSdkVersion()
        {
            var arcoreSettings = ARCoreSettings.GetOrCreateSettings();
            int minSdkVersion;
            if (arcoreSettings.ARCoreRequirement == ARCoreSettings.Requirement.Optional)
                minSdkVersion = 14;
            else
                minSdkVersion = 24;

            if ((int)PlayerSettings.Android.minSdkVersion < minSdkVersion)
                throw new BuildFailedException(string.Format("ARCore {0} apps require a minimum SDK version of {1}. Currently set to {2}",
                    arcoreSettings.ARCoreRequirement, minSdkVersion, PlayerSettings.Android.minSdkVersion));
        }

        void EnsureARCoreSupportedIsNotChecked()
        {
            if (PlayerSettings.Android.ARCoreEnabled)
                throw new BuildFailedException("\"ARCore Supported\" (Player Settings > XR Settings) refers to the built-in ARCore support in Unity and conflicts with the \"ARCore XR Plugin\" package.");
        }

        void EnsureGoogleARCoreIsNotPresent()
        {
            var googleARAssetPath = AssetDatabase.GUIDToAssetPath("afb3e05691ff94d2cbad20643e5c5879");
            if (!string.IsNullOrEmpty(googleARAssetPath))
                throw new BuildFailedException("GoogleARCore detected. Google's \"ARCore SDK for Unity\" and Unity's \"ARCore XR Plugin\" package cannot be used together.");
        }

        void EnsureMultithreadedRenderingDisabled(BuildReport report)
        {
            var multithreadedRenderingEnabled =
                PlayerSettings.GetMobileMTRendering(report.summary.platformGroup) ||
                PlayerSettings.graphicsJobs;

            if (multithreadedRenderingEnabled)
                throw new BuildFailedException("Multithreaded Rendering and Graphics Jobs (Player Settings > Other Settings) are not supported for ARCore.");
        }

        void EnsureOnlyOpenGLES3IsUsed()
        {
            var graphicsApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
            if (graphicsApis.Length > 0)
            {
                var graphicsApi = graphicsApis[0];
                if (graphicsApi != GraphicsDeviceType.OpenGLES3)
                    throw new BuildFailedException(
                        string.Format("You have enabled the {0} graphics API, which is not supported by ARCore.", graphicsApi));
            }
        }
    }

    internal class ARCoreManifest : IPostGenerateGradleAndroidProject
    {
        static readonly string k_AndroidURI = "http://schemas.android.com/apk/res/android";

        static readonly string k_AndroidNameValue = "com.google.ar.core";

        static readonly string k_AndroidManifestPath = "/src/main/AndroidManifest.xml";

        static readonly string k_AndroidHardwareCameraAr = "android.hardware.camera.ar";

        static readonly string k_AndroidPermissionCamera = "android.permission.CAMERA";

        XmlNode FindFirstChild(XmlNode node, string tag)
        {
            if (node.HasChildNodes)
            {
                for (int i = 0; i < node.ChildNodes.Count; ++i)
                {
                    var child = node.ChildNodes[i];
                    if (child.Name == tag)
                        return child;
                }
            }

            return null;
        }

        void AppendNewAttribute(XmlDocument doc, XmlElement element, string attributeName, string attributeValue)
        {
            var attribute = doc.CreateAttribute(attributeName, k_AndroidURI);
            attribute.Value = attributeValue;
            element.Attributes.Append(attribute);
        }

        void FindOrCreateTagWithAttribute(XmlDocument doc, XmlNode containingNode, string tagName,
            string attributeName, string attributeValue)
        {
            if (containingNode.HasChildNodes)
            {
                for (int i = 0; i < containingNode.ChildNodes.Count; ++i)
                {
                    var child = containingNode.ChildNodes[i];
                    if (child.Name == tagName)
                    {
                        var childElement = child as XmlElement;
                        if (childElement != null && childElement.HasAttributes)
                        {
                            var attribute = childElement.GetAttributeNode(attributeName, k_AndroidURI);
                            if (attribute != null && attribute.Value == attributeValue)
                                return;
                        }
                    }
                }
            }

            // Didn't find it, so create it
            var element = doc.CreateElement(tagName);
            AppendNewAttribute(doc, element, attributeName, attributeValue);
            containingNode.AppendChild(element);
        }

        void FindOrCreateTagWithAttributes(XmlDocument doc, XmlNode containingNode, string tagName,
            string firstAttributeName, string firstAttributeValue, string secondAttributeName, string secondAttributeValue)
        {
            if (containingNode.HasChildNodes)
            {
                for (int i = 0; i < containingNode.ChildNodes.Count; ++i)
                {
                    var childNode = containingNode.ChildNodes[i];
                    if (childNode.Name == tagName)
                    {
                        var childElement = childNode as XmlElement;
                        if (childElement != null && childElement.HasAttributes)
                        {
                            var firstAttribute = childElement.GetAttributeNode(firstAttributeName, k_AndroidURI);
                            if (firstAttribute == null || firstAttribute.Value != firstAttributeValue)
                                continue;

                            var secondAttribute = childElement.GetAttributeNode(secondAttributeName, k_AndroidURI);
                            if (secondAttribute != null)
                            {
                                secondAttribute.Value = secondAttributeValue;
                                return;
                            }

                            // Create it
                            AppendNewAttribute(doc, childElement, secondAttributeName, secondAttributeValue);
                            return;
                        }
                    }
                }
            }

            // Didn't find it, so create it
            var element = doc.CreateElement(tagName);
            AppendNewAttribute(doc, element, firstAttributeName, firstAttributeValue);
            AppendNewAttribute(doc, element, secondAttributeName, secondAttributeValue);
            containingNode.AppendChild(element);
        }

        // This ensures the Android Manifest corresponds to
        // https://developers.google.com/ar/develop/java/enable-arcore
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string manifestPath = path + k_AndroidManifestPath;
            var manifestDoc = new XmlDocument();
            manifestDoc.Load(manifestPath);

            var manifestNode = FindFirstChild(manifestDoc, "manifest");
            if (manifestNode == null)
                return;

            var applicationNode = FindFirstChild(manifestNode, "application");
            if (applicationNode == null)
                return;

            FindOrCreateTagWithAttribute(manifestDoc, manifestNode, "uses-permission", "name", k_AndroidPermissionCamera);
            FindOrCreateTagWithAttributes(manifestDoc, applicationNode, "meta-data", "name", "unityplayer.SkipPermissionsDialog", "value", "true");

            var settings = ARCoreSettings.GetOrCreateSettings();
            if (settings.ARCoreRequirement == ARCoreSettings.Requirement.Optional)
            {
                FindOrCreateTagWithAttributes(manifestDoc, applicationNode, "meta-data", "name", k_AndroidNameValue, "value", "optional");
            }
            else if (settings.ARCoreRequirement == ARCoreSettings.Requirement.Required)
            {
                FindOrCreateTagWithAttributes(manifestDoc, manifestNode, "uses-feature", "name", k_AndroidHardwareCameraAr, "required", "true");
                FindOrCreateTagWithAttributes(manifestDoc, applicationNode, "meta-data", "name", k_AndroidNameValue, "value", "required");
            }

            manifestDoc.Save(manifestPath);
        }

        void DebugPrint(XmlDocument doc)
        {
            var sw = new System.IO.StringWriter();
            var xw = XmlWriter.Create(sw);
            doc.Save(xw);
            Debug.Log(sw);
        }

        public int callbackOrder { get { return 2; } }
    }
}
#endif
