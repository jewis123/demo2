/***************************************************************************************************
* 
* OutlinePass
*
***************************************************************************************************/


#ifndef __STANDARD_OUTLINEPASS_HLSL__
#define __STANDARD_OUTLINEPASS_HLSL__

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/StandardInput.hlsl"

struct Attributes
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 texcoord0 : TEXCOORD0;
    float4 color : COLOR;
};

struct Varyings
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 vertex_color : TEXCOORD1;
};

Varyings OutlinePassVertex(Attributes v)
{
    Varyings o = (Varyings)0;
    float3 normal_world = TransformObjectToWorldNormal(v.normal);
    float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
    float3 pos_view = TransformWorldToView(worldPos);
    float3 outline_dir = normalize(mul((float3x3)UNITY_MATRIX_V, normal_world));
    
    pos_view = pos_view + outline_dir * _OutlineWidth * 0.001 * v.color.r;
    o.pos = mul(UNITY_MATRIX_P, float4(pos_view, 1.0));
    o.uv = v.texcoord0.xy;
    o.vertex_color = v.color;
    return o;
}

float4 OutlinePassFragment(Varyings i) : COLOR
{
    float3 basecolor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).rgb;
    half maxComponent = max(max(basecolor.r, basecolor.g), basecolor.b);
    half3 saturatedColor = step(maxComponent.rrr, basecolor) * basecolor;
    saturatedColor = lerp(basecolor.rgb, saturatedColor, 0.6);
    half3 outlineColor = lerp(saturatedColor, _OutlineColor.xyz, _OutlineLerpFactor);
    return float4(outlineColor, 1.0);
}

#endif	//__STANDARD_OUTLINEPASS_HLSL__