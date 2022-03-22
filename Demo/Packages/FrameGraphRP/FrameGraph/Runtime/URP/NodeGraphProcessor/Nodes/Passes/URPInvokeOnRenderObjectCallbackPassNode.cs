using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/InvokeOnRenderObjectCallbackPass")]
    public class URPInvokeOnRenderObjectCallbackPassNode : BaseColorDepthPassNode
    {
        public override string name => "URPInvokeOnRenderObjectCallbackPass";

        public override string scriptableRenderPass => "FrameGraph.InvokeOnRenderObjectCallbackPassWrapper";
    }
}