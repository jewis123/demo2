using System.IO;
using GraphProcessor;
using FrameGraph;
using UnityEditor;
using UnityEngine;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;

namespace FrameGraph
{
    public class FrameGraphWindow : BaseGraphWindow
    {
        [MenuItem("Assets/Create/FrameGraph/FrameGraphAsset", false, 10)]
        public static void CreateGraphAsset()
        {
            var graph = ScriptableObject.CreateInstance<FrameGraphAsset>();
            ProjectWindowUtil.CreateAsset(graph, "FrameGraphAsset.asset");
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent("FrameGraph");
            
            if (graphView == null)
            {
                graphView = new BaseGraphView(this);
                var toolbarView = new FrameGraphToolbarView(graphView);
                graphView.Add(toolbarView);
            }
            
             rootView.Add(graphView);
        }
    }
}