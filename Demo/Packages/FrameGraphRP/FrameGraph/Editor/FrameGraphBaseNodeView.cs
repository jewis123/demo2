using GraphProcessor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrameGraph
{
    [NodeCustomEditor(typeof(FrameGraphBaseNode))]
    public class FrameGraphBaseNodeView : BaseNodeView
    {
        public override void Enable()
        {
            base.Enable();
            if (nodeTarget is BaseResourceNode)
            {
                Color c = Color.gray;
                mainContainer.style.backgroundColor = new StyleColor(c);
            }
            if (nodeTarget is BasePassNode)
            {
                BasePassNode node = nodeTarget as BasePassNode;
                SetNodeActived(node.actived);
            }
        }

        public void OnFieldChange(string fieldName)
        {
            if (nodeTarget is BasePassNode)
            {
                BasePassNode node = nodeTarget as BasePassNode;
                if (fieldName.Equals("actived"))
                {
                    SetNodeActived(node.actived);
                }
            }
        }

        public void SetNodeActived(bool enabled)
        {
            Color c = enabled ? Color.clear : Color.red;
            mainContainer.style.backgroundColor = new StyleColor(c);
        }
    }
}