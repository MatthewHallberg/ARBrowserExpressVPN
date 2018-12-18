using System.IO;
using System.Xml;
using UnityEditor.Build;
using UnityEngine;

namespace UnityEditor.XR.ARExtensions
{
    /// <summary>
    /// This utility is used by some packages to create or modify a link.xml file in the user's project.
    /// This is necessary when using the IL2CPP scripting backend to preserve some packages' runtime assemblies.
    /// 
    /// See <a href="https://docs.unity3d.com/Manual/IL2CPP-BytecodeStripping.html">Managed bytecode stripping with IL2CPP</a>
    /// for more information.
    /// </summary>
    public static class LinkerUtility
    {
        /// <summary>
        /// Determines whether assembly stripping is enabled for the given <c>BuildTargetGroup</c>.
        /// If it is, then <see cref="EnsureLinkXmlExistsFor(string)"/> can be used to create and
        /// validate a special file which will prevent assembly stripping.
        /// </summary>
        public static bool AssemblyStrippingEnabled(BuildTargetGroup targetGroup)
        {
            return
                (PlayerSettings.GetScriptingBackend(targetGroup) == ScriptingImplementation.IL2CPP) ||
#if UNITY_2018_3_OR_NEWER
                (PlayerSettings.GetManagedStrippingLevel(targetGroup) != ManagedStrippingLevel.Disabled);
#else
                (PlayerSettings.strippingLevel != StrippingLevel.Disabled);
#endif
        }

        /// <summary>
        /// Checks for the existence of a link.xml file in the user's <c>Assets</c> folder,
        /// and validates it if it exists. If a correction is necessary (i.e., the file is missing
        /// or incorrect), it prompts the user to fix the problem, cancel the build, or take no action.
        /// 
        /// This utility is intended to be used by platform-specific packages during a player build.
        /// </summary>
        /// <param name="packageName">The name of the package with which to validate the link.xml</param>
        public static void EnsureLinkXmlExistsFor(string packageName)
        {
            var assemblyName = GetAssemblyName(packageName);

            var requiredAction = RequiredAction.CreateXml;
            XmlDocument document = null;
            if (File.Exists(k_Path))
            {
                document = new XmlDocument();
                document.Load(k_Path);
                var nav = document.CreateNavigator();

                if (nav.SelectSingleNode(string.Format("/linker/assembly[@fullname='{0}']", assemblyName)) != null)
                {
                    // Everything is setup correctly
                    requiredAction = RequiredAction.None;
                }
                else if (nav.SelectSingleNode("/linker") == null)
                {
                    // The XML doc exists, but <linker> is not the root node.
                    requiredAction = RequiredAction.ReplaceXml;
                }
                else
                {
                    // <linker> exists, but our assembly isn't named.
                    requiredAction = RequiredAction.AddToXml;
                }
            }

            if (requiredAction == RequiredAction.None)
                return;

            var dialogMsg = "";
            switch (requiredAction)
            {
                case RequiredAction.CreateXml:
                    dialogMsg = string.Format(
                        "{0} will not work properly because your project does not contain a link.xml in the root Assets directory. Would you like to create it now?",
                        packageName);
                    break;

                case RequiredAction.AddToXml:
                    dialogMsg = string.Format(
                        "{1} will not work properly because your project's link.xml does not name the {0} assembly. Would you like to add it now?",
                        assemblyName,
                        packageName);
                    break;

                case RequiredAction.ReplaceXml:
                    dialogMsg = string.Format(
                        "{1} will not work properly because your project's link.xml file does not contain linker information.\n\nWould you like to REPLACE the contents of your EXISTING link.xml file with a linker command which will preserve {0}? This will fix the issue, but will replace the contents of the existing link.xml. This cannot be undone.",
                        assemblyName,
                        packageName);
                    break;
            }

            var choice = EditorUtility.DisplayDialogComplex(
                string.Format("{0} will be stripped", assemblyName),
                dialogMsg,
                "Yes, fix and build",
                "No, keep building",
                "Cancel build");
            
            switch (choice)
            {
                case 0:
                    if (document == null)
                        Create(assemblyName);
                    else
                        AddAssembly(document, assemblyName);

                    AssetDatabase.Refresh();
                    break;
                case 1:
                    Debug.LogWarningFormat("{0} will not work properly until the link.xml is fixed.", packageName);
                    break;
                case 2:
                    throw new BuildFailedException("You canceled the build.");
            }
        }

        enum RequiredAction
        {
            None,
            CreateXml,
            AddToXml,
            ReplaceXml
        }

        static LinkerUtility()
        {
            k_Path = Path.Combine("Assets", "link.xml");
        }

        static string GetAssemblyName(string packageName)
        {
            return "Unity.XR." + packageName;
        }

        static string GetAssemblyXml(string assemblyName)
        {
            return string.Format(
                "<assembly fullname=\"{0}\" preserve=\"none\" ignoreIfMissing=\"1\"/>",
                assemblyName);
        }

        static void AddAssembly(XmlDocument document, string assemblyName)
        {
            var nav = document.CreateNavigator();
            var iter = nav.SelectSingleNode("/linker");
            if (iter == null)
            {
                Create(assemblyName);
                return;
            }

            var assemblyXml = GetAssemblyXml(assemblyName);
            iter.MoveToFirstChild();
            iter.InsertBefore(assemblyXml);

            document.Save(k_Path);
            Debug.LogFormat("Added \"{0}\" to Assets/link.xml", assemblyXml);
        }

        static void Create(string assemblyName)
        {
            var contents = string.Format("<linker>\n  {0}\n</linker>", GetAssemblyXml(assemblyName));
            File.WriteAllText(k_Path, contents);
            Debug.LogFormat("Created Assets/link.xml with contents:\n{0}", contents);
        }

        static readonly string k_Path;
    }
}
