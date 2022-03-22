using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/RenderOpaqueForwardPass")]
    public class URPRenderOpaqueForwardPassNode : BaseColorDepthPassNode
    {
        [Input("ShadowMapTex")] public int inputShadowMap;
        
        public override string name => "URPRenderOpaqueForwardPass";

        public override string scriptableRenderPass => "FrameGraph.RenderOpaqueForwardPassWrapper";
    }
}