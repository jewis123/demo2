using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// 从PostProcessPass中剥离出来的Bloom部分
    /// </summary>
    public class BloomPassWrapper : BasePostProcessPass
    {
        private Bloom m_Bloom;

        // Misc
        private int m_MaxPyramidSize = 4;
        private bool m_UseRGBM;

        public BloomPassWrapper(URPContext context, BasePostProcessPass.Settings settings) : base(context, settings, "Bloom")
        {
            // Texture format pre-lookup
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.Linear | FormatUsage.Render))
            {
                m_UseRGBM = false;
            }
            else
            {
                m_UseRGBM = true;
            }

            // Bloom pyramid shader ids - can't use a simple stackalloc in the bloom function as we
            // unfortunately need to allocate strings
            FrameGraphConstant.ShaderConstants._BloomMipUp = new int[m_MaxPyramidSize];
            FrameGraphConstant.ShaderConstants._BloomMipDown = new int[m_MaxPyramidSize];

            // for (int i = 0; i < k_MaxPyramidSize; i++)
            // {
            //     FrameGraphConstant.ShaderConstants._BloomMipUp[i] = Shader.PropertyToID("_BloomMipUp" + i);
            //     FrameGraphConstant.ShaderConstants._BloomMipDown[i] = Shader.PropertyToID("_BloomMipDown" + i);
            // }
        }

        protected override void OnProcess(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SetupBloom(cmd, source.id);
        }

        protected override bool OnSetup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            m_Bloom = stack.GetComponent<Bloom>();

            for (int i = 0; i < m_MaxPyramidSize; i++)
            {
                FrameGraphConstant.ShaderConstants._BloomMipUp[i] = outputs[i * 2].ToRenderTargetHandle().id;
                FrameGraphConstant.ShaderConstants._BloomMipDown[i] = outputs[i * 2 + 1].ToRenderTargetHandle().id;
            }

            return renderingData.cameraData.postProcessEnabled && m_Bloom.IsActive();
        }

        #region Bloom

        void SetupBloom(CommandBuffer cmd, int source)
        {
            // 由于PostProcessPass中Bloom一共用到了32张RT，过于多，所以这里进行精简，改为了8张
            // Start at half-res
            int tw = descriptor.width >> 1;
            int th = descriptor.height >> 1;

            int mipCount = m_MaxPyramidSize;

            // Pre-filtering parameters
            float clamp = m_Bloom.clamp.value;
            float threshold = Mathf.GammaToLinearSpace(m_Bloom.threshold.value);
            float thresholdKnee = threshold * 0.5f; // Hardcoded soft knee

            // Material setup
            float scatter = Mathf.Lerp(0.05f, 0.95f, m_Bloom.scatter.value);
            var bloomMaterial = context.postProcessMaterials.bloom;
            bloomMaterial.SetVector(FrameGraphConstant.ShaderConstants._Params, new Vector4(scatter, clamp, threshold, thresholdKnee));
            CoreUtils.SetKeyword(bloomMaterial, ShaderKeywordStrings.BloomHQ, m_Bloom.highQualityFiltering.value);
            CoreUtils.SetKeyword(bloomMaterial, ShaderKeywordStrings.UseRGBM, m_UseRGBM);

            FrameGraphPostProcessUtils.Blit(cmd, source, FrameGraphConstant.ShaderConstants._BloomMipDown[0], bloomMaterial, 0);
            
            // Downsample - gaussian pyramid
            int lastDown = FrameGraphConstant.ShaderConstants._BloomMipDown[0];
            for (int i = 1; i < mipCount; i++)
            {
                tw = Mathf.Max(1, tw >> 1);
                th = Mathf.Max(1, th >> 1);
                int mipDown = FrameGraphConstant.ShaderConstants._BloomMipDown[i];
                int mipUp = FrameGraphConstant.ShaderConstants._BloomMipUp[i];

                // Classic two pass gaussian blur - use mipUp as a temporary target
                //   First pass does 2x downsampling + 9-tap gaussian
                //   Second pass does 9-tap gaussian using a 5-tap filter + bilinear filtering
                FrameGraphPostProcessUtils.Blit(cmd, lastDown, mipUp, bloomMaterial, 1);
                FrameGraphPostProcessUtils.Blit(cmd, mipUp, mipDown, bloomMaterial, 2);

                lastDown = mipDown;
            }

            // Upsample (bilinear by default, HQ filtering does bicubic instead
            for (int i = mipCount - 2; i >= 0; i--)
            {
                int lowMip = (i == mipCount - 2) ? FrameGraphConstant.ShaderConstants._BloomMipDown[i + 1] : FrameGraphConstant.ShaderConstants._BloomMipUp[i + 1];
                int highMip = FrameGraphConstant.ShaderConstants._BloomMipDown[i];
                int dst = FrameGraphConstant.ShaderConstants._BloomMipUp[i];

                cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._SourceTexLowMip, lowMip);
                FrameGraphPostProcessUtils.Blit(cmd, highMip, FrameGraphPostProcessUtils.BlitDstDiscardContent(cmd, dst), bloomMaterial, 3);
            }
        }

        #endregion
    }
}