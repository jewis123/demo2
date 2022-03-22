using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameGraph
{
    [CustomPropertyDrawer(typeof(VolumeBlitGroupPass.Settings), true)]
    internal class VolumeBlitGroupPassSettingsEditor : PropertyDrawer
    {
        internal class Styles
        {
            public static float defaultLineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private SerializedProperty m_TagSettings;
        
        private List<SerializedObject> m_properties = new List<SerializedObject>();

        private void Init(SerializedProperty property)
        {
            m_TagSettings = property.FindPropertyRelative("tag");
            
            m_properties.Add(property.serializedObject);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(rect, label, property);
            
            if (!m_properties.Contains(property.serializedObject))
            {
                Init(property);
            }
            
            EditorGUI.PropertyField(rect, m_TagSettings);
            rect.y += Styles.defaultLineSpace;
            
            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = Styles.defaultLineSpace;

            return height;
        }
    }
}