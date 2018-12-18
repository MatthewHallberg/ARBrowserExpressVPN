using UnityEngine;

namespace UnityEditor.XR.ARCore
{
    /// <summary>
    /// Holds settings that are used to configure the Unity ARCore Plugin.
    /// </summary>
    public class ARCoreSettings : ScriptableObject
    {
        /// <summary>
        /// Enum which defines whether ARCore is optional or required.
        /// </summary>
        public enum Requirement
        {
            /// <summary>
            /// ARCore is required, which means the app cannot be installed on devices that do not support ARCore.
            /// </summary>
            Required,

            /// <summary>
            /// ARCore is optional, which means the the app can be installed on devices that do not support ARCore.
            /// </summary>
            Optional
        }

        [SerializeField, Tooltip("Toggles whether ARCore is required for this app. Will make app only downloadable by devices with ARCore support if set to 'Required'.")]
        Requirement m_ARCoreRequirement;

        /// <summary>
        /// Determines whether ARCore is required for this app: will make app only downloadable by devices with ARCore support if set to <see cref="Requirement.Required"/>.
        /// </summary>
        public Requirement ARCoreRequirement
        {
            get { return m_ARCoreRequirement; }
            set { m_ARCoreRequirement = value; }
        }

        /// <summary>
        /// Gets the currently selected settings, or create a default one if no <see cref="ARCoreSettings"/> has been set in Player Settings.
        /// </summary>
        /// <returns>The ARCore settings to use for the current Player build.</returns>
        public static ARCoreSettings GetOrCreateSettings()
        {
            var settings = currentSettings;
            if (settings != null)
                return settings;

            return CreateInstance<ARCoreSettings>();
        }

        /// <summary>
        /// Get or set the <see cref="ARCoreSettings"/> that will be used for the player build.
        /// </summary>
        public static ARCoreSettings currentSettings
        {
            get
            {
                ARCoreSettings settings = null;
                EditorBuildSettings.TryGetConfigObject(k_ConfigObjectName, out settings);
                return settings;
            }

            set
            {
                if (value == null)
                {
                    EditorBuildSettings.RemoveConfigObject(k_ConfigObjectName);
                }
                else
                {
                    EditorBuildSettings.AddConfigObject(k_ConfigObjectName, value, true);
                }
            }
        }

        internal static bool TrySelect()
        {
            var settings = currentSettings;
            if (settings == null)
                return false;

            Selection.activeObject = settings;
            return true;
        }

        static readonly string k_ConfigObjectName = "com.unity.xr.arcore.PlayerSettings";
    }

    internal class SettingsSelectionWindow : EditorWindow
    {
        [MenuItem("Edit/Project Settings/ARCore")]
        static void ShowSelectionWindow()
        {
            ARCoreSettings.TrySelect();
            Rect rect = new Rect(500, 300, 400, 150);
            var window = GetWindowWithRect<SettingsSelectionWindow>(rect);
            window.titleContent = new GUIContent("ARCore");
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Space(5);
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.wordWrap = true;
            EditorGUILayout.LabelField("Select an existing ARCoreSettings object or create a new one.", titleStyle);

            EditorGUI.BeginChangeCheck();
            ARCoreSettings.currentSettings =
                EditorGUILayout.ObjectField("ARCoreSettings", ARCoreSettings.currentSettings, typeof(ARCoreSettings), false) as ARCoreSettings;
            if (EditorGUI.EndChangeCheck())
                ARCoreSettings.TrySelect();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create New"))
                Create();

            if (GUILayout.Button("Close"))
                Close();

            EditorGUILayout.EndHorizontal();
        }

        void Create()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save ARCore Settings", "ARCoreSettings", "asset", "Please enter a filename to save the ARCore settings.");
            if (string.IsNullOrEmpty(path))
                return;

            var settings = CreateInstance<ARCoreSettings>();
            AssetDatabase.CreateAsset(settings, path);
            ARCoreSettings.currentSettings = settings;
        }
    }
}
