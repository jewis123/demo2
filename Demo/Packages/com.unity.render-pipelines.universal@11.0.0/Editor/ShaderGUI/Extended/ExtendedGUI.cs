using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEditor;

public static class ExtendedGUI
{
	static GUIStyle _HelpBoxRichTextStyle;
	public static GUIStyle HelpBoxRichTextStyle
	{
		get
		{
			if (_HelpBoxRichTextStyle == null)
			{
				_HelpBoxRichTextStyle = new GUIStyle("HelpBox");
				_HelpBoxRichTextStyle.richText = true;
				_HelpBoxRichTextStyle.margin = new RectOffset(4, 4, 0, 0);
				_HelpBoxRichTextStyle.padding = new RectOffset(4, 4, 4, 4);
			}
			return _HelpBoxRichTextStyle;
		}
	}
}

public class GradientDrawer : MaterialPropertyDrawer
{
	static Texture2D DefaultRampTexture;
	static bool DefaultTextureSearched; //Avoid searching each update if texture isn't found

	private static GUIContent editButtonLabel = new GUIContent("Edit Gradient", "Edit the ramp texture using Unity's gradient editor");
	private static GUIContent editButtonDisabledLabel = new GUIContent("Edit Gradient", "Can't edit the ramp texture because it hasn't been generated with the Ramp Generator\n\n(Tools/Toony Colors Pro 2/Ramp Generator)");

	private AssetImporter assetImporter;

	public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{
		float indent = EditorGUI.indentLevel * 15;

		//Label
		var labelRect = position;
		labelRect.height = EditorGUIUtility.singleLineHeight;
		var space = labelRect.height + 4;
		position.y += space - 3;
		position.height -= space;
		EditorGUI.PrefixLabel(labelRect, new GUIContent(label));

		//Texture object field
		position.height = EditorGUIUtility.singleLineHeight;
		var newTexture = (Texture)EditorGUI.ObjectField(position, prop.textureValue, typeof(Texture2D), false);
		if (newTexture != prop.textureValue)
		{
			prop.textureValue = newTexture;
			assetImporter = null;
		}

		//Preview texture override (larger preview, hides texture name)
		var previewRect = new Rect(position.x + indent, position.y + 1, position.width - indent - 19, position.height - 2);
		if (prop.hasMixedValue)
		{
			var col = GUI.color;
			GUI.color = EditorGUIUtility.isProSkin ? new Color(.25f, .25f, .25f) : new Color(.85f, .85f, .85f);
			EditorGUI.DrawPreviewTexture(previewRect, Texture2D.whiteTexture);
			GUI.color = col;
			GUI.Label(previewRect, "―");
		}
		else if (prop.textureValue != null)
			EditorGUI.DrawPreviewTexture(previewRect, prop.textureValue);

		if (prop.textureValue != null)
		{
			assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(prop.textureValue));
		}

		//Edit button
		var buttonRect = labelRect;
		buttonRect.xMin += buttonRect.width - 200;
		buttonRect.width /= 2;
		if (GUI.Button(buttonRect, "Create New", EditorStyles.miniButtonLeft))
		{
			var lastSavePath = GradientManager.LAST_SAVE_PATH;
			if (!lastSavePath.Contains(Application.dataPath))
				lastSavePath = Application.dataPath;

			var path = EditorUtility.SaveFilePanel("Create New Ramp Texture", lastSavePath, "TCP2_CustomRamp", "png");
			if (!string.IsNullOrEmpty(path))
			{
				bool overwriteExistingFile = File.Exists(path);

				GradientManager.LAST_SAVE_PATH = Path.GetDirectoryName(path);

				//Create texture and save PNG
				var projectPath = path.Replace(Application.dataPath, "Assets");
				GradientManager.CreateAndSaveNewGradientTexture(256, projectPath);

				//Load created texture
				var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(projectPath);
				assetImporter = AssetImporter.GetAtPath(projectPath);

				//Assign to material(s)
				prop.textureValue = texture;

				//Open for editing
				TCP2_RampGenerator.OpenForEditing(texture, editor.targets, true, !overwriteExistingFile);
			}
		}
		buttonRect.x += buttonRect.width;
		var enabled = GUI.enabled;
		GUI.enabled = (assetImporter != null) && (assetImporter.userData.StartsWith("GRADIENT") || assetImporter.userData.StartsWith("gradient:")) && !prop.hasMixedValue;
		if (GUI.Button(buttonRect, GUI.enabled ? editButtonLabel : editButtonDisabledLabel, EditorStyles.miniButtonRight))
		{
			TCP2_RampGenerator.OpenForEditing((Texture2D)prop.textureValue, editor.targets, true, false);
		}
		GUI.enabled = enabled;
	}

	public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
	{
		return EditorGUIUtility.singleLineHeight * 2.0f + EditorGUIUtility.standardVerticalSpacing;
	}
}


