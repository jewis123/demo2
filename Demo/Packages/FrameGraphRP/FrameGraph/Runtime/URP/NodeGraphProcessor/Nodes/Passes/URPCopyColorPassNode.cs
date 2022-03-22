using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/CopyColorPass")]
    public class URPCopyColorPassNode : BaseColorPassNode
    {
        [Input("SourceTex")]
        private int inputSourceRT;
        
        [SerializeField, HideInInspector, Parameter, Context]
        private CopyColorPassWrapper.Settings settings;
        
        public override string name => "URPCopyColorPass";

        public override string scriptableRenderPass => "FrameGraph.CopyColorPassWrapper";
        
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