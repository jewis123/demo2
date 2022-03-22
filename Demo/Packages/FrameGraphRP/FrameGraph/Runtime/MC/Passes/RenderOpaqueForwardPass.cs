using FrameGraph;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph.MC
{
    public class RenderOpaqueForwardPass : RenderOpaqueForwardPassWrapper
    {
        protected bool hasHairShadowMap;
        protected bool hasCharacterDepthTex;
    
        private RenderTargetHandle m_HairShadowMask;
        private RenderTargetHandle m_CharacterDepthTex;
    
        public RenderOpaqueForwardPass(URPContext context) : base(context)
        {
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            bool result = base.Setup(pass, ref renderingData);
            var pi = FrameGraphConfig.GetPass(pass);
            var inputs = pi.inputs;
            hasHairShadowMap = inputs[1].IsValidRenderTarget() && inputs[1].IsRenderTargetAllocated();
            m_HairShadowMask = inputs[1].ToRenderTargetHandle();

            hasCharacterDepthTex = inputs[2].IsValidRenderTarget() && inputs[2].IsRenderTargetAllocated();
            m_CharacterDepthTex = inputs[2].ToRenderTargetHandle();
            return result;
        }
    
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            
            if (hasHairShadowMap)
                cmd.SetGlobalTexture("_HairShadowMask", m_HairShadowMask.Identifier());
            
            if (hasCharacterDepthTex)
                cmd.SetGlobalTexture("_CharacterDepthTexture", m_CharacterDepthTex.Identifier());
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            base.Execute(context, ref renderingData);
        }
    }
}

