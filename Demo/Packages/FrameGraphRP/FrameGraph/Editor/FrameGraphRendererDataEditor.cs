using System;
using UnityEditor;
using UnityEngine;

namespace FrameGraph
{
    [CustomEditor(typeof(FrameGraphRendererData), true)]
    public class FrameGraphRendererDataEditor : Editor
    {
        private static class Styles
        {
            public static readonly GUIContent RendererTitle = new GUIContent("FrameGraph Renderer", "Custom FrameGraph Renderer for Universal RP.");
            
            public static readonly GUIContent PostProcessLabel = new GUIContent("Post Process Data", "The asset containing references to shaders and Textures that the Renderer uses for post-processing.");

            public static readonly GUIContent FilteringLabel = new GUIContent("Filtering", "Controls filter rendering settings for this renderer.");

            public static readonly GUIContent GraphAssetLabel = new GUIContent("Graph Asset", "The asset containing rendering pipeline");

            public static readonly GUIContent OpaqueMask = new GUIContent("Opaque Layer Mask", "Controls which opaque layers this renderer draws.");

            public static readonly GUIContent TransparentMask = new GUIContent("Transparent Layer Mask", "Controls which transparent layers this renderer draws.");

            public static readonly GUIContent defaultStencilStateLabel = EditorGUIUtility.TrTextContent("Default Stencil State", "Configure the stencil state for the opaque and transparent render passes.");
            
            public static readonly GUIContent shadowTransparentReceiveLabel = EditorGUIUtility.TrTextContent("Transparent Receive Shadows", "When disabled, none of the transparent objects will receive shadows.");
        }


        SerializedProperty m_GraphAssetData;
        SerializedProperty m_PostProcessData;
        SerializedProperty m_OpaqueLayerMask;
        SerializedProperty m_TransparentLayerMask;
        SerializedProperty m_DefaultStencilState;
        SerializedProperty m_ShadowTransparentReceiveProp;

        public override void OnInspectorGUI()
        {
            if (m_GraphAssetData == null) m_GraphAssetData = serializedObject.FindProperty("m_GraphAsset");
            if (m_PostProcessData == null) m_PostProcessData = serializedObject.FindProperty("postProcessData");
            if (m_OpaqueLayerMask == null) m_OpaqueLayerMask = serializedObject.FindProperty("m_OpaqueLayerMask");
            if (m_TransparentLayerMask == null) m_TransparentLayerMask = serializedObject.FindProperty("m_TransparentLayerMask");
            if (m_DefaultStencilState == null) m_DefaultStencilState = serializedObject.FindProperty("m_DefaultStencilState");
            if (m_ShadowTransparentReceiveProp == null) m_ShadowTransparentReceiveProp = serializedObject.FindProperty("m_ShadowTransparentReceive");

            serializedObject.Update();
            EditorGUILayout.Space();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.RendererTitle, EditorStyles.boldLabel); // Title
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_GraphAssetData, Styles.GraphAssetLabel);
            EditorGUILayout.PropertyField(m_PostProcessData, Styles.PostProcessLabel);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Styles.FilteringLabel, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_OpaqueLayerMask, Styles.OpaqueMask);
            EditorGUILayout.PropertyField(m_TransparentLayerMask, Styles.TransparentMask);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Shadows", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_ShadowTransparentReceiveProp, Styles.shadowTransparentReceiveLabel);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Overrides", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_DefaultStencilState, Styles.defaultStencilStateLabel, true);
            SerializedProperty overrideStencil = m_DefaultStencilState.FindPropertyRelative("overrideStencilState");
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}