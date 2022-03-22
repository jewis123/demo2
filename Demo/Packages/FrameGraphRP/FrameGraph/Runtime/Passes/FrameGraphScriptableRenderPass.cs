using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// FrameGraph的可编程渲染Pass基类
    /// </summary>
    public abstract class FrameGraphScriptableRenderPass : ScriptableRenderPass, IFrameGraphRenderPass
    {
        public FrameGraphScriptableRenderPass()
        {
            renderPassEvent = FrameGraphConstant.defaultRenderPassEvent;
        }
        
        public bool Setup(int pass, ref RenderingData renderingData)
        {
            var pi = FrameGraphConfig.GetPass(pass);
            var outputs = pi.outputs;
            var inputs = pi.inputs;
            return Setup(inputs, outputs, ref renderingData);
        }

        public ScriptableRenderPass GetScriptableRenderPass()
        {
            return this;
        }
    
        protected abstract bool Setup(int[] inputs, int[] outputs, ref RenderingData renderingData);

        public abstract void ClearUp();
        
        public virtual void ConfigureBeforeEnqueued()
        {

        }
    }
}

