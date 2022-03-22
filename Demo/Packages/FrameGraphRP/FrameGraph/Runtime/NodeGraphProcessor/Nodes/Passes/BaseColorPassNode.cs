using GraphProcessor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FrameGraph
{
    public abstract class BaseColorPassNode : BasePassNode
    {
        [Input("Color Attachment")] public int inputColor;

        [Parameter, HideInInspector] public ClearFlag clearFlag = ClearFlag.None;

        [Parameter, HideInInspector] public Color clearColor = new Color(0,0,0,0);

        [Output("Color Attachment", true)] public int outputColor;
        
        protected override string[] customInputFilters => new[] {nameof(inputColor)};

        protected override void Process()
        {
            base.Process();
            outputColor = inputColor;
        }
    }
}