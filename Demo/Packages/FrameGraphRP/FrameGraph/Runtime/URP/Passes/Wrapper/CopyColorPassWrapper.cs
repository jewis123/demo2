using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace FrameGraph
{
    public class CopyColorPassWrapper : CopyColorPass, IFrameGraphRenderPass
    {
        public enum SampleMode
        {
            Bilinear,
            Box
        }

        [Serializable]
        public class Settings
        {
            public SampleMode sampleMode;
            public Material samplingMaterial;
            public Material copyMaterial;
        }

        public CopyColorPassWrapper(URPContext context, Settings settings) : base(FrameGraphConstant.defaultRenderPassEvent,
            settings.samplingMaterial == null ? context.samplingMaterial : settings.samplingMaterial, 
            settings.copyMaterial == null ? context.blitMaterial : settings.copyMaterial)
        {

        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            var pi = FrameGraphConfig.GetPass(pass);
            var inputs = pi.inputs;
            var outputs = pi.outputs;
            RenderTargetHandle sourceRT = inputs[0].ToRenderTargetHandle();
            RenderTargetHandle destRt = outputs[0].ToRenderTargetHandle();

            var settings = pi.contexts[0] as Settings;
            Setup(sourceRT.Identifier(), destRt, settings.sampleMode == SampleMode.Box ? UnityEngine.Rendering.Universal.Downsampling._4xBox : UnityEngine.Rendering.Universal.Downsampling._4xBilinear);
            return true;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // 去除原CopyColorPass中申请RT内存的逻辑
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // 去除原CopyColorPass中释放RT内存的逻辑
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