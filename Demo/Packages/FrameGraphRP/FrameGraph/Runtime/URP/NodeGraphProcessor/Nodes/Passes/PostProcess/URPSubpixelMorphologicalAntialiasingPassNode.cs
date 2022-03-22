using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/PostProcess/SMAAPass")]
    public class URPSubpixelMorphologicalAntialiasingPassNode : BasePostProcessPassNode
    {
        
        [Input("EdgeTexture")] public int inputEdgeTex;

        [Output("EdgeTexture", true)] public int outputEdgeTex;
        
        [Input("BlendTexture")] public int inputBlendTex;

        [Output("BlendTexture", true)] public int outputBlendTex;
        
        [Input("DepthTex")] public int inputDepthSource;
        
        protected override string[] customInputFilters => new[] {nameof(inputDestination), nameof(inputEdgeTex), nameof(inputBlendTex)};
        
        public override string name => "URPSMAAPass";
        
        public override string scriptableRenderPass => "FrameGraph.SubpixelMorphologicalAntialiasingPassWrapper";
    }
}