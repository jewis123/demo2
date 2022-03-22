using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/PostProcess/FXAAPass")]
    public class URPFastApproximateAntialiasingPassNode : BasePostProcessPassNode
    {
        public override string name => "URPFXAAPass";
        
        public override string scriptableRenderPass => "FrameGraph.FastApproximateAntialiasingPassWrapper";
    }
}