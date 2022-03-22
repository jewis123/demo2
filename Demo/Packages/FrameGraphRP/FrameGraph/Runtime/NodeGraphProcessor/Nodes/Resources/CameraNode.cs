using GraphProcessor;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("RenderTarget/Camera")]
    public class CameraNode : BaseResourceNode
    {
        public override string name => "Camera";
    }
}