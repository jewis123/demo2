using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace FrameGraph
{
    public class DepthNormalOnlyPassWrapper : DepthNormalOnlyPass, IFrameGraphRenderPass
    {
        public DepthNormalOnlyPassWrapper(URPContext context) : base(FrameGraphConstant.defaultRenderPassEvent,
            RenderQueueRange.opaque, context.data.opaqueLayerMask)
        {
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            var pi = FrameGraphConfig.GetPass(pass);
            var outputs = pi.outputs;
            RenderTargetHandle rt1 = outputs[0].ToRenderTargetHandle();
            RenderTargetHandle rt2 = outputs[1].ToRenderTargetHandle();

            Setup(renderingData.cameraData.cameraTargetDescriptor, rt1, rt2);
            return true;
        }

        public ScriptableRenderPass GetScriptableRenderPass()
        {
            return this;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }
        
        public void ClearUp()
        {
            
        }

        public void ConfigureBeforeEnqueued()
        {
            
        }
    }
}