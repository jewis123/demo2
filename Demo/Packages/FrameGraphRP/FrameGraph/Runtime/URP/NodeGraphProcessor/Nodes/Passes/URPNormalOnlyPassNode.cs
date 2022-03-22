using System.Collections.Generic;
using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/DepthNormalOnlyPass")]
    public class URPNormalOnlyPassNode : BaseColorDepthPassNode
    {
        public override string name => "URPDepthNormalOnlyPass";

        public override string scriptableRenderPass => "FrameGraph.DepthNormalOnlyPassWrapper";
        
        [CustomPortBehavior(nameof(inputColor))]
        protected IEnumerable<PortData> GetInputColorPort(List<SerializableEdge> edges)
        {
            yield return NewPortData("inputColor", "NormalTex", false);
        }
        
        [CustomPortBehavior(nameof(outputColor))]
        protected IEnumerable<PortData> GetOutputColorPort(List<SerializableEdge> edges)
        {
            yield return NewPortData("outputColor", "NormalTex", true);
        }
    }
}