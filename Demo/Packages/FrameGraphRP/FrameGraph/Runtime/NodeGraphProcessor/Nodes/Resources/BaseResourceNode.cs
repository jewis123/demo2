using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    public class BaseResourceNode : FrameGraphBaseNode
    {
        [Output("RenderTarget", false)] public int outputRT;

        public override Color color { get; }
    }
}