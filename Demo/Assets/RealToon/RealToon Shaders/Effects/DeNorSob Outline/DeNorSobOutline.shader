//RealToon - DeNorSob Outline Effect (URP - Post Processing)
//MJQStudioWorks
//2021

Shader  "Hidden/URP/RealToon/Effects/DeNorSobOutline"
{

    Properties
    {
        _OutlineWidth("Outline Width", Float) = 1.0

        _DepthThreshold("Depth Threshold", Float) = 900.0

        _NormalThreshold("Normal Threshold", Float) = 1.3
        _NormalMin("Normal Min", Float) = 1.0
        _NormalMax("Normal Max", Float) = 1.0

        _SobOutSel("Sobel Outline", Float) = 0.0
        _SobelOutlineThreshold(" Sobel Outline Threshold", Float) = 50.0
        _WhiThres("Black Threshold", Float) = 0.0
        _BlaThres("White Threshold", Float) = 0.0

        _OutlineColor("Outline Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _OutlineColorIntensity("Outline Color Intensity", Float) = 0.0
        _ColOutMiSel("Mix Full Screen Color", Float) = 0.0

        _OutOnSel("Show Outline Only", Float) = 0.0
        _MixDeNorSob("Mix Deph Normal And Sobel Outline", Float) = 0.0
    }

    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

    #pragma shader_feature_local RENDER_OUTLINE_ALL
    #pragma shader_feature_local MIX_DENOR_SOB

    TEXTURE2D(_CameraColorTexture);
    SAMPLER(sampler_CameraColorTexture);

    TEXTURE2D(_CameraDepthTexture);
    SAMPLER(sampler_CameraDepthTexture);

    TEXTURE2D(_CameraDepthNormalsTexture);
    SAMPLER(sampler_CameraDepthNormalsTexture);

    float _OutlineWidth;

    float _DepthThreshold;
    float _NormalThreshold;
    float _NormalMin;
    float _NormalMax;

    float _SobOutSel;
    float _SobelOutlineThreshold;
    float _WhiThres;
    float _BlaThres;

    float3 _OutlineColor;
    float _OutlineColorIntensity;
    float _ColOutMiSel;

    float _OutOnSel;
    float _MixDeNorSob;

    float SamDep(float2 uv)
    {
        float output = (float)min(max(SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv), _BlaThres), _WhiThres);

        return output;
    }

    float sob_fil(float CX, float2 uv)
    {
        float2 d = float2(CX, CX);

        float hr = 0;
        float vt = 0;

        hr += SamDep(uv + float2(-1.0, -1.0) * d) * 1.0;
        hr += SamDep(uv + float2(1.0, -1.0) * d) * -1.0;
        hr += SamDep(uv + float2(-1.0, 0.0) * d) * 2.0;
        hr += SamDep(uv + float2(1.0, 0.0) * d) * -2.0;
        hr += SamDep(uv + float2(-1.0, 1.0) * d) * 1.0;
        hr += SamDep(uv + float2(1.0, 1.0) * d) * -1.0;

        vt += SamDep(uv + float2(-1.0, -1.0) * d) * 1.0;
        vt += SamDep(uv + float2(0.0, -1.0) * d) * 2.0;
        vt += SamDep(uv + float2(1.0, -1.0) * d) * 1.0;
        vt += SamDep(uv + float2(-1.0, 1.0) * d) * -1.0;
        vt += SamDep(uv + float2(0.0, 1.0) * d) * -2.0;
        vt += SamDep(uv + float2(1.0, 1.0) * d) * -1.0;

        return sqrt(dot(hr , hr) + dot(vt , vt));

    }

    float3 DecodeNormal(float4 enc)
    {
        float kScale = 1.7777;
        float3 nn = enc.xyz * float3(2 * kScale, 2 * kScale, 0) + float3(-kScale, -kScale, 1);
        float g = 2.0 / dot(nn.xyz, nn.xyz);
        float3 n;
        n.xy = g * nn.xy;
        n.z = g - 1;
        return n;
    }

    float EdgeDetect(float2 uv, float4 input_pos_cs)
    {

        float2 _ScreenSize = (1.0) / float2(_ScreenParams.r, _ScreenParams.g);

        float halfScaleFloor = floor(_OutlineWidth * 0.5);
        float halfScaleCeil = ceil(_OutlineWidth * 0.5);

        float2 bottomLeftUV = uv - float2(_ScreenSize.x, _ScreenSize.y) * halfScaleFloor;
        float2 topRightUV = uv + float2(_ScreenSize.x, _ScreenSize.y) * halfScaleCeil;
        float2 bottomRightUV = uv + float2(_ScreenSize.x * halfScaleCeil, -_ScreenSize.y * halfScaleFloor);
        float2 topLeftUV = uv + float2(-_ScreenSize.x * halfScaleFloor, _ScreenSize.y * halfScaleCeil);

        float depth0 = (float)SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, bottomLeftUV);
        float depth1 = (float)SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, topRightUV);
        float depth2 = (float)SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, bottomRightUV);
        float depth3 = (float)SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, topLeftUV);

        float depthDerivative0 = depth1 - depth0;
        float depthDerivative1 = depth3 - depth2;

        float edgeDepth = sqrt(pow(depthDerivative0, 2.0) + pow(depthDerivative1, 2.0)) * 100;
        edgeDepth = edgeDepth > (depth0 * (_DepthThreshold * 0.01)) ? 1 : 0;

        float3 normalData0 = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, bottomLeftUV));
        float3 normalData1 = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, topRightUV));
        float3 normalData2 = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, bottomRightUV));
        float3 normalData3 = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, topLeftUV));

        float3 normalFiniteDifference0 = (normalData1 - normalData0);
        float3 normalFiniteDifference1 = (normalData3 - normalData2);

        float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
        edgeNormal = smoothstep(_NormalMin, _NormalMax, edgeNormal * _NormalThreshold);

        float edgeSob = sob_fil(_OutlineWidth * 0.00004, uv) > ((_SobelOutlineThreshold * 0.01) * SamDep(uv)) ? 1 : 0;
        edgeSob *= (float)SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv) != UNITY_RAW_FAR_CLIP_VALUE;

        #ifndef MIX_DENOR_SOB

            #ifdef RENDER_OUTLINE_ALL
                return edgeSob;
            #else
                return max(edgeDepth, edgeNormal);
            #endif

        #else

            #ifdef RENDER_OUTLINE_ALL
                float edgeSob_Mix = edgeSob;
            #else
                float edgeSob_Mix = 0.0;
            #endif

                return max(edgeDepth, max(edgeNormal, edgeSob_Mix));

        #endif

    }

    half4 Frag(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float3 ful_scr_so = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, input.uv).rgb;

        float denorsobOut = EdgeDetect(input.uv, input.positionCS);

        float3 coloutmix = lerp(_OutlineColor * _OutlineColorIntensity, lerp(ful_scr_so * ful_scr_so, ful_scr_so , _OutlineColorIntensity) * _OutlineColor, _ColOutMiSel);
        return float4( lerp( coloutmix, lerp( ful_scr_so , 1.0, _OutOnSel ) , (1.0 - denorsobOut) ) ,1.0);

    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
            LOD 100
            ZTest Always ZWrite Off Cull Off

            Pass
        {
            Name "DeNorSob_Outline"

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
            ENDHLSL
        }
    }

}
