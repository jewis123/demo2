/***************************************************************************************************
 * 
 * 提供 Debug 工具
 *
 ***************************************************************************************************/


#ifndef __TA_UTILS_DEBUG_HLSL__
#define __TA_UTILS_DEBUG_HLSL__




 /*********************************
 * Debug Show Color Params
 * Possible Defines:
 *      _DIFFUSE_ON
 *      _SPECULAR_ON
 *      _INDIFFUSE_ON
 *      _INSPECULAR_ON
 *      _AO_ON
 *********************************/


half3 ColorRange(half3 color)
{
    half brightness = max(max(color.r, color.g), color.b);
    if (brightness < 0.5)   return half3(1, 0, 0.5);
    else if (0.5 <= brightness && brightness < 1.0) return half3(1, 0, 1);
    else if (1.0 <= brightness && brightness < 1.5) return half3(0.5, 0, 1);
    else if (1.5 <= brightness && brightness < 2.0) return half3(0, 0, 1);
    else if (2.0 <= brightness && brightness < 2.5) return half3(0, 1, 1);
    else if (2.5 <= brightness && brightness < 3.0) return half3(0, 1, 0);
    else if (3.0 <= brightness && brightness < 3.5) return half3(0.5, 1, 0);
    else if (3.5 <= brightness && brightness < 4.0) return half3(1, 1, 0);
    else if (4.0 <= brightness && brightness < 4.5) return half3(1, 0.5, 0);
    else if (4.5 <= brightness && brightness < 5.0) return half3(1, 0, 0);
    else return half3(1, 1, 1);
}


int MipmapIndex(half2 texcoord, float4 _DiffuseTex_TexelSize)
{
    float2 uv = texcoord * _DiffuseTex_TexelSize.zw;
    float2 dx = ddx(uv);
    float2 dy = ddy(uv);
    float rho = max(sqrt(dot(dx, dx)), sqrt(dot(dy, dy)));
    float lambda = log2(rho);
    return max(int(lambda + 0.5), 0);
}

#define ADD_COLOR_TO_DEBUG(color)   _debug_color.xyz += (color).xyz;  _debug_flag = 1;


#if _DIRECT_DIFFUSE_SPECULAR_ON
    #define _DIFFUSE_ON 1
    #define _SPECULAR_ON     1
#endif

#if _INDIRECT_DIFFUSE_SPECULAR_ON
    #define _INDIFFUSE_ON 1
    #define _INSPECULAR_ON     1
#endif


#ifdef _DIFFUSE_INDIFFUSE_ON
    #define _DIFFUSE_ON     1
    #define _INDIFFUSE_ON   1
#endif

#ifdef _SPECULAR_IBL_ON
    #define _SPECULAR_ON     1
    #define _INSPECULAR_ON   1
#endif


#ifdef _DIFFUSE_ON
    #define DEBUG_DIFFUSE_IF_ON(color)              ADD_COLOR_TO_DEBUG(color)
#else
    #define DEBUG_DIFFUSE_IF_ON(color)
#endif



#ifdef _SPECULAR_ON
    #define DEBUG_SPECULAR_IF_ON(color)             ADD_COLOR_TO_DEBUG(color)
#else
    #define DEBUG_SPECULAR_IF_ON(color)
#endif



#ifdef _INDIFFUSE_ON
    #define DEBUG_IDIFFUSE_IF_ON(color)             ADD_COLOR_TO_DEBUG(color)
#else
    #define DEBUG_IDIFFUSE_IF_ON(color)
#endif



#ifdef _INSPECULAR_ON
    #define DEBUG_INSPECULAR_IF_ON(color)           ADD_COLOR_TO_DEBUG(color)
#else
    #define DEBUG_INSPECULAR_IF_ON(color)
#endif

#ifdef _LM_ON
    #define DEBUG_LIGHTMAP_IF_ON(color)             ADD_COLOR_TO_DEBUG(color);
#else
    #define DEBUG_LIGHTMAP_IF_ON(color)             _debug_color.xyz += (color).xyz * 0.0001; // not set flag
#endif



#ifdef _AO_ON
    #define DEBUG_AO(AO)        return AO;
#else
    #define DEBUG_AO(AO)
#endif


#ifdef _SPECULAR_RANGE_ON
    #define DEBUG_SPECULAR_RANGE_IF_ON(color)  ADD_COLOR_TO_DEBUG(ColorRange(color.xyz));
#else
    #define DEBUG_SPECULAR_RANGE_IF_ON(color)
#endif




#define DEBUG_COLOR_BEGIN(finalColor, lightMapColor, diffuse, specular, indirectDiffuse, indirectSpecular, AO) \
            float _debug_flag = 0;                      \
            float3 _debug_color = 0;                    \
            DEBUG_DIFFUSE_IF_ON(diffuse)                \
            DEBUG_SPECULAR_IF_ON(specular)              \
            DEBUG_IDIFFUSE_IF_ON(indirectDiffuse)       \
            DEBUG_INSPECULAR_IF_ON(indirectSpecular)    \
            DEBUG_LIGHTMAP_IF_ON(lightMapColor)         \
            DEBUG_AO(AO)                                \
            DEBUG_SPECULAR_RANGE_IF_ON(finalColor)


#define DEBUG_COLOR_END    if (_debug_flag > 0.5) return float4(_debug_color, 1);



#endif