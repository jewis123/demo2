using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/RenderTransparentForwardPass")]
    public class URPRenderTransparentForwardPassNode : BaseColorDepthPassNode
    {
        [Input("ShadowMapTex")] public int inputShadowMap;
        
        public override string name => "URPRenderTransparentForwardPass";

        public override string scriptableRenderPass => "FrameGraph.RenderTransparentForwardPassWrapper";
    }
}