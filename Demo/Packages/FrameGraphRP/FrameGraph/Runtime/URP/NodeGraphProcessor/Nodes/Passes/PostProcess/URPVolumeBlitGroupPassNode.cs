using GraphProcessor;
using UnityEngine;

namespace FrameGraph
{
    [System.Serializable, NodeMenuItem("Passes/URP/PostProcess/BlitGroupPass")]
    public class URPVolumeBlitGroupPassNode : BasePostProcessPassNode
    {
        
        [SerializeField, HideInInspector, Parameter, Context]
        public VolumeBlitGroupPass.Settings blitGroupSettings;
        
        public override string name => "URPBlitGroupPass";
        
        public override string scriptableRenderPass => "FrameGraph.VolumeBlitGroupPass";
    }
}