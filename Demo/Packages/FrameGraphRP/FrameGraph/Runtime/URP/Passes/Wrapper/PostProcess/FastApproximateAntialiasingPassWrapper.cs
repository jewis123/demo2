using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// 从PostProcessPass中剥离出来的动态模糊部分
    /// </summary>
    public class FastApproximateAntialiasingPassWrapper : BasePostProcessPass
    {
        
        public FastApproximateAntialiasingPassWrapper(URPContext context, BasePostProcessPass.Settings settings) : base(context, settings, "FXAA")
        {

        }

        protected override void OnProcess(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            RenderFinalPass(cmd, ref renderingData);
        }

        protected override bool OnSetup(int[] inputs, int[] outputs, ref RenderingData renderingData)
        {
            return renderingData.cameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing;
        }

        bool RequireSRGBConversionBlitToBackBuffer(CameraData cameraData)
        {
            return Display.main.requiresSrgbBlitToBackbuffer;
        }
        
        #region Final pass

        void RenderFinalPass(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var material = context.postProcessMaterials.finalPass;
            material.shaderKeywords = null;

            // FXAA setup
            material.EnableKeyword(ShaderKeywordStrings.Fxaa);

            FrameGraphPostProcessUtils.SetSourceSize(cmd, cameraData.cameraTargetDescriptor);

            if (RequireSRGBConversionBlitToBackBuffer(cameraData))
                material.EnableKeyword(ShaderKeywordStrings.LinearToSRGBConversion);

            cmd.SetGlobalTexture( FrameGraphConstant.ShaderConstants._SourceTex, source.Identifier());

            var colorLoadAction = cameraData.isDefaultViewport ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;

//             RenderTargetHandle cameraTargetHandle = RenderTargetHandle.GetCameraTarget(cameraData.xr);
//
// #if ENABLE_VR && ENABLE_XR_MODULE
//             if (cameraData.xr.enabled)
//             {
//                 RenderTargetIdentifier cameraTarget = cameraTargetHandle.Identifier();
//
//                 //Blit(cmd, m_Source.Identifier(), BuiltinRenderTextureType.CurrentActive, material);
//                 bool isRenderToBackBufferTarget = cameraTarget == cameraData.xr.renderTarget && !cameraData.xr.renderTargetIsRenderTexture;
//                 // We y-flip if
//                 // 1) we are bliting from render texture to back buffer and
//                 // 2) renderTexture starts UV at top
//                 bool yflip = isRenderToBackBufferTarget && SystemInfo.graphicsUVStartsAtTop;
//
//                 Vector4 scaleBias = yflip ? new Vector4(1, -1, 0, 1) : new Vector4(1, 1, 0, 0);
//
//                 cmd.SetRenderTarget(new RenderTargetIdentifier(cameraTarget, 0, CubemapFace.Unknown, -1),
//                     colorLoadAction, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
//                 cmd.SetViewport(cameraData.pixelRect);
//                 cmd.SetGlobalVector(ShaderPropertyId.scaleBias, scaleBias);
//                 cmd.DrawProcedural(Matrix4x4.identity, material, 0, MeshTopology.Quads, 4, 1, null);
//             }
//             else
// #endif
            {
                // // Note: We need to get the cameraData.targetTexture as this will get the targetTexture of the camera stack.
                // // Overlay cameras need to output to the target described in the base camera while doing camera stack.
                // RenderTargetIdentifier cameraTarget = (cameraData.targetTexture != null) ? new RenderTargetIdentifier(cameraData.targetTexture) : cameraTargetHandle.Identifier();

                cmd.SetRenderTarget(destination.id, colorLoadAction, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
                cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                cmd.SetViewport(cameraData.camera.pixelRect);
                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material);
                cmd.SetViewProjectionMatrices(cameraData.camera.worldToCameraMatrix, cameraData.camera.projectionMatrix);
            }
        }

        #endregion
    }
}