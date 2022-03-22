using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/DrawSkyboxPass")]
    public class URPDrawSkyboxNode : BaseColorDepthPassNode
    {
        public override string name => "URPDrawSkyboxPass";

        public override string scriptableRenderPass => "FrameGraph.DrawSkyboxPassWrapper";
    }
}