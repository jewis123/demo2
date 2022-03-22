using System.Collections.Generic;
using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/PostProcess/DepthOfFieldPass")]
    public class URPDepthOfFieldPassNode : BasePostProcessPassNode
    {
        [Input("FullCoCTex")] public int inputFullCoCTexture;

        [Output("FullCoCTex", true)] public int outputFullCoCTexture;
        
        [Input("HalfCoCTex")] public int inputHalfCoCTexture;

        [Output("HalfCoCTex", true)] public int outputHalfCoCTexture;
        
        [Input("PingTex")] public int inputPingTexture;

        [Output("PingTex", true)] public int outputPingTexture;
        
        [Input("PongTex")] public int inputPongTexture;

        [Output("PongTex", true)] public int outputPongTexture;
        
        [Input("DepthTex")] public int inputDepthSource;

        protected override string[] customInputFilters => new[] {nameof(inputDestination), nameof(inputFullCoCTexture), nameof(inputHalfCoCTexture), nameof(inputPingTexture), nameof(inputPongTexture)};

        protected override void Process()
        {
            base.Process();
            outputFullCoCTexture = inputFullCoCTexture;
            outputHalfCoCTexture = inputHalfCoCTexture;
            outputPingTexture = inputPingTexture;
            outputPongTexture = inputPongTexture;
        }
        
        public override string name => "URPDepthOfFieldPass";
        
        public override string scriptableRenderPass => "FrameGraph.DepthOfFieldPassWrapper";
        
    }
}