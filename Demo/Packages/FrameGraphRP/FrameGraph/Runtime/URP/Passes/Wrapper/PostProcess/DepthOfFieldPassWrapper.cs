using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// 从PostProcessPass中剥离出来的景深部分
    /// </summary>
    public class DepthOfFieldPassWrapper : BasePostProcessPass
    {
        private DepthOfField m_DepthOfField;

        private RenderTargetHandle m_FullCoCTexture;
        private RenderTargetHandle m_HalfCoCTexture;
        private RenderTargetHandle m_PingTexture;
        private RenderTargetHandle m_PongTexture;

        private RenderTargetHandle m_DepthTex;

        private RenderTargetIdentifier[] m_MRT2;
        private Vector4[] m_BokehKernel;
        private int m_BokehHash;

        private Downsampling m_Downsampling;

        public DepthOfFieldPassWrapper(URPContext context, BasePostProcessPass.Settings settings) : base(context, settings, "DepthOfField")
        {
            m_MRT2 = new RenderTargetIdentifier[2];
        }

        protected override void OnProcess(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // 执行逻辑来自PostProcessPass的368行
            DoDepthOfField(renderingData.cameraData.camera, cmd, source.id, destination.id, renderingData.cameraData.camera.pixelRect);
        }

        protected override bool OnSetup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            // 判断逻辑来自PostProcessPass的362行
            var stack = VolumeManager.instance.stack;
            m_DepthOfField = stack.GetComponent<DepthOfField>();

            m_FullCoCTexture = outputs[1].ToRenderTargetHandle();
            m_HalfCoCTexture = outputs[2].ToRenderTargetHandle();
            m_PingTexture = outputs[3].ToRenderTargetHandle();
            m_PongTexture = outputs[4].ToRenderTargetHandle();

            m_DepthTex = inputs[1].ToRenderTargetHandle();

            var ri = FrameGraphConfig.GetRenderTarget(outputs[2]);
            if (ri != null)
            {
                m_Downsampling = ri.downsampling;
            }

            return renderingData.cameraData.postProcessEnabled && m_DepthOfField.IsActive() && !renderingData.cameraData.isSceneViewCamera;
        }

        // 原逻辑为PostProcessPass的600行

        #region Depth Of Field

        // TODO: CoC reprojection once TAA gets in LW
        // TODO: Proper LDR/gamma support
        void DoDepthOfField(Camera camera, CommandBuffer cmd, int source, int destination, Rect pixelRect)
        {
            // Setup projection matrix for cmd.DrawMesh()
            cmd.SetGlobalMatrix(FrameGraphConstant.ShaderConstants._FullscreenProjMat, GL.GetGPUProjectionMatrix(Matrix4x4.identity, true));
            
            if (m_DepthOfField.mode.value == DepthOfFieldMode.Gaussian)
                DoGaussianDepthOfField(camera, cmd, source, destination, pixelRect);
            else if (m_DepthOfField.mode.value == DepthOfFieldMode.Bokeh)
                DoBokehDepthOfField(cmd, source, destination, pixelRect);
        }

        void DoGaussianDepthOfField(Camera camera, CommandBuffer cmd, int source, int destination, Rect pixelRect)
        {
            int downSample = (int) m_Downsampling + 1;
            var material = context.postProcessMaterials.gaussianDepthOfField;
            int wh = descriptor.width / downSample;
            float farStart = m_DepthOfField.gaussianStart.value;
            float farEnd = Mathf.Max(farStart, m_DepthOfField.gaussianEnd.value);

            // Assumes a radius of 1 is 1 at 1080p
            // Past a certain radius our gaussian kernel will look very bad so we'll clamp it for
            // very high resolutions (4K+).
            float maxRadius = m_DepthOfField.gaussianMaxRadius.value * (wh / 1080f);
            maxRadius = Mathf.Min(maxRadius, 2f);

            CoreUtils.SetKeyword(material, ShaderKeywordStrings.HighQualitySampling, m_DepthOfField.highQualitySampling.value);
            material.SetVector(FrameGraphConstant.ShaderConstants._CoCParams, new Vector3(farStart, farEnd, maxRadius));

            FrameGraphPostProcessUtils.SetSourceSize(cmd, descriptor);
            cmd.SetGlobalVector(FrameGraphConstant.ShaderConstants._DownSampleScaleFactor, new Vector4(1.0f / downSample, 1.0f / downSample, downSample, downSample));

            // Compute CoC
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._CameraDepthTexture, m_DepthTex.id);
            FrameGraphPostProcessUtils.Blit(cmd, source, m_FullCoCTexture.id, material, 0);

            // Downscale & prefilter color + coc
            m_MRT2[0] = m_HalfCoCTexture.id;
            m_MRT2[1] = m_PingTexture.id;

            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.SetViewport(pixelRect);
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._ColorTexture, source);
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._FullCoCTexture, m_FullCoCTexture.id);
            cmd.SetRenderTarget(m_MRT2, m_HalfCoCTexture.id, 0, CubemapFace.Unknown, -1);
            
            FrameGraphPostProcessUtils.DrawFullscreenMesh(cmd, material, 1);

            cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);

            // Blur
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._HalfCoCTexture, m_HalfCoCTexture.id);
            FrameGraphPostProcessUtils.Blit(cmd, m_PingTexture.id, m_PongTexture.id, material, 2);
            FrameGraphPostProcessUtils.Blit(cmd, m_PongTexture.id, FrameGraphPostProcessUtils.BlitDstDiscardContent(cmd, m_PingTexture.id), material, 3);

            // Composite
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._ColorTexture, m_PingTexture.id);
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._FullCoCTexture, m_FullCoCTexture.id);
            FrameGraphPostProcessUtils.Blit(cmd, source, FrameGraphPostProcessUtils.BlitDstDiscardContent(cmd, destination), material, 4);
        }

        void PrepareBokehKernel()
        {
            const int kRings = 4;
            const int kPointsPerRing = 7;

            // Check the existing array
            if (m_BokehKernel == null)
                m_BokehKernel = new Vector4[42];

            // Fill in sample points (concentric circles transformed to rotated N-Gon)
            int idx = 0;
            float bladeCount = m_DepthOfField.bladeCount.value;
            float curvature = 1f - m_DepthOfField.bladeCurvature.value;
            float rotation = m_DepthOfField.bladeRotation.value * Mathf.Deg2Rad;
            const float PI = Mathf.PI;
            const float TWO_PI = Mathf.PI * 2f;

            for (int ring = 1; ring < kRings; ring++)
            {
                float bias = 1f / kPointsPerRing;
                float radius = (ring + bias) / (kRings - 1f + bias);
                int points = ring * kPointsPerRing;

                for (int point = 0; point < points; point++)
                {
                    // Angle on ring
                    float phi = 2f * PI * point / points;

                    // Transform to rotated N-Gon
                    // Adapted from "CryEngine 3 Graphics Gems" [Sousa13]
                    float nt = Mathf.Cos(PI / bladeCount);
                    float dt = Mathf.Cos(phi - (TWO_PI / bladeCount) * Mathf.Floor((bladeCount * phi + Mathf.PI) / TWO_PI));
                    float r = radius * Mathf.Pow(nt / dt, curvature);
                    float u = r * Mathf.Cos(phi - rotation);
                    float v = r * Mathf.Sin(phi - rotation);

                    m_BokehKernel[idx] = new Vector4(u, v);
                    idx++;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetMaxBokehRadiusInPixels(float viewportHeight)
        {
            // Estimate the maximum radius of bokeh (empirically derived from the ring count)
            const float kRadiusInPixels = 14f;
            return Mathf.Min(0.05f, kRadiusInPixels / viewportHeight);
        }

        void DoBokehDepthOfField(CommandBuffer cmd, int source, int destination, Rect pixelRect)
        {
            int downSample = 2;
            var material = context.postProcessMaterials.bokehDepthOfField;
            int wh = descriptor.width / downSample;
            int hh = descriptor.height / downSample;

            // "A Lens and Aperture Camera Model for Synthetic Image Generation" [Potmesil81]
            float F = m_DepthOfField.focalLength.value / 1000f;
            float A = m_DepthOfField.focalLength.value / m_DepthOfField.aperture.value;
            float P = m_DepthOfField.focusDistance.value;
            float maxCoC = (A * F) / (P - F);
            float maxRadius = GetMaxBokehRadiusInPixels(descriptor.height);
            float rcpAspect = 1f / (wh / (float) hh);

            cmd.SetGlobalVector(FrameGraphConstant.ShaderConstants._CoCParams, new Vector4(P, maxCoC, maxRadius, rcpAspect));

            // Prepare the bokeh kernel constant buffer
            int hash = m_DepthOfField.GetHashCode();
            if (hash != m_BokehHash)
            {
                m_BokehHash = hash;
                PrepareBokehKernel();
            }

            cmd.SetGlobalVectorArray(FrameGraphConstant.ShaderConstants._BokehKernel, m_BokehKernel);

            // Temporary textures
            cmd.GetTemporaryRT(m_FullCoCTexture.id, FrameGraphPostProcessUtils.GetCompatibleDescriptor(descriptor, descriptor.width, descriptor.height, GraphicsFormat.R8_UNorm), FilterMode.Bilinear);
            cmd.GetTemporaryRT(m_PingTexture.id, FrameGraphPostProcessUtils.GetCompatibleDescriptor(descriptor, wh, hh, GraphicsFormat.R16G16B16A16_SFloat), FilterMode.Bilinear);
            cmd.GetTemporaryRT(m_PongTexture.id, FrameGraphPostProcessUtils.GetCompatibleDescriptor(descriptor, wh, hh, GraphicsFormat.R16G16B16A16_SFloat), FilterMode.Bilinear);

            FrameGraphPostProcessUtils.SetSourceSize(cmd, descriptor);
            cmd.SetGlobalVector(FrameGraphConstant.ShaderConstants._DownSampleScaleFactor, new Vector4(1.0f / downSample, 1.0f / downSample, downSample, downSample));

            // Compute CoC
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._CameraDepthTexture, m_DepthTex.id);
            FrameGraphPostProcessUtils.Blit(cmd, source, m_FullCoCTexture.id, material, 0);
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._FullCoCTexture, m_FullCoCTexture.id);

            // Downscale & prefilter color + coc
            FrameGraphPostProcessUtils.Blit(cmd, source, m_PingTexture.id, material, 1);

            // Bokeh blur
            FrameGraphPostProcessUtils.Blit(cmd, m_PingTexture.id, m_PongTexture.id, material, 2);

            // Post-filtering
            FrameGraphPostProcessUtils.Blit(cmd, m_PongTexture.id, FrameGraphPostProcessUtils.BlitDstDiscardContent(cmd, m_PingTexture.id), material, 3);

            // Composite
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._DofTexture, m_PingTexture.id);
            FrameGraphPostProcessUtils.Blit(cmd, source, FrameGraphPostProcessUtils.BlitDstDiscardContent(cmd, destination), material, 4);
        }

        #endregion
    }
}