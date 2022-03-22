using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameGraph
{
    [CustomPropertyDrawer(typeof(BasePostProcessPass.Settings), true)]
    internal class PostProcessSettingsEditor : PropertyDrawer
    {
        internal class Styles
        {
            public static float defaultLineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private SerializedProperty m_SwapSettings;
        
        private List<SerializedObject> m_properties = new List<SerializedObject>();

        private void Init(SerializedProperty property)
        {
            m_SwapSettings = property.FindPropertyRelative("swap");
            
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
            
            EditorGUI.PropertyField(rect, m_SwapSettings);
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