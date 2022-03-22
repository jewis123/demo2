using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/PostProcess/PaniniProjectionPass")]
    public class URPPaniniProjectionPassNode : BasePostProcessPassNode
    {
        public override string name => "URPPaniniProjectionPass";
        
        public override string scriptableRenderPass => "FrameGraph.PaniniProjectionPassWrapper";
    }
}