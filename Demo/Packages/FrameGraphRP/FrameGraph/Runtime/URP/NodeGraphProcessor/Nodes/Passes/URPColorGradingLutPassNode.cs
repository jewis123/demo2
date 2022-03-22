using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/ColorGradingLutPass")]
    public class URPColorGradingLutPassNode : BaseColorPassNode
    {
        public override string name => "URPColorGradingLutPass";
        
        public override string scriptableRenderPass => "FrameGraph.ColorGradingLutPassWrapper";
        
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