using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    public class SubpixelMorphologicalAntialiasingPassWrapper : BasePostProcessPass
    {
        private RenderTargetHandle m_EdgeTexture;
        private RenderTargetHandle m_BlendTexture;
        private RenderTargetHandle m_DepthTex;
        
        public SubpixelMorphologicalAntialiasingPassWrapper(URPContext context, Settings settings) : base(context, settings, "SMAA")
        {
        }

        protected override void OnProcess(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            DoSubpixelMorphologicalAntialiasing(ref renderingData.cameraData, cmd, source.id, destination.id);
        }

        protected override bool OnSetup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            m_DepthTex = inputs[1].ToRenderTargetHandle();
            
            m_EdgeTexture = outputs[1].ToRenderTargetHandle();
            m_BlendTexture = outputs[2].ToRenderTargetHandle();
            
            return renderingData.cameraData.antialiasing == AntialiasingMode.SubpixelMorphologicalAntiAliasing && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2;
        }

        #region Sub-pixel Morphological Anti-aliasing

        void DoSubpixelMorphologicalAntialiasing(ref CameraData cameraData, CommandBuffer cmd, int source, int destination)
        {
            var camera = cameraData.camera;
            var pixelRect = camera.pixelRect;
            var material = context.postProcessMaterials.subpixelMorphologicalAntialiasing;
            const int kStencilBit = 64;

            // Globals
            material.SetVector(FrameGraphConstant.ShaderConstants._Metrics, new Vector4(1f / descriptor.width, 1f / descriptor.height, descriptor.width, descriptor.height));
            material.SetTexture(FrameGraphConstant.ShaderConstants._AreaTexture, context.data.postProcessData.textures.smaaAreaTex);
            material.SetTexture(FrameGraphConstant.ShaderConstants._SearchTexture, context.data.postProcessData.textures.smaaSearchTex);
            material.SetInt(FrameGraphConstant.ShaderConstants._StencilRef, kStencilBit);
            material.SetInt(FrameGraphConstant.ShaderConstants._StencilMask, kStencilBit);

            // Quality presets
            material.shaderKeywords = null;

            switch (cameraData.antialiasingQuality)
            {
                case AntialiasingQuality.Low:
                    material.EnableKeyword(ShaderKeywordStrings.SmaaLow);
                    break;
                case AntialiasingQuality.Medium:
                    material.EnableKeyword(ShaderKeywordStrings.SmaaMedium);
                    break;
                case AntialiasingQuality.High:
                    material.EnableKeyword(ShaderKeywordStrings.SmaaHigh);
                    break;
            }

            // Intermediate targets
            RenderTargetIdentifier stencil; // We would only need stencil, no depth. But Unity doesn't support that.
            int tempDepthBits;
            if (m_DepthTex == RenderTargetHandle.CameraTarget || descriptor.msaaSamples > 1)
            {
                // In case m_Depth is CameraTarget it may refer to the backbuffer and we can't use that as an attachment on all platforms
                stencil = FrameGraphConstant.ShaderConstants._EdgeTexture;
                tempDepthBits = 24;
            }
            else
            {
                stencil = m_DepthTex.Identifier();
                tempDepthBits = 0;
            }

            // cmd.GetTemporaryRT(FrameGraphConstant.ShaderConstants._EdgeTexture, FrameGraphPostProcessUtils.GetCompatibleDescriptor(descriptor, descriptor.width, descriptor.height, m_SMAAEdgeFormat, tempDepthBits), FilterMode.Bilinear);
            // cmd.GetTemporaryRT(FrameGraphConstant.ShaderConstants._BlendTexture, FrameGraphPostProcessUtils.GetCompatibleDescriptor(descriptor, descriptor.width, descriptor.height, GraphicsFormat.R8G8B8A8_UNorm), FilterMode.Point);

            // Prepare for manual blit
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.SetViewport(pixelRect);

            // Pass 1: Edge detection
            cmd.SetRenderTarget(m_EdgeTexture.Identifier(),
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, stencil,
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            cmd.ClearRenderTarget(true, true, Color.clear);
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._ColorTexture, source);
            FrameGraphPostProcessUtils.DrawFullscreenMesh(cmd, material, 0);

            // Pass 2: Blend weights
            cmd.SetRenderTarget(m_BlendTexture.Identifier(),
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, stencil,
                RenderBufferLoadAction.Load, RenderBufferStoreAction.DontCare);
            cmd.ClearRenderTarget(false, true, Color.clear);
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._ColorTexture, m_EdgeTexture.Identifier());
            FrameGraphPostProcessUtils.DrawFullscreenMesh(cmd, material, 1);

            // Pass 3: Neighborhood blending
            cmd.SetRenderTarget(new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1),
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._ColorTexture, source);
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._BlendTexture, m_BlendTexture.Identifier());
            FrameGraphPostProcessUtils.DrawFullscreenMesh(cmd, material, 2);

            // Cleanup
            // cmd.ReleaseTemporaryRT(FrameGraphConstant.ShaderConstants._EdgeTexture);
            // cmd.ReleaseTemporaryRT(FrameGraphConstant.ShaderConstants._BlendTexture);
            cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
        }

        #endregion
    }
}