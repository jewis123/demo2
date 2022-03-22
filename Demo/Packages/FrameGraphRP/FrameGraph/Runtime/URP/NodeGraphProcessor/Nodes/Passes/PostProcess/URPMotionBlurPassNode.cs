using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/PostProcess/MotionBlurPass")]
    public class URPMotionBlurPassNode : BasePostProcessPassNode
    {
        [Input("DepthTex")] public int inputDepthSource;
        
        public override string name => "URPMotionBlurPass";
        
        public override string scriptableRenderPass => "FrameGraph.MotionBlurPassWrapper";
    }
}