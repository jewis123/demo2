using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using FrameGraph;
using UnityEngine.UIElements;

namespace FrameGraph
{
	[CustomEditor(typeof(FrameGraphAsset), true)]
	public class FrameGraphAssetInspector : GraphInspector
	{

		protected override void CreateInspector()
		{
			base.CreateInspector();

			root.Add(new Button(() => EditorWindow.GetWindow<FrameGraphWindow>().InitializeGraph(target as FrameGraphAsset))
			{
				text = "Open graph window"
			});
		}
	}
}