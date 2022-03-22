using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;


namespace FrameGraph
{
    /// <summary>
    /// 这里将URP的TransparentSettingsPass逻辑合并到此处，不再使TransparentSettingsPass单独作为一个Pass
    /// 由于TransparentSettingsPass会控制阴影宏的开关，URP是在绘制半透明物体之前执行该Pass，所以不会产生问题。而FrameGraph并不知晓具体顺序，所以将宏的控制逻辑合并到这里
    /// </summary>
    public abstract class DrawObjectPassWrapper : DrawObjectsPass, IFrameGraphRenderPass
    {
        protected virtual bool needShadow => hasShadowMap;

        protected bool hasShadowMap;
        private RenderTargetHandle m_ShadowMap;
        
        public DrawObjectPassWrapper(URPContext context, string profilerTag, bool opaque, RenderQueueRange renderQueueRange, LayerMask layerMask)
            : base(profilerTag, opaque, FrameGraphConstant.defaultRenderPassEvent, renderQueueRange,
                layerMask, context.defaultStencilState,
                context.stencilData.stencilReference)
        {
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            var pi = FrameGraphConfig.GetPass(pass);
            var inputs = pi.inputs;
            hasShadowMap = inputs[0].IsValidRenderTarget() && inputs[0].IsRenderTargetAllocated();
            m_ShadowMap = inputs[0].ToRenderTargetHandle();
            return true;
        }

        public ScriptableRenderPass GetScriptableRenderPass()
        {
            return this;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // URP中开关阴影的宏由MainLightShadowCasterPass决定，而FrameGraph中的ShadowMap不一定来自于MainLightShadowCasterPass
            // 为了Pass的自洽，当启用了ShadowMap就要打开阴影宏。否则关闭阴影
            if (needShadow)
            {
                CommandBuffer cmd = CommandBufferPool.Get();
                cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._ShadowMapTexture, m_ShadowMap.Identifier());
                context.ExecuteCommandBuffer(cmd);

                bool softShadows = false;
                int shadowLightIndex = renderingData.lightData.mainLightIndex;
                if (shadowLightIndex != -1)
                {
                    VisibleLight shadowLight = renderingData.lightData.visibleLights[shadowLightIndex];
                    softShadows = shadowLight.light.shadows == LightShadows.Soft && renderingData.shadowData.supportsSoftShadows;
                }

                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.MainLightShadows, renderingData.shadowData.mainLightShadowCascadesCount == 1);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.MainLightShadowCascades, renderingData.shadowData.mainLightShadowCascadesCount > 1);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SoftShadows, softShadows);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            else
            {
                CommandBuffer cmd = CommandBufferPool.Get();
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.MainLightShadows, false);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.MainLightShadowCascades, false);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.AdditionalLightShadows, false);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            
            base.Execute(context, ref renderingData);
        }
        
        public void ClearUp()
        {
            
        }

        public void ConfigureBeforeEnqueued()
        {
            
        }
    }
}