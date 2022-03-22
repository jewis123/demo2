using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FrameGraph
{
    internal class FrameGraphConstant
    {
        public static readonly RenderPassEvent defaultRenderTargetServerEvent = RenderPassEvent.BeforeRendering;    // 这个顺序是为了避免在执行创建RT的Pass时会去重置RenderTarget
        public static readonly RenderPassEvent defaultRenderPassEvent = RenderPassEvent.AfterRendering;
        
        public static class ShaderConstants
        {
            public static readonly int _SourceTex = Shader.PropertyToID("_SourceTex");
            public static readonly int _SourceSize = Shader.PropertyToID("_SourceSize");
            
            public static readonly int _CameraDepthTexture    = Shader.PropertyToID("_CameraDepthTexture");
            public static readonly int _ShadowMapTexture    = Shader.PropertyToID("_MainLightShadowmapTexture");
            
            public static readonly int _ColorBalance = Shader.PropertyToID("_ColorBalance");
            public static readonly int _ColorFilter = Shader.PropertyToID("_ColorFilter");
            public static readonly int _ChannelMixerRed = Shader.PropertyToID("_ChannelMixerRed");
            public static readonly int _ChannelMixerGreen = Shader.PropertyToID("_ChannelMixerGreen");
            public static readonly int _ChannelMixerBlue = Shader.PropertyToID("_ChannelMixerBlue");
            public static readonly int _HueSatCon = Shader.PropertyToID("_HueSatCon");
            public static readonly int _Lift = Shader.PropertyToID("_Lift");
            public static readonly int _Gamma = Shader.PropertyToID("_Gamma");
            public static readonly int _Gain = Shader.PropertyToID("_Gain");
            public static readonly int _Shadows = Shader.PropertyToID("_Shadows");
            public static readonly int _Midtones = Shader.PropertyToID("_Midtones");
            public static readonly int _Highlights = Shader.PropertyToID("_Highlights");
            public static readonly int _ShaHiLimits = Shader.PropertyToID("_ShaHiLimits");
            public static readonly int _SplitShadows = Shader.PropertyToID("_SplitShadows");
            public static readonly int _SplitHighlights = Shader.PropertyToID("_SplitHighlights");
            public static readonly int _CurveMaster = Shader.PropertyToID("_CurveMaster");
            public static readonly int _CurveRed = Shader.PropertyToID("_CurveRed");
            public static readonly int _CurveGreen = Shader.PropertyToID("_CurveGreen");
            public static readonly int _CurveBlue = Shader.PropertyToID("_CurveBlue");
            public static readonly int _CurveHueVsHue = Shader.PropertyToID("_CurveHueVsHue");
            public static readonly int _CurveHueVsSat = Shader.PropertyToID("_CurveHueVsSat");
            public static readonly int _CurveLumVsSat = Shader.PropertyToID("_CurveLumVsSat");
            public static readonly int _CurveSatVsSat = Shader.PropertyToID("_CurveSatVsSat");
            
            public static readonly int _TempTarget         = Shader.PropertyToID("_TempTarget");
            public static readonly int _TempTarget2        = Shader.PropertyToID("_TempTarget2");

            public static readonly int _StencilRef         = Shader.PropertyToID("_StencilRef");
            public static readonly int _StencilMask        = Shader.PropertyToID("_StencilMask");

            public static readonly int _FullCoCTexture     = Shader.PropertyToID("_FullCoCTexture");
            public static readonly int _HalfCoCTexture     = Shader.PropertyToID("_HalfCoCTexture");
            public static readonly int _DofTexture         = Shader.PropertyToID("_DofTexture");
            public static readonly int _CoCParams          = Shader.PropertyToID("_CoCParams");
            public static readonly int _BokehKernel        = Shader.PropertyToID("_BokehKernel");
            public static readonly int _PongTexture        = Shader.PropertyToID("_PongTexture");
            public static readonly int _PingTexture        = Shader.PropertyToID("_PingTexture");

            public static readonly int _Metrics            = Shader.PropertyToID("_Metrics");
            public static readonly int _AreaTexture        = Shader.PropertyToID("_AreaTexture");
            public static readonly int _SearchTexture      = Shader.PropertyToID("_SearchTexture");
            public static readonly int _EdgeTexture        = Shader.PropertyToID("_EdgeTexture");
            public static readonly int _BlendTexture       = Shader.PropertyToID("_BlendTexture");

            public static readonly int _ColorTexture       = Shader.PropertyToID("_ColorTexture");
            public static readonly int _Params             = Shader.PropertyToID("_Params");
            public static readonly int _SourceTexLowMip    = Shader.PropertyToID("_SourceTexLowMip");
            public static readonly int _Bloom_Params       = Shader.PropertyToID("_Bloom_Params");
            public static readonly int _Bloom_RGBM         = Shader.PropertyToID("_Bloom_RGBM");
            public static readonly int _Bloom_Texture      = Shader.PropertyToID("_Bloom_Texture");
            public static readonly int _LensDirt_Texture   = Shader.PropertyToID("_LensDirt_Texture");
            public static readonly int _LensDirt_Params    = Shader.PropertyToID("_LensDirt_Params");
            public static readonly int _LensDirt_Intensity = Shader.PropertyToID("_LensDirt_Intensity");
            public static readonly int _Distortion_Params1 = Shader.PropertyToID("_Distortion_Params1");
            public static readonly int _Distortion_Params2 = Shader.PropertyToID("_Distortion_Params2");
            public static readonly int _Chroma_Params      = Shader.PropertyToID("_Chroma_Params");
            public static readonly int _Vignette_Params1   = Shader.PropertyToID("_Vignette_Params1");
            public static readonly int _Vignette_Params2   = Shader.PropertyToID("_Vignette_Params2");
            public static readonly int _Lut_Params         = Shader.PropertyToID("_Lut_Params");
            public static readonly int _UserLut_Params     = Shader.PropertyToID("_UserLut_Params");
            public static readonly int _InternalLut        = Shader.PropertyToID("_InternalLut");
            public static readonly int _UserLut            = Shader.PropertyToID("_UserLut");
            public static readonly int _DownSampleScaleFactor = Shader.PropertyToID("_DownSampleScaleFactor");

            public static readonly int _FullscreenProjMat  = Shader.PropertyToID("_FullscreenProjMat");

            public static int[] _BloomMipUp;
            public static int[] _BloomMipDown;
        }
    }
}