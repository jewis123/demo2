using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
	internal class ToonParamGUI
	{
		public static class Styles
		{
			public static readonly GUIContent toonParamText = new GUIContent("Toon Param",
				"These settings let you add Toon Param to the surface.");
		}

		public struct ToonProperties
		{
			public MaterialProperty rampTexture;
			public MaterialProperty rampScale;
			public MaterialProperty rampOffset;
			public MaterialProperty rampThreshold;

			public ToonProperties(MaterialProperty[] properties)
			{
				rampTexture = BaseShaderGUI.FindProperty("_Ramp", properties, false);
				rampScale = BaseShaderGUI.FindProperty("_RampScale", properties, false);
				rampOffset = BaseShaderGUI.FindProperty("_RampOffset", properties, false);
				rampThreshold = BaseShaderGUI.FindProperty("_RampThreshold", properties, false);
			}
		}

		public static void DoToonParamArea(ToonProperties properties, MaterialEditor materialEditor)
		{
			DisplayProperty(properties.rampTexture, "RampTexture", materialEditor);
			DisplayProperty(properties.rampScale, "RampScale", materialEditor);
			DisplayProperty(properties.rampOffset, "RampOffset", materialEditor);
			DisplayProperty(properties.rampThreshold, "RampThreshold", materialEditor);

		}

		public static void SetMaterialKeywords(Material material)
		{
			// if (material.HasProperty("_DetailAlbedoMap") && material.HasProperty("_DetailNormalMap") && material.HasProperty("_DetailAlbedoMapScale"))
			// {
			// 	bool isScaled = material.GetFloat("_DetailAlbedoMapScale") != 1.0f;
			// 	bool hasDetailMap = material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap");
			// 	CoreUtils.SetKeyword(material, "_DETAIL_MULX2", !isScaled && hasDetailMap);
			// 	CoreUtils.SetKeyword(material, "_DETAIL_SCALED", isScaled && hasDetailMap);
			// }
		}
		
		protected static void DisplayProperty(MaterialProperty property, string label, MaterialEditor materialEditor)
		{
			float propertyHeight = materialEditor.GetPropertyHeight(property, label);
			Rect controlRect = EditorGUILayout.GetControlRect(true, propertyHeight, EditorStyles.layerMaskField, new GUILayoutOption[0]);
			materialEditor.ShaderProperty(controlRect, property, label);
		}
	}
}
