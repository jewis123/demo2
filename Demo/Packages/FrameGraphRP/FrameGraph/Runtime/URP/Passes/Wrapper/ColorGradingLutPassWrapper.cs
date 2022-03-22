using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace FrameGraph
{
    /// <summary>
    /// 实现逻辑同ColorGradingLutPass，但由于ColorGradingLutPass在Execute中去申请RT，所以只能将原逻辑照搬过来并删除申请RT的部分
    /// </summary>
    public class ColorGradingLutPassWrapper : ColorGradingLutPass, IFrameGraphRenderPass
    {
        private readonly Material m_LutBuilderLdr;
        private readonly Material m_LutBuilderHdr;

        private RenderTargetHandle m_InternalLut;

        private ProfilingSampler m_ProfilingSampler;

        public ColorGradingLutPassWrapper(URPContext context) : base(FrameGraphConstant.defaultRenderPassEvent, context.data.postProcessData)
        {
            // 通过反射获取父类的值
            m_LutBuilderLdr = GetType().BaseType.GetField("m_LutBuilderLdr", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as Material;
            m_LutBuilderHdr = GetType().BaseType.GetField("m_LutBuilderHdr", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as Material;
            m_ProfilingSampler = new ProfilingSampler("ColorGradingLUT");
        }

        public bool Setup(int pass, ref RenderingData renderingData)
        {
            m_InternalLut = FrameGraphConfig.GetPass(pass).outputs[0].ToRenderTargetHandle();
            return renderingData.cameraData.postProcessEnabled;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // Fetch all color grading settings
                var stack = VolumeManager.instance.stack;
                var channelMixer = stack.GetComponent<ChannelMixer>();
                var colorAdjustments = stack.GetComponent<ColorAdjustments>();
                var curves = stack.GetComponent<ColorCurves>();
                var liftGammaGain = stack.GetComponent<LiftGammaGain>();
                var shadowsMidtonesHighlights = stack.GetComponent<ShadowsMidtonesHighlights>();
                var splitToning = stack.GetComponent<SplitToning>();
                var tonemapping = stack.GetComponent<Tonemapping>();
                var whiteBalance = stack.GetComponent<WhiteBalance>();

                ref var postProcessingData = ref renderingData.postProcessingData;
                bool hdr = postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;

                // Prepare texture & material
                int lutHeight = postProcessingData.lutSize;
                int lutWidth = lutHeight * lutHeight;
                var material = hdr ? m_LutBuilderHdr : m_LutBuilderLdr;

                // Prepare data
                var lmsColorBalance = ColorUtils.ColorBalanceToLMSCoeffs(whiteBalance.temperature.value, whiteBalance.tint.value);
                var hueSatCon = new Vector4(colorAdjustments.hueShift.value / 360f, colorAdjustments.saturation.value / 100f + 1f, colorAdjustments.contrast.value / 100f + 1f, 0f);
                var channelMixerR = new Vector4(channelMixer.redOutRedIn.value / 100f, channelMixer.redOutGreenIn.value / 100f, channelMixer.redOutBlueIn.value / 100f, 0f);
                var channelMixerG = new Vector4(channelMixer.greenOutRedIn.value / 100f, channelMixer.greenOutGreenIn.value / 100f, channelMixer.greenOutBlueIn.value / 100f, 0f);
                var channelMixerB = new Vector4(channelMixer.blueOutRedIn.value / 100f, channelMixer.blueOutGreenIn.value / 100f, channelMixer.blueOutBlueIn.value / 100f, 0f);

                var shadowsHighlightsLimits = new Vector4(
                    shadowsMidtonesHighlights.shadowsStart.value,
                    shadowsMidtonesHighlights.shadowsEnd.value,
                    shadowsMidtonesHighlights.highlightsStart.value,
                    shadowsMidtonesHighlights.highlightsEnd.value
                );

                var (shadows, midtones, highlights) = ColorUtils.PrepareShadowsMidtonesHighlights(
                    shadowsMidtonesHighlights.shadows.value,
                    shadowsMidtonesHighlights.midtones.value,
                    shadowsMidtonesHighlights.highlights.value
                );

                var (lift, gamma, gain) = ColorUtils.PrepareLiftGammaGain(
                    liftGammaGain.lift.value,
                    liftGammaGain.gamma.value,
                    liftGammaGain.gain.value
                );

                var (splitShadows, splitHighlights) = ColorUtils.PrepareSplitToning(
                    splitToning.shadows.value,
                    splitToning.highlights.value,
                    splitToning.balance.value
                );

                var lutParameters = new Vector4(lutHeight, 0.5f / lutWidth, 0.5f / lutHeight,
                    lutHeight / (lutHeight - 1f));

                // Fill in constants
                material.SetVector(FrameGraphConstant.ShaderConstants._Lut_Params, lutParameters);
                material.SetVector(FrameGraphConstant.ShaderConstants._ColorBalance, lmsColorBalance);
                material.SetVector(FrameGraphConstant.ShaderConstants._ColorFilter, colorAdjustments.colorFilter.value.linear);
                material.SetVector(FrameGraphConstant.ShaderConstants._ChannelMixerRed, channelMixerR);
                material.SetVector(FrameGraphConstant.ShaderConstants._ChannelMixerGreen, channelMixerG);
                material.SetVector(FrameGraphConstant.ShaderConstants._ChannelMixerBlue, channelMixerB);
                material.SetVector(FrameGraphConstant.ShaderConstants._HueSatCon, hueSatCon);
                material.SetVector(FrameGraphConstant.ShaderConstants._Lift, lift);
                material.SetVector(FrameGraphConstant.ShaderConstants._Gamma, gamma);
                material.SetVector(FrameGraphConstant.ShaderConstants._Gain, gain);
                material.SetVector(FrameGraphConstant.ShaderConstants._Shadows, shadows);
                material.SetVector(FrameGraphConstant.ShaderConstants._Midtones, midtones);
                material.SetVector(FrameGraphConstant.ShaderConstants._Highlights, highlights);
                material.SetVector(FrameGraphConstant.ShaderConstants._ShaHiLimits, shadowsHighlightsLimits);
                material.SetVector(FrameGraphConstant.ShaderConstants._SplitShadows, splitShadows);
                material.SetVector(FrameGraphConstant.ShaderConstants._SplitHighlights, splitHighlights);

                // YRGB curves
                material.SetTexture(FrameGraphConstant.ShaderConstants._CurveMaster, curves.master.value.GetTexture());
                material.SetTexture(FrameGraphConstant.ShaderConstants._CurveRed, curves.red.value.GetTexture());
                material.SetTexture(FrameGraphConstant.ShaderConstants._CurveGreen, curves.green.value.GetTexture());
                material.SetTexture(FrameGraphConstant.ShaderConstants._CurveBlue, curves.blue.value.GetTexture());

                // Secondary curves
                material.SetTexture(FrameGraphConstant.ShaderConstants._CurveHueVsHue, curves.hueVsHue.value.GetTexture());
                material.SetTexture(FrameGraphConstant.ShaderConstants._CurveHueVsSat, curves.hueVsSat.value.GetTexture());
                material.SetTexture(FrameGraphConstant.ShaderConstants._CurveLumVsSat, curves.lumVsSat.value.GetTexture());
                material.SetTexture(FrameGraphConstant.ShaderConstants._CurveSatVsSat, curves.satVsSat.value.GetTexture());

                // Tonemapping (baked into the lut for HDR)
                if (hdr)
                {
                    material.shaderKeywords = null;

                    switch (tonemapping.mode.value)
                    {
                        case TonemappingMode.Neutral:
                            material.EnableKeyword(ShaderKeywordStrings.TonemapNeutral);
                            break;
                        case TonemappingMode.ACES:
                            material.EnableKeyword(ShaderKeywordStrings.TonemapACES);
                            break;
                        default: break; // None
                    }
                }

                // Render the lut
                Blit(cmd, m_InternalLut.id, m_InternalLut.id, material);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnFinishCameraStackRendering(CommandBuffer cmd)
        {
        }

        public ScriptableRenderPass GetScriptableRenderPass()
        {
            return this;
        }

        public void ClearUp()
        {
            CoreUtils.Destroy(m_LutBuilderLdr);
            CoreUtils.Destroy(m_LutBuilderHdr);
        }

        public void ConfigureBeforeEnqueued()
        {
            
        }
    }
}