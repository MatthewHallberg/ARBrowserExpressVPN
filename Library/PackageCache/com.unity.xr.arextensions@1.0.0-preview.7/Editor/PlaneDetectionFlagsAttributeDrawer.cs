using UnityEngine;
using UnityEngine.XR.ARExtensions;

namespace UnityEditor.XR.ARExtensions
{
    /// <summary>
    /// A custom property drawer for the <see cref="PlaneDetectionFlags"/> enum.
    /// </summary>
    [CustomPropertyDrawer(typeof(PlaneDetectionFlagsMaskAttribute))]
    class PlaneDetectionFlagsMaskAttributeDrawer : PropertyDrawer
    {
        string[] m_EnumNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Remove the "None" option
            if (m_EnumNames == null)
            {
                m_EnumNames = new string[property.enumNames.Length - 1];
                for (int i = 1; i < property.enumNames.Length; ++i)
                    m_EnumNames[i - 1] = property.enumNames[i];
            }

            property.intValue = EditorGUI.MaskField(position, label, property.intValue, m_EnumNames);
        }
    }
}