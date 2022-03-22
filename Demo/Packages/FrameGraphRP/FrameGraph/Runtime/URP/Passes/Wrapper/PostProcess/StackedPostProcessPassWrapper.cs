using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    public class StackedPostProcessPassWrapper : PostProcessPassWrapper
    {
        protected BasePostProcessPass.Settings settings;
        
        private int m_DestinationId;
        private int m_SourceId;
        
        public StackedPostProcessPassWrapper(URPContext context, BasePostProcessPass.Settings settings) : base(context)
        {
            this.settings = settings;
        }

        protected override bool Setup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.resolveFinalTarget && renderingData.postProcessingEnabled && renderingData.cameraData.postProcessEnabled)
            {
                m_SourceId = inputs[0];
                m_DestinationId = outputs[0];
                destination = m_DestinationId.ToRenderTargetHandle();
                source = m_SourceId.ToRenderTargetHandle();
                // if resolving to screen we need to be able to perform sRGBConvertion in post-processing if necessary
                enableSRGBConversionIfNeeded = !outputs[0].IsValidRenderTarget();

                internalLut = inputs[1].ToRenderTargetHandle();
                bloomTex = inputs[2].ToRenderTargetHandle();

                return true;
            }
            else
            {
                return false;
            }
        }
        
        protected override void OnExecute(CommandBuffer cmd, ref RenderingData renderingData)
        {
            cmd.SetRenderTarget(destination.id);
            RenderFinalPass(cmd, ref renderingData, source);
            cmd.SetRenderTarget(source.id);
            Render(cmd, ref renderingData, destination, source);
        }
        
        // public override void ConfigureBeforeEnqueued()
        // {
        //     if (settings.swap)
        //     {
        //         SwapRenderTarget();
        //     }
        // }
        //
        // protected void SwapRenderTarget()
        // {
        //     RenderTargetRegistrar.SwapRenderTarget(m_SourceId, m_DestinationId);
        // }
    }
}