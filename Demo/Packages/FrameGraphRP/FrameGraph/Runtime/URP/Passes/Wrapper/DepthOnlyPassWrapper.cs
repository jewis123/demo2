using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace FrameGraph
{
    public class DepthOnlyPassWrapper : DepthOnlyPass, IFrameGraphRenderPass
    {
        public DepthOnlyPassWrapper(URPContext context) : base(FrameGraphConstant.defaultRenderPassEvent,
            RenderQueueRange.opaque, context.data.opaqueLayerMask)
        {
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            var pi = FrameGraphConfig.GetPass(pass);
            var outputs = pi.outputs;
            
            Setup(renderingData.cameraData.cameraTargetDescriptor, outputs[0].ToRenderTargetHandle());
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

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }

        public void ConfigureBeforeEnqueued()
        {
            
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
  
        }
    }
}