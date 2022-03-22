using System.Collections.Generic;
using GraphProcessor;

namespace FrameGraph
{
    /// <summary>
    /// 普通摄像机或堆栈类型的最后一个摄像机能够执行的后处理节点
    /// </summary>
    [System.Serializable, NodeMenuItem("Passes/URP/PostProcess/FinalTargetPostProcessPass")]
    public class URPFinalTargetPostProcessPassNode : BaseColorPassNode
    {
        [Input("SourceTex")] public int inputColorSource;
        
        [Input("LutTex")] public int inputLut;
        
        [Input("BloomTex")] public int inputBloom;

        public override string name => "URPFinalTargetPostProcessPass";
        
        public override string scriptableRenderPass => "FrameGraph.FinalTargetPostProcessPassWrapper";
        
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