using System.Collections.Generic;
using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/DepthOnlyPass")]
    public class URPDepthOnlyPassNode : BaseColorPassNode
    {
        public override string name => "URPDepthOnlyPass";

        public override string scriptableRenderPass => "FrameGraph.DepthOnlyPassWrapper";
        
        [CustomPortBehavior(nameof(inputColor))]
        protected IEnumerable<PortData> GetInputColorPort(List<SerializableEdge> edges)
        {
            yield return NewPortData("inputColor", "Depth Attachment", false);
        }
        
        [CustomPortBehavior(nameof(outputColor))]
        protected IEnumerable<PortData> GetOutputColorPort(List<SerializableEdge> edges)
        {
            yield return NewPortData("outputColor", "Depth Attachment", true);
        }
    }
}