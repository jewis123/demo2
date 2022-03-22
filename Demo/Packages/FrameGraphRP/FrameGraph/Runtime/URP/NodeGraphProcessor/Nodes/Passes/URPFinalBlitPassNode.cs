using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/FinalBlitPass")]
    public class URPFinalBlitPassNode : BaseColorPassNode
    {
        [Input("SourceTex")] public int inputSourceTex;
        
        public override string name => "URPFinalBlitPass";

        public override string scriptableRenderPass => "FrameGraph.FinalBlitPassWrapper";
        
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