using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    public abstract class BasePostProcessPass : FrameGraphScriptableRenderPass
    {
        [Serializable]
        public class Settings
        {
            public bool swap = true;
        }

        protected URPContext context;
        protected Settings settings;
        protected RenderTextureDescriptor descriptor;
        
        
        protected RenderTargetHandle destination;
        protected RenderTargetHandle source;
        
        private ProfilingSampler m_ProfilingSampler;
        private int m_DestinationId;
        private int m_SourceId;
        
        public BasePostProcessPass(URPContext context, Settings settings, string profileTag)
        {
            m_ProfilingSampler = new ProfilingSampler(profileTag);
            this.context = context;
            this.settings = settings;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                descriptor = renderingData.cameraData.cameraTargetDescriptor;
                OnProcess(cmd, context, ref renderingData);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            OnProcessFinish();
        }

        public override void ClearUp()
        {

        }

        protected override bool Setup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            m_DestinationId = outputs[0];
            m_SourceId = inputs[0];
            destination = m_DestinationId.ToRenderTargetHandle();
            source = m_SourceId.ToRenderTargetHandle();

            return OnSetup(inputs, outputs, ref renderingData);
        }

        public override void ConfigureBeforeEnqueued()
        {
            if (settings.swap)
            {
                SwapRenderTarget();
            }
        }

        protected void SwapRenderTarget()
        {
            RenderTargetRegistrar.SwapRenderTarget(m_SourceId, m_DestinationId);
        }

        protected abstract void OnProcess(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData);
        
        protected abstract bool OnSetup(int[] inputs, int[] outputs, ref RenderingData renderingData);

        protected virtual void OnProcessFinish()
        {
            
        }
    }
}