using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    public class FinalTargetPostProcessPassWrapper : PostProcessPassWrapper
    {
        public FinalTargetPostProcessPassWrapper(URPContext context) : base(context)
        {
        }

        protected override void OnExecute(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (renderingData.postProcessingEnabled && renderingData.cameraData.postProcessEnabled)
            {
                Render(cmd, ref renderingData, source, destination);
            }
            else
            {
                RenderFinalPass(cmd, ref renderingData, source);
            }
        }

        protected override bool Setup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.resolveFinalTarget)
            {
                // if resolving to screen we need to be able to perform sRGBConvertion in post-processing if necessary
                enableSRGBConversionIfNeeded = !outputs[0].IsValidRenderTarget();

                source = inputs[0].ToRenderTargetHandle();
                internalLut = inputs[1].ToRenderTargetHandle();
                bloomTex = inputs[2].ToRenderTargetHandle();

                destination = outputs[0].ToRenderTargetHandle();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}