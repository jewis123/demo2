using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// 整体逻辑拷贝在PostProcessPass，移除固定在里面中的Bloom，DOF，MotionBlur，PaniniProjection逻辑。将RT的设置开放为外部传入
    /// </summary>
    public abstract class PostProcessPassWrapper : FrameGraphScriptableRenderPass
    {
        RenderTextureDescriptor m_Descriptor;
        protected RenderTargetHandle source;
        protected RenderTargetHandle destination;
        protected RenderTargetHandle internalLut;
        protected RenderTargetHandle bloomTex;

        const string k_RenderFinalPostProcessingTag = "Render Final PostProcessing Pass";
        private static readonly ProfilingSampler m_ProfilingRenderFinalPostProcessing = new ProfilingSampler(k_RenderFinalPostProcessingTag);

        MaterialLibrary m_Materials;
        PostProcessData m_Data;

        // Builtin effects settings
        Bloom m_Bloom;
        LensDistortion m_LensDistortion;
        ChromaticAberration m_ChromaticAberration;
        Vignette m_Vignette;
        ColorLookup m_ColorLookup;
        ColorAdjustments m_ColorAdjustments;
        Tonemapping m_Tonemapping;
        FilmGrain m_FilmGrain;

        // Misc
        bool m_UseRGBM;

        int m_DitheringTextureIndex;

        // Some Android devices do not support sRGB backbuffer
        // We need to do the conversion manually on those
        protected bool enableSRGBConversionIfNeeded;

        private URPContext m_Context;

        private IVolumePatchedFunction<Tonemapping> m_TonemappingFunction;

        public PostProcessPassWrapper(URPContext context) : base()
        {
            base.profilingSampler = new ProfilingSampler("PostProcessPass");
            m_Context = context;
            m_Data = context.data.postProcessData;
            m_Materials = context.postProcessMaterials;

            // Texture format pre-lookup
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.Linear | FormatUsage.Render))
            {
                m_UseRGBM = false;
            }
            else
            {
                m_UseRGBM = true;
            }

            m_TonemappingFunction = VolumeFunctionManager.Instance.GetFunction<Tonemapping>();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            m_Descriptor = renderingData.cameraData.cameraTargetDescriptor;

            // Start by pre-fetching all builtin effect settings we need
            // Some of the color-grading settings are only used in the color grading lut pass
            var stack = VolumeManager.instance.stack;
            m_Bloom = stack.GetComponent<Bloom>();
            m_LensDistortion = stack.GetComponent<LensDistortion>();
            m_ChromaticAberration = stack.GetComponent<ChromaticAberration>();
            m_Vignette = stack.GetComponent<Vignette>();
            m_ColorLookup = stack.GetComponent<ColorLookup>();
            m_ColorAdjustments = stack.GetComponent<ColorAdjustments>();
            m_Tonemapping = stack.GetComponent<Tonemapping>();
            m_FilmGrain = stack.GetComponent<FilmGrain>();

            // Regular render path (not on-tile) - we do everything in a single command buffer as it
            // makes it easier to manage temporary targets' lifetime
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingRenderFinalPostProcessing))
            {
                OnExecute(cmd, ref renderingData);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        protected abstract void OnExecute(CommandBuffer cmd, ref RenderingData renderingData);

        protected override bool Setup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            // if resolving to screen we need to be able to perform sRGBConvertion in post-processing if necessary
            enableSRGBConversionIfNeeded = !outputs[0].IsValidRenderTarget();

            source = inputs[0].ToRenderTargetHandle();
            internalLut = inputs[1].ToRenderTargetHandle();
            bloomTex = inputs[2].ToRenderTargetHandle();

            destination = outputs[0].ToRenderTargetHandle();
            return true;
        }

        public override void ClearUp()
        {
        }

        protected void Render(CommandBuffer cmd, ref RenderingData renderingData, RenderTargetHandle sourceTex, RenderTargetHandle destinationTex)
        {
            ref var cameraData = ref renderingData.cameraData;

            // Don't use these directly unless you have a good reason to, use GetSource() and
            // GetDestination() instead
            bool isSceneViewCamera = cameraData.isSceneViewCamera;

            // Combined post-processing stack

            // Reset uber keywords
            m_Materials.uber.shaderKeywords = null;

            // Bloom goes first
            bool bloomActive = m_Bloom.IsActive();
            if (bloomActive)
            {
                SetupBloom(cmd, m_Materials.uber);
            }

            // Setup other effects constants
            SetupLensDistortion(m_Materials.uber, isSceneViewCamera);
            SetupChromaticAberration(m_Materials.uber);
            SetupVignette(m_Materials.uber);
            SetupColorGrading(cmd, ref renderingData, m_Materials.uber);

            // Only apply dithering & grain if there isn't a final pass.
            SetupGrain(cameraData, m_Materials.uber);
            SetupDithering(cameraData, m_Materials.uber);

            if (RequireSRGBConversionBlitToBackBuffer(cameraData))
                m_Materials.uber.EnableKeyword(ShaderKeywordStrings.LinearToSRGBConversion);

            // Done with Uber, blit it
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._SourceTex, sourceTex.id);

            var colorLoadAction = RenderBufferLoadAction.DontCare;
            if (destinationTex == RenderTargetHandle.CameraTarget && !cameraData.isDefaultViewport)
                colorLoadAction = RenderBufferLoadAction.Load;

            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);

            if (destinationTex == RenderTargetHandle.CameraTarget)
                cmd.SetViewport(cameraData.camera.pixelRect);

            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_Materials.uber);

            cmd.SetViewProjectionMatrices(cameraData.camera.worldToCameraMatrix, cameraData.camera.projectionMatrix);
        }

        bool RequireSRGBConversionBlitToBackBuffer(CameraData cameraData)
        {
            return Display.main.requiresSrgbBlitToBackbuffer && enableSRGBConversionIfNeeded;
        }


        #region Final pass

        protected void RenderFinalPass(CommandBuffer cmd, ref RenderingData renderingData, RenderTargetHandle sourceTex)
        {
            ref var cameraData = ref renderingData.cameraData;
            var material = m_Context.blitMaterial;//m_Materials.finalPass;
            material.shaderKeywords = null;

            // FXAA setup
            if (cameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing)
                material.EnableKeyword(ShaderKeywordStrings.Fxaa);

            FrameGraphPostProcessUtils.SetSourceSize(cmd, cameraData.cameraTargetDescriptor);

            SetupGrain(cameraData, material);
            SetupDithering(cameraData, material);

            if (RequireSRGBConversionBlitToBackBuffer(cameraData))
                material.EnableKeyword(ShaderKeywordStrings.LinearToSRGBConversion);

            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._SourceTex, sourceTex.Identifier());


            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.SetViewport(cameraData.camera.pixelRect);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material);
            cmd.SetViewProjectionMatrices(cameraData.camera.worldToCameraMatrix, cameraData.camera.projectionMatrix);
        }

        #endregion

        #region Bloom

        void SetupBloom(CommandBuffer cmd, Material uberMaterial)
        {
            // Setup bloom on uber
            var tint = m_Bloom.tint.value.linear;
            var luma = ColorUtils.Luminance(tint);
            tint = luma > 0f ? tint * (1f / luma) : Color.white;

            var bloomParams = new Vector4(m_Bloom.intensity.value, tint.r, tint.g, tint.b);
            uberMaterial.SetVector(FrameGraphConstant.ShaderConstants._Bloom_Params, bloomParams);
            uberMaterial.SetFloat(FrameGraphConstant.ShaderConstants._Bloom_RGBM, m_UseRGBM ? 1f : 0f);

            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._Bloom_Texture, bloomTex.id);

            // Setup lens dirtiness on uber
            // Keep the aspect ratio correct & center the dirt texture, we don't want it to be
            // stretched or squashed
            var dirtTexture = m_Bloom.dirtTexture.value == null ? Texture2D.blackTexture : m_Bloom.dirtTexture.value;
            float dirtRatio = dirtTexture.width / (float) dirtTexture.height;
            float screenRatio = m_Descriptor.width / (float) m_Descriptor.height;
            var dirtScaleOffset = new Vector4(1f, 1f, 0f, 0f);
            float dirtIntensity = m_Bloom.dirtIntensity.value;

            if (dirtRatio > screenRatio)
            {
                dirtScaleOffset.x = screenRatio / dirtRatio;
                dirtScaleOffset.z = (1f - dirtScaleOffset.x) * 0.5f;
            }
            else if (screenRatio > dirtRatio)
            {
                dirtScaleOffset.y = dirtRatio / screenRatio;
                dirtScaleOffset.w = (1f - dirtScaleOffset.y) * 0.5f;
            }

            uberMaterial.SetVector(FrameGraphConstant.ShaderConstants._LensDirt_Params, dirtScaleOffset);
            uberMaterial.SetFloat(FrameGraphConstant.ShaderConstants._LensDirt_Intensity, dirtIntensity);
            uberMaterial.SetTexture(FrameGraphConstant.ShaderConstants._LensDirt_Texture, dirtTexture);

            // Keyword setup - a bit convoluted as we're trying to save some variants in Uber...
            if (m_Bloom.highQualityFiltering.value)
                uberMaterial.EnableKeyword(dirtIntensity > 0f ? ShaderKeywordStrings.BloomHQDirt : ShaderKeywordStrings.BloomHQ);
            else
                uberMaterial.EnableKeyword(dirtIntensity > 0f ? ShaderKeywordStrings.BloomLQDirt : ShaderKeywordStrings.BloomLQ);
        }

        #endregion

        #region Lens Distortion

        void SetupLensDistortion(Material material, bool isSceneView)
        {
            float amount = 1.6f * Mathf.Max(Mathf.Abs(m_LensDistortion.intensity.value * 100f), 1f);
            float theta = Mathf.Deg2Rad * Mathf.Min(160f, amount);
            float sigma = 2f * Mathf.Tan(theta * 0.5f);
            var center = m_LensDistortion.center.value * 2f - Vector2.one;
            var p1 = new Vector4(
                center.x,
                center.y,
                Mathf.Max(m_LensDistortion.xMultiplier.value, 1e-4f),
                Mathf.Max(m_LensDistortion.yMultiplier.value, 1e-4f)
            );
            var p2 = new Vector4(
                m_LensDistortion.intensity.value >= 0f ? theta : 1f / theta,
                sigma,
                1f / m_LensDistortion.scale.value,
                m_LensDistortion.intensity.value * 100f
            );

            material.SetVector(FrameGraphConstant.ShaderConstants._Distortion_Params1, p1);
            material.SetVector(FrameGraphConstant.ShaderConstants._Distortion_Params2, p2);

            if (m_LensDistortion.IsActive() && !isSceneView)
                material.EnableKeyword(ShaderKeywordStrings.Distortion);
        }

        #endregion

        #region Chromatic Aberration

        void SetupChromaticAberration(Material material)
        {
            material.SetFloat(FrameGraphConstant.ShaderConstants._Chroma_Params, m_ChromaticAberration.intensity.value * 0.05f);

            if (m_ChromaticAberration.IsActive())
                material.EnableKeyword(ShaderKeywordStrings.ChromaticAberration);
        }

        #endregion

        #region Vignette

        void SetupVignette(Material material)
        {
            var color = m_Vignette.color.value;
            var center = m_Vignette.center.value;
            var aspectRatio = m_Descriptor.width / (float) m_Descriptor.height;

            var v1 = new Vector4(
                color.r, color.g, color.b,
                m_Vignette.rounded.value ? aspectRatio : 1f
            );
            var v2 = new Vector4(
                center.x, center.y,
                m_Vignette.intensity.value * 3f,
                m_Vignette.smoothness.value * 5f
            );

            material.SetVector(FrameGraphConstant.ShaderConstants._Vignette_Params1, v1);
            material.SetVector(FrameGraphConstant.ShaderConstants._Vignette_Params2, v2);
        }

        #endregion

        #region Color Grading

        void SetupColorGrading(CommandBuffer cmd, ref RenderingData renderingData, Material material)
        {
            ref var postProcessingData = ref renderingData.postProcessingData;
            bool hdr = postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;
            int lutHeight = postProcessingData.lutSize;
            int lutWidth = lutHeight * lutHeight;

            // Source material setup
            float postExposureLinear = Mathf.Pow(2f, m_ColorAdjustments.postExposure.value);
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._InternalLut, internalLut.Identifier());
            material.SetVector(FrameGraphConstant.ShaderConstants._Lut_Params, new Vector4(1f / lutWidth, 1f / lutHeight, lutHeight - 1f, postExposureLinear));
            material.SetTexture(FrameGraphConstant.ShaderConstants._UserLut, m_ColorLookup.texture.value);
            material.SetVector(FrameGraphConstant.ShaderConstants._UserLut_Params, !m_ColorLookup.IsActive()
                ? Vector4.zero
                : new Vector4(1f / m_ColorLookup.texture.value.width,
                    1f / m_ColorLookup.texture.value.height,
                    m_ColorLookup.texture.value.height - 1f,
                    m_ColorLookup.contribution.value)
            );

            if (hdr)
            {
                material.EnableKeyword(ShaderKeywordStrings.HDRGrading);
            }
            else
            {
                switch (m_Tonemapping.mode.value)
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
            
            m_TonemappingFunction?.OnVolumePatched(cmd, renderingData, material, m_Tonemapping);
        }

        #endregion

        #region Film Grain

        void SetupGrain(in CameraData cameraData, Material material)
        {
            if (m_FilmGrain.IsActive())
            {
                material.EnableKeyword(ShaderKeywordStrings.FilmGrain);
                PostProcessUtils.ConfigureFilmGrain(
                    m_Data,
                    m_FilmGrain,
                    cameraData.camera.pixelWidth, cameraData.camera.pixelHeight,
                    material
                );
            }
        }

        #endregion

        #region 8-bit Dithering

        void SetupDithering(in CameraData cameraData, Material material)
        {
            if (cameraData.isDitheringEnabled)
            {
                material.EnableKeyword(ShaderKeywordStrings.Dithering);
                m_DitheringTextureIndex = PostProcessUtils.ConfigureDithering(
                    m_Data,
                    m_DitheringTextureIndex,
                    cameraData.camera.pixelWidth, cameraData.camera.pixelHeight,
                    material
                );
            }
        }

        #endregion
    }
}