using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace FrameGraph
{
    public class CopyDepthPassWrapper : CopyDepthPass, IFrameGraphRenderPass
    {
        public CopyDepthPassWrapper(URPContext context) : base(FrameGraphConstant.defaultRenderPassEvent, context.copyDepthMaterial)
        {
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            var pi = FrameGraphConfig.GetPass(pass);
            var inputs = pi.inputs;
            var outputs = pi.outputs;
            RenderTargetHandle sourceRT = inputs[0].ToRenderTargetHandle();
            RenderTargetHandle destRt = outputs[0].ToRenderTargetHandle();

            Setup(sourceRT, destRt);
            return true;
        }

        public ScriptableRenderPass GetScriptableRenderPass()
        {
            return this;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
   
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
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