using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    internal class CreateRenderTargetPass : RenderTargetServicePass
    {
        const int k_ShadowmapBufferBits = 16;   // 来源：MainLightShadowCasterPass
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
          //  ConfigureTarget(renderTargetHandle.id);
            
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

            
            // 一般情况下的RenderTarget创建
            if (renderTargetInfo.rtType == RenderTargetInfo.RenderTargetType.CustomRT)
            {
                if (!renderTargetInfo.defaultWidth) 
                    descriptor.width = renderTargetInfo.width;
                if (!renderTargetInfo.defaultHeight) 
                    descriptor.height = renderTargetInfo.height;
                if (!renderTargetInfo.defaultColorFormat) 
                    descriptor.colorFormat = renderTargetInfo.colorFormat;
                if (!renderTargetInfo.defaultGraphicsFormat) 
                    descriptor.graphicsFormat = renderTargetInfo.graphicsFormat;
                if (!renderTargetInfo.defaultUseMipmap) 
                    descriptor.useMipMap = renderTargetInfo.useMipMap;
                if (!renderTargetInfo.defaultAutoGenerateMips) 
                    descriptor.autoGenerateMips = renderTargetInfo.autoGenerateMips;
                if (!renderTargetInfo.defaultDepthBufferBits) 
                    descriptor.depthBufferBits = renderTargetInfo.depthBufferBits;

                int downsample = (int)renderTargetInfo.downsampling;
                descriptor.width >>= downsample;
                descriptor.height >>= downsample;
            
                cmd.GetTemporaryRT(renderTargetHandle.id, descriptor, renderTargetInfo.filterMode);
            }
            // 创建URP的ShadowMap
            else if (renderTargetInfo.rtType == RenderTargetInfo.RenderTargetType.URPShadowMap)
            {
                // 参照MainLightShadowCasterPass的Setup中的代码
                var shadowCasterCascadesCount = renderingData.shadowData.mainLightShadowCascadesCount;
                int shadowResolution = ShadowUtils.GetMaxTileResolutionInAtlas(renderingData.shadowData.mainLightShadowmapWidth,
                    renderingData.shadowData.mainLightShadowmapHeight, shadowCasterCascadesCount);
                var shadowmapWidth = renderingData.shadowData.mainLightShadowmapWidth;
                var shadowmapHeight = (shadowCasterCascadesCount == 2) ?
                    renderingData.shadowData.mainLightShadowmapHeight >> 1 :
                    renderingData.shadowData.mainLightShadowmapHeight;
                
                // MainLightShadowCasterPass的渲染目标在Confiure方法中进行创建，逻辑为创建RenderTexture，再用RenderTexture生成RenderTargetIdentifier
                // 这里则直接对已有的RT进行赋值
                
                // ************** 来源：ShadowUtils的静态构造函数
                var shadowmapFormat = RenderingUtils.SupportsRenderTextureFormat(RenderTextureFormat.Shadowmap) && (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2)
                    ? RenderTextureFormat.Shadowmap
                    : RenderTextureFormat.Depth;
                var forceShadowPointSampling = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal &&
                                             GraphicsSettings.HasShaderDefine(Graphics.activeTier, BuiltinShaderDefine.UNITY_METAL_SHADOWS_USE_POINT_FILTERING);
                // *********************************************
           
                descriptor.width = shadowmapWidth;
                descriptor.height = shadowmapHeight;
                descriptor.depthBufferBits = k_ShadowmapBufferBits;
                descriptor.colorFormat = shadowmapFormat;
                descriptor.msaaSamples = 1;
                descriptor.useMipMap = false;
                
                cmd.GetTemporaryRT(renderTargetHandle.id, descriptor, forceShadowPointSampling ? FilterMode.Point: FilterMode.Bilinear);
            }
            // 创建URP的GradingLut
            else if (renderTargetInfo.rtType == RenderTargetInfo.RenderTargetType.URPGradingLut)
            {
                // 参照ColorGradingLutPass的Execute中的代码
                GraphicsFormat hdrLutFormat;
                GraphicsFormat ldrLutFormat;
                
                // Warm up lut format as IsFormatSupported adds GC pressure...
                const FormatUsage kFlags = FormatUsage.Linear | FormatUsage.Render;
                if (SystemInfo.IsFormatSupported(GraphicsFormat.R16G16B16A16_SFloat, kFlags))
                    hdrLutFormat = GraphicsFormat.R16G16B16A16_SFloat;
                else if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, kFlags))
                    hdrLutFormat = GraphicsFormat.B10G11R11_UFloatPack32;
                else
                    // Obviously using this for log lut encoding is a very bad idea for precision but we
                    // need it for compatibility reasons and avoid black screens on platforms that don't
                    // support floating point formats. Expect banding and posterization artifact if this
                    // ends up being used.
                    hdrLutFormat = GraphicsFormat.R8G8B8A8_UNorm;
                ldrLutFormat = GraphicsFormat.R8G8B8A8_UNorm;
                
                ref var postProcessingData = ref renderingData.postProcessingData;
                bool hdr = postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;

                // Prepare texture & material
                int lutHeight = postProcessingData.lutSize;
                int lutWidth = lutHeight * lutHeight;
                var format = hdr ? hdrLutFormat : ldrLutFormat;
                var desc = new RenderTextureDescriptor(lutWidth, lutHeight, format, 0);
                desc.vrUsage = VRTextureUsage.None; // We only need one for both eyes in VR
                desc.msaaSamples = 1;
                desc.useMipMap = false;
                cmd.GetTemporaryRT(renderTargetHandle.id, desc, FilterMode.Bilinear);
            }
        }
    }
}