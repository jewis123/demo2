using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/CopyDepthPass")]
    public class URPCopyDepthPassNode: BaseColorPassNode
    {
        [Input("SourceTex")]
        private int inputSourceRT;
        
        public override string name => "URPCopyDepthPass";

        public override string scriptableRenderPass => "FrameGraph.CopyDepthPassWrapper";
        
        [CustomPortBehavior(nameof(inputColor))]
        protected IEnumerable<PortData> GetInputColorPort(List<SerializableEdge> edges)
        {
            yield return NewPortData("inputColor", "Destination", false);
        }
        
        [CustomPortBehavior(nameof(outputColor))]
        protected IEnumerable<PortData> GetOutputColorPort(List<SerializableEdge> edges)
        {
            yield return NewPortData("outputColor", "Destination", true);
        }
    }
}