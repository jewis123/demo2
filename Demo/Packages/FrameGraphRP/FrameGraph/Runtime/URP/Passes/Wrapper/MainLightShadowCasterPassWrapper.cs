using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace FrameGraph
{
    public class MainLightShadowCasterPassWrapper : MainLightShadowCasterPass, IFrameGraphRenderPass
    {
        private URPContext m_Context;
        
        public MainLightShadowCasterPassWrapper(URPContext context) : base(FrameGraphConstant.defaultRenderPassEvent)
        {
            m_Context = context;
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            // 如果是使用自定义的RT为阴影的渲染目标，则需要修改对应renderingData中的数值
            var pi = FrameGraphConfig.GetPass(pass);
            var outputs = pi.outputs;
            if (outputs[0].IsValidRenderTarget())
            {
                var rtInfo = FrameGraphConfig.GetRenderTarget(outputs[0]);
                if (rtInfo.rtType == RenderTargetInfo.RenderTargetType.CustomRT)
                {
                    renderingData.shadowData.mainLightShadowmapWidth = rtInfo.width;
                    renderingData.shadowData.mainLightShadowmapHeight = rtInfo.height;
                    int d = (int)rtInfo.downsampling;
                    renderingData.shadowData.mainLightShadowmapWidth >>= d;
                    renderingData.shadowData.mainLightShadowmapHeight >>= d;
                }
            }
            
            var data = m_Context.data;
            return Setup(ref renderingData);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // MainLightShadowCasterPass会在该方法中创建ShadowMap，由于现在创建的流程已在Graph中单独指定，这里对此进行屏蔽
        }

        public ScriptableRenderPass GetScriptableRenderPass()
        {
            return this;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            base.Execute(context, ref renderingData);
            // URP通过RenderPassEvent来控制渲染顺序，阴影会在BeforeRenderingShadows绘制，从BeforeRendering之后开始会重新设置因为绘制阴影导致的视口变化
            // 这部分逻辑详见MainLightShadowCasterPass被构造的地方以及ScriptableRenderer的513行到541行
            // 现在通过连线来决定渲染顺序，所有RenderPassEvent被统一设置为RenderPassEvent.AfterRendering。导致绘制了阴影后视口没有被重置而使之后主相机的绘制出现位置错误
            // 以下部分逻辑拷贝自ScriptableRenderer中的重置逻辑
            
            CommandBuffer cmd = CommandBufferPool.Get();
            context.SetupCameraProperties(renderingData.cameraData.camera);
            ScriptableRenderer.SetCameraMatrices(cmd, ref renderingData.cameraData, true);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public void ClearUp()
        {
            
        }

        public void ConfigureBeforeEnqueued()
        {
            
        }
    }
}