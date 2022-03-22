using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    /// <summary>
    /// 将PostProcessPass中的方法单独整合进一个工具类中
    /// </summary>
    internal class FrameGraphPostProcessUtils
    {
        public static void SetSourceSize(CommandBuffer cmd, RenderTextureDescriptor desc)
        {
            float width = desc.width;
            float height = desc.height;
            if (desc.useDynamicScale)
            {
                width *= ScalableBufferManager.widthScaleFactor;
                height *= ScalableBufferManager.heightScaleFactor;
            }

            cmd.SetGlobalVector(FrameGraphConstant.ShaderConstants._SourceSize, new Vector4(width, height, 1.0f / width, 1.0f / height));
        }

        public static BuiltinRenderTextureType BlitDstDiscardContent(CommandBuffer cmd, RenderTargetIdentifier rt)
        {
            // We set depth to DontCare because rt might be the source of PostProcessing used as a temporary target
            // Source typically comes with a depth buffer and right now we don't have a way to only bind the color attachment of a RenderTargetIdentifier
            cmd.SetRenderTarget(new RenderTargetIdentifier(rt, 0, CubemapFace.Unknown, -1),
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
            return BuiltinRenderTextureType.CurrentActive;
        }

        public static RenderTextureDescriptor GetCompatibleDescriptor(RenderTextureDescriptor descriptor)
            => GetCompatibleDescriptor(descriptor, descriptor.width, descriptor.height, descriptor.graphicsFormat, descriptor.depthBufferBits);

        public static RenderTextureDescriptor GetCompatibleDescriptor(RenderTextureDescriptor descriptor, int width, int height, GraphicsFormat format, int depthBufferBits = 0)
        {
            var desc = descriptor;
            desc.depthBufferBits = depthBufferBits;
            desc.msaaSamples = 1;
            desc.width = width;
            desc.height = height;
            desc.graphicsFormat = format;
            return desc;
        }

        public static void Blit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, string sourceTexName, int passIndex = 0)
        {
            cmd.SetGlobalTexture(sourceTexName, source);
            cmd.Blit(source, destination, material, passIndex);
        }
        
        public static void Blit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int passIndex = 0)
        {
            cmd.SetGlobalTexture(FrameGraphConstant.ShaderConstants._SourceTex, source);
            // if (m_UseDrawProcedural)
            // {
            //     Vector4 scaleBias = new Vector4(1, 1, 0, 0);
            //     cmd.SetGlobalVector(ShaderPropertyId.scaleBias, scaleBias);
            //
            //     cmd.SetRenderTarget(new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1),
            //         RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
            //     cmd.DrawProcedural(Matrix4x4.identity, material, passIndex, MeshTopology.Quads, 4, 1, null);
            // }
            // else
            //{
            cmd.Blit(source, destination, material, passIndex);
            //}
        }
        
        public static void DrawFullscreenMesh(CommandBuffer cmd, Material material, int passIndex)
        {
            // if (m_UseDrawProcedural)
            // {
            //     Vector4 scaleBias = new Vector4(1, 1, 0, 0);
            //     cmd.SetGlobalVector(ShaderPropertyId.scaleBias, scaleBias);
            //     cmd.DrawProcedural(Matrix4x4.identity, material, passIndex, MeshTopology.Quads, 4, 1, null);
            // }
            // else
            {
                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material, 0, passIndex);
            }
        }
    }
}