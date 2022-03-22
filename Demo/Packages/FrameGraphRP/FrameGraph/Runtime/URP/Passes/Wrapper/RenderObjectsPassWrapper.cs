using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    public class RenderObjectsPassWrapper : RenderObjectsPass, IFrameGraphRenderPass
    {
        // 这里定义一个新的Settings是为了在Inspector上屏蔽RenderPassEvent字段
        [Serializable]
        public class Settings : RenderObjects.RenderObjectsSettings
        {
            
        }
        
        public RenderObjectsPassWrapper(URPContext context, Settings settings) : base(
            settings.passTag, FrameGraphConstant.defaultRenderPassEvent, settings.filterSettings.PassNames,
            settings.filterSettings.RenderQueueType, settings.filterSettings.LayerMask, settings.cameraSettings)
        {
            // 沿用RenderObjectsPass初始化的逻辑
            overrideMaterial = settings.overrideMaterial;
            overrideMaterialPassIndex = settings.overrideMaterialPassIndex;
            
            if (settings.overrideDepthState)
                SetDetphState(settings.enableWrite, settings.depthCompareFunction);

            if (settings.stencilSettings.overrideStencilState)
                SetStencilState(settings.stencilSettings.stencilReference,
                    settings.stencilSettings.stencilCompareFunction, settings.stencilSettings.passOperation,
                    settings.stencilSettings.failOperation, settings.stencilSettings.zFailOperation);
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            return true;
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