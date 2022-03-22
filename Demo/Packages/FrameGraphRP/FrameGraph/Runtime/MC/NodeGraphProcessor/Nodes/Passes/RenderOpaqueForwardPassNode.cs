using FrameGraph;
using GraphProcessor;

namespace FrameGraph.MC
{
    [System.Serializable, NodeMenuItem("Passes/MC/RenderOpaqueForwardPass")]
    public class RenderOpaqueForwardPassNode : URPRenderOpaqueForwardPassNode
    {
        [Input("HairShadowMaskTex")] public int inputHairShadowMask;
        
        [Input("CharacterDepthTex")] public int inputCharacterDepthTex;
        
        public override string name => "RenderOpaqueForwardPass";

        public override string scriptableRenderPass => "FrameGraph.MC.RenderOpaqueForwardPass";
    }
}