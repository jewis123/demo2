using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("RenderTarget/URP/GradingLut")]
    public class URPGradingLutNode : BaseResourceNode
    {
        public override string name => "_InternalGradingLut";
    }
}