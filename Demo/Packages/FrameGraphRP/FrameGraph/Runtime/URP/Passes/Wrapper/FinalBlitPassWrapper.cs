using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace FrameGraph
{
    public class FinalBlitPassWrapper : FinalBlitPass, IFrameGraphRenderPass
    {
        public FinalBlitPassWrapper(URPContext context) : base(FrameGraphConstant.defaultRenderPassEvent, context.blitMaterial)
        {
            
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            var pi = FrameGraphConfig.GetPass(pass);
            var inputs = pi.inputs;
            var outputs = pi.outputs;
            // FinalBlitPass的Setup中虽然传入了类型为RenderTextureDescriptor的参数，但并未实际使用，所以这里直接传入defualt值
            // 取inputs下标为0的值是和RGFinalBlitPassNode中第一Input参数的顺序保持一致
            Setup(default(RenderTextureDescriptor), inputs[0].ToRenderTargetHandle());
            return inputs[0].ToRenderTargetHandle() != outputs[0].ToRenderTargetHandle();
        }

        public ScriptableRenderPass GetScriptableRenderPass()
        {
            return this;
        }
        
        public void ClearUp()
        {
            
        }

        public void ConfigureBeforeEnqueued()
        {
            
        }
    }
}