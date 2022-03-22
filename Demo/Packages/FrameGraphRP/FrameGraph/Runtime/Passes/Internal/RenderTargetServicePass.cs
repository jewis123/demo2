using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    internal abstract class RenderTargetServicePass : ScriptableRenderPass, IFrameGraphRenderPass
    {
        protected RenderTargetInfo renderTargetInfo;
        protected RenderTargetHandle renderTargetHandle;

        public RenderTargetServicePass()
        {
            renderPassEvent = FrameGraphConstant.defaultRenderTargetServerEvent;          
        }
        
        public bool Setup(int pass, ref RenderingData renderingData)
        {
            return true;
        }

        public void SetupRenderTarget(int targetRT)
        {
            renderTargetInfo = FrameGraphConfig.GetRenderTarget(targetRT);
            renderTargetHandle = RenderTargetRegistrar.GetRenderTarget(targetRT);
           // ConfigureTarget(RenderTargetHandle.CameraTarget.id);    // 这句代码是为了使ScritpeRenderPass中的overrideCameraTarget被设为true，结合renderPassEvent的顺序使ScriptableRenderer在执行时跳过执行本Pass时触发的重置RenderTarget逻辑
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

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
        }
    }
}