using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    public class DrawSkyboxPassWrapper : DrawSkyboxPass, IFrameGraphRenderPass
    {
        public DrawSkyboxPassWrapper(URPContext context) : base(FrameGraphConstant.defaultRenderPassEvent)
        {
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            Skybox cameraSkybox;
            renderingData.cameraData.camera.TryGetComponent<Skybox>(out cameraSkybox);
            return renderingData.cameraData.camera.clearFlags == CameraClearFlags.Skybox && (RenderSettings.skybox != null || cameraSkybox?.material != null) ;
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

