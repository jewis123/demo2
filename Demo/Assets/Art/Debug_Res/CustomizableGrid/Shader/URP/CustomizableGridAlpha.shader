Shader "Universal Render Pipeline/YMT/CustomizableGridAlpha"
{
    Properties
    {
        //__________________________________________[User editable section]__________________________________________\\
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //-Write all per material settings here, just like a regular .shader.
        //-In order to make SRP batcher compatible,
        //make sure to match all uniforms inside CBUFFER_START(UnityPerMaterial) in the next [User editable section]
        
        //below are just some example use case Properties, you can write whatever you want here
        [Header(BaseColor)]
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        //custom data
        
        _TilingX ("Global Tiling X", Float) = 1
        _TilingY ("Global Tiling Y", Float) = 1
        [Toggle] _WorldPosition("World Position UV", Float) = 0
        _GlobalMet("Global Metallic", Range(-1, 2)) = 1
        _GlobalSmo("Global Smoothness", Range(-1, 2)) = 1
        _GlobalEmi("Global Emission", Range(0, 2)) = 1
        _BasePatternColor ("Base Pattern Color", Color) = (0.8,0.8,0.8,1)
        _BasePatternTex ("Base Patter Texture", 2D) = "black" {}
        _BasePatternMet ("Metallic BP", Range (-1, 1)) = 0
        _BasePatternSmo ("Smoothness BP", Range (0, 1)) = 0.3
        _BasePatternEmi ("Emission BP", Range (0, 10)) = 0
        _CenterGridColor ("Center Grid Color", Color) = (0.7,0.7,0.7,1)
        _CenterGridTex ("Center Grid Texture", 2D) = "black" {}
        _CenterGridMet ("Metallic CG", Range (-1, 1)) = 0
        _CenterGridSmo ("Smoothness CG", Range (0, 1)) = 0.3
        _CenterGridEmi ("Emission CG", Range (0, 10)) = 0
        _CenterLineColor ("Center Line Color", Color) = (0.6,0.6,0.6,1)
        _CenterLineTex ("Center Line Texture", 2D) = "black" {}
        _CenterLineMet ("Metallic CL", Range (-1, 1)) = 0
        _CenterLineSmo ("Smoothness CL", Range (0, 1)) = 0.3
        _CenterLineEmi ("Emission CL", Range (0, 10)) = 0
        _EdgeColor ("Edge Color", Color) = (0.9,0.9,0.9,1)
        _EdgeTex ("Edge Texture", 2D) = "black" {}
        _EdgeMet ("Metallic Edge", Range (-1, 1)) = 0
        _EdgeSmo ("Smoothness Edge", Range (0, 1)) = 0.3
        _EdgeEmi ("Emission Edge", Range (0, 10)) = 0
        _TextColor ("Text Color", Color) = (0.9,0.9,0.9,1)
        _TextTex ("Text Texture", 2D) = "black" {}
        _TextMet ("Metallic Text", Range (-1, 1)) = 0
        _TextSmo ("Smoothness Text", Range (0, 1)) = 0.3
        _TextEmi ("Emission Text", Range (0, 10)) = 0
        _WolrdBpattTiling("BP WTiling", Vector) = (1,1,0,0)
        _WolrdCgridTiling("CG WTiling", Vector) = (1,1,0,0)
        _WolrdClineTiling("CL WTiling", Vector) = (1,1,0,0)
        _WolrdEdgeTiling("Edge Tiling", Vector) = (1,1,0,0)
        _WolrdTextTiling("Text Tiling", Vector) = (1,1,0,0)
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

    HLSLINCLUDE

    //this section are multi_compile keywords set by unity:
    //-Sadly there seems to be no way to hide #pragma from user, 
    // so multi_compile must be copied to every .shader due to shaderlab's design,
    // which makes updating this section in future almost impossible once users already produced lots of .shader files
    //-The good part is exposing multi_compiles which makes editing by user possible, 
    // but it contradict with the goal of surface shader - "hide lighting implementation from user"
    //==================================================================================================================
    //copied URP multi_compile note from Felipe Lira's UniversalPipelineTemplateShader.shader
    //https://gist.github.com/phi-lira/225cd7c5e8545be602dca4eb5ed111ba

    // Universal Render Pipeline keywords
    // When doing custom shaders you most often want to copy and paste these #pragmas,
    // These multi_compile variants are stripped from the build depending on:
    // 1) Settings in the URP Asset assigned in the GraphicsSettings at build time
    // e.g If you disable AdditionalLights in the asset then all _ADDITIONA_LIGHTS variants
    // will be stripped from build
    // 2) Invalid combinations are stripped. e.g variants with _MAIN_LIGHT_SHADOWS_CASCADE
    // but not _MAIN_LIGHT_SHADOWS are invalid and therefore stripped.

    //100% copied from URP PBR shader graph's generated code
    // Pragmas
    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    #pragma target 2.0
    #pragma multi_compile_fog
    #pragma multi_compile_instancing

    //100% copied from URP PBR shader graph's generated code
    // Keywords
    #pragma multi_compile _ LIGHTMAP_ON
    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
    #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
    #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
    #pragma multi_compile _ _SHADOWS_SOFT
    #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE



	
    
    //==================================================================================================================


    //the core .hlsl of the whole URP surface shader structure, must be included
    #include "../../../URP/Surface/Core/NiloURPSurfaceShaderInclude.hlsl"


    //__________________________________________[User editable section]__________________________________________\\
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //first, select a lighting function = a .hlsl which contains the concrete body of CalculateSurfaceFinalResultColor(...)
    //you can select any .hlsl you want here, default is NiloPBRLitCelShadeLightingFunction.hlsl, you can always change it
    
    // #include "../LightingFunctionLibrary/NiloPBRLitCelShadeLightingFunction.hlsl"
    #include "../../../URP/Surface/LightingFunctionLibrary/NiloPBRLitLightingFunction.hlsl"

    
    #pragma shader_feature _NORMALMAP


    #pragma multi_compile WPOS_ON WPOS_OFF
    

    //define texture & sampler as usual
    
	TEXTURE2D(_BasePatternTex);
    SAMPLER(sampler_BasePatternTex);

    TEXTURE2D(_CenterGridTex);
    SAMPLER(sampler_CenterGridTex);

    TEXTURE2D(_CenterLineTex);
    SAMPLER(sampler_CenterLineTex);

    TEXTURE2D(_EdgeTex);
    SAMPLER(sampler_EdgeTex);


    TEXTURE2D(_TextTex);
    SAMPLER(sampler_TextTex);
    

    
    CBUFFER_START(UnityPerMaterial)
    float4 _BasePatternTex_ST;
	float4 _CenterGridTex_ST;
	float4 _CenterLineTex_ST;
	float4 _EdgeTex_ST;
	float4 _TextTex_ST;

    
    half4 _BaseColor;

    float4 _BasePatternColor;
	half _BasePatternMet;
	half _BasePatternSmo;
	half _BasePatternEmi;

	float4 _CenterGridColor;
	half _CenterGridMet;
	half _CenterGridSmo;
	half _CenterGridEmi;

	float4 _CenterLineColor;
	half _CenterLineMet;
	half _CenterLineSmo;
	half _CenterLineEmi;

	float4 _EdgeColor;
	half _EdgeMet;
	half _EdgeSmo;
	half _EdgeEmi;

	float4 _TextColor;
	half _TextMet;
	half _TextSmo;
	half _TextEmi;
    
	half _TilingX;
	half _TilingY;

	half _GlobalMet;
	half _GlobalSmo;
	half _GlobalEmi;

	half4 _WolrdBpattTiling;
	half4 _WolrdCgridTiling;
	half4 _WolrdClineTiling;
	half4 _WolrdEdgeTiling;
	half4 _WolrdTextTiling;

	bool _WorldPosition;



    
    CBUFFER_END
    
    void UserGeometryDataOutputFunction(Attributes IN, inout UserGeometryOutputData geometryOutputData, bool isExtraCustomPass)
    {
    }
    
    /*
    //100% same as URP PBR shader graph's fragment input
    struct UserSurfaceOutputData
    {
        half3   albedo;             
        half3   normalTS;          
        half3   emission;     
        half    metallic;
        half    smoothness;
        half    occlusion;                
        half    alpha;          
        half    alphaClipThreshold;
    };
    */
    void UserSurfaceOutputDataFunction(Varyings IN, inout UserSurfaceOutputData surfaceData, bool isExtraCustomPass)
    {
	    float2 overT = float2(_TilingX,_TilingY);

    	float2 UV_BP = TRANSFORM_TEX(IN.uv,_BasePatternTex) * overT;
    	float2 UV_CG = TRANSFORM_TEX(IN.uv,_CenterGridTex) * overT;
    	float2 UV_CL = TRANSFORM_TEX(IN.uv,_CenterLineTex) * overT;
    	float2 UV_ED = TRANSFORM_TEX(IN.uv,_EdgeTex) * overT;
    	float2 UV_TX = TRANSFORM_TEX(IN.uv,_TextTex) * overT;
    	




    	#if WPOS_ON
				
    	if (abs(IN.normalWS.x)>0.5) {// side
    		UV_BP = (IN.positionWSAndFogFactor.zy * overT * _WolrdBpattTiling.xy) + _WolrdBpattTiling.zw;
    		UV_CG = (IN.positionWSAndFogFactor.zy * overT * _WolrdCgridTiling.xy) + _WolrdCgridTiling.zw;
    		UV_CL = (IN.positionWSAndFogFactor.zy * overT * _WolrdClineTiling.xy) + _WolrdClineTiling.zw;
    		UV_ED = (IN.positionWSAndFogFactor.zy * overT * _WolrdEdgeTiling.xy) + _WolrdEdgeTiling.zw;
    		UV_TX = (IN.positionWSAndFogFactor.zy * overT * _WolrdTextTiling.xy) + _WolrdTextTiling.zw;
					
    	}
    	else if (abs(IN.normalWS.z)>0.5) {// front
    		UV_BP = (IN.positionWSAndFogFactor.xy * overT * _WolrdBpattTiling.xy) + _WolrdBpattTiling.zw;
    		UV_CG = (IN.positionWSAndFogFactor.xy * overT * _WolrdCgridTiling.xy) + _WolrdCgridTiling.zw;
    		UV_CL = (IN.positionWSAndFogFactor.xy * overT * _WolrdClineTiling.xy) + _WolrdClineTiling.zw;
    		UV_ED = (IN.positionWSAndFogFactor.xy * overT * _WolrdEdgeTiling.xy) + _WolrdEdgeTiling.zw;
    		UV_TX = (IN.positionWSAndFogFactor.xy * overT * _WolrdTextTiling.xy) + _WolrdTextTiling.zw;
    	}
    	else { // top
    		UV_BP = (IN.positionWSAndFogFactor.xz * overT * _WolrdBpattTiling.xy) + _WolrdBpattTiling.zw;
    		UV_CG = (IN.positionWSAndFogFactor.xz * overT * _WolrdCgridTiling.xy) + _WolrdCgridTiling.zw;
    		UV_CL = (IN.positionWSAndFogFactor.xz * overT * _WolrdClineTiling.xy) + _WolrdClineTiling.zw;
    		UV_ED = (IN.positionWSAndFogFactor.xz * overT * _WolrdEdgeTiling.xy) + _WolrdEdgeTiling.zw;
    		UV_TX = (IN.positionWSAndFogFactor.xz * overT * _WolrdTextTiling.xy) + _WolrdTextTiling.zw;
    	}

    	#endif
    	real4 colBaseP = SAMPLE_TEXTURE2D (_BasePatternTex,sampler_BasePatternTex, UV_BP);
    	real4 colCGrid = SAMPLE_TEXTURE2D (_CenterGridTex, sampler_CenterGridTex,UV_CG);
    	real4 colCLine = SAMPLE_TEXTURE2D (_CenterLineTex,sampler_CenterLineTex, UV_CL);
    	real4 colEdge = SAMPLE_TEXTURE2D (_EdgeTex, sampler_EdgeTex,UV_ED);
    	real4 colText = SAMPLE_TEXTURE2D (_TextTex, sampler_TextTex,UV_TX);

    	real4 colFinal =  ((((_BaseColor * (1 - colBaseP) + (colBaseP * _BasePatternColor)) * 
							(1 - colCGrid) + (colCGrid * _CenterGridColor)) * 
							(1 - colCLine) + (colCLine * _CenterLineColor)) * 
							(1 - colEdge) + (colEdge * _EdgeColor)) * 
							(1 - colText) + (colText * _TextColor);

    	real4 metBaseP = _BasePatternMet > 0 ? _BasePatternMet * colBaseP : - _BasePatternMet * (1 - colBaseP);
    	real4 metCGrid = _CenterGridMet > 0 ? _CenterGridMet * colCGrid : - _CenterGridMet * (1 - colCGrid);
    	real4 metCLine = _CenterLineMet > 0 ? _CenterLineMet * colCLine : - _CenterLineMet * (1 - colCLine);
    	real4 metEdge = _EdgeMet > 0 ? _EdgeMet * colEdge : - _EdgeMet * (1 - colEdge);
    	real4 metText = _TextMet > 0 ? _TextMet * colText : - _TextMet * (1 - colText);

    	real4 metFinal = metBaseP + metCGrid + metCLine + metEdge + metText;
    	real4 smoFinal = (metBaseP * _BasePatternSmo) + (metCGrid * _CenterGridSmo) + (metCLine * _CenterLineSmo) +
					(metEdge * _EdgeSmo) + ( metText * _TextSmo);

    	real4 emiFinal = (colBaseP * _BasePatternColor * _BasePatternEmi) +
				(colCGrid * _CenterGridColor * _CenterGridEmi) + 
				(colCLine * _CenterLineColor * _CenterLineEmi) + 
				(colEdge * _EdgeColor * _EdgeEmi) + 
				(colText * _TextColor * _TextEmi);

    	surfaceData.albedo = colFinal.rgb;
    	surfaceData.metallic = metFinal.x * _GlobalMet;
    	surfaceData.smoothness = smoFinal.x * _GlobalSmo;
    	surfaceData.emission = emiFinal.x * _GlobalEmi;
    	surfaceData.alpha = 1;
    	
    	if(isExtraCustomPass)
    	{
    		//make outline pass darker
    		surfaceData.albedo = 0;
    		surfaceData.smoothness = 0;
    		surfaceData.metallic = 0;
    		surfaceData.occlusion = 0;
    	}
    }
    
    void FinalPostProcessFrag(Varyings IN, UserSurfaceOutputData surfaceData, LightingData lightingData, inout half4 inputColor)
    {
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ENDHLSL

    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalRenderPipeline"

            //__________________________________________[User editable section]__________________________________________\\
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //You can edit per SubShader tags here as usual
            //doc: https://docs.unity3d.com/Manual/SL-SubShaderTags.html
            
            "Queue" = "Transparent"
            "RenderType" = "TransparentCutout"

            "DisableBatching" = "False"
            "ForceNoShadowCasting" = "False"
            "IgnoreProjector" = "True"
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        //UniversalForward pass
        Pass
        {
            Name "Universal Forward"
            Tags { "LightMode"="UniversalForward" }

            //__________________________________________[User editable section]__________________________________________\\
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //You can edit per Pass Render State here as usual
            //doc: https://docs.unity3d.com/Manual/SL-Pass.html
            
            Cull Back
            ZTest LEqual
            ZWrite On
            Offset 0,0
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGBA

            //stencil also 
            //doc: https://docs.unity3d.com/Manual/SL-Stencil.html
            Stencil
            {
                //...
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

            HLSLPROGRAM
            #pragma vertex vertUniversalForward
            #pragma fragment fragUniversalForward
            ENDHLSL
        }

 
        //__________________________________________[User editable section]__________________________________________\\
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //User can insert 1 extra custom passes here.
        //For example, an outline pass this time
        Pass
        {
            //no LightMode is needed for extra custom pass
            Cull front
            HLSLPROGRAM
            #pragma vertex vertExtraCustomPass
            #pragma fragment fragExtraCustomPass
            ENDHLSL
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
          
        //ShadowCaster pass, for rendering this shader into URP's shadowmap renderTextures
        //User should not need to edit this pass in most cases
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            ColorMask 0 //optimization: ShadowCaster pass don't care fragment shader output value, disable color write to reduce bandwidth usage

            HLSLPROGRAM

            #pragma vertex vertShadowCaster
            #pragma fragment fragDoAlphaClipOnlyAndEarlyExit

            ENDHLSL
        }

        //DepthOnly pass, for rendering this shader into URP's _CameraDepthTexture
//        Pass
//        {
//            Name "DepthOnly"
//            Tags { "LightMode"="DepthOnly" }
//            ColorMask 0 //optimization: DepthOnly pass don't care fragment shader output value, disable color write to reduce bandwidth usage
//
//            HLSLPROGRAM
//
//            //__________________________________________[User editable section]__________________________________________\\
//            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//            //not using vertUniversalForward function due to outline pass edited positionOS by bool isExtraCustomPass in UserGeometryDataOutputFunction(...)
//            //#pragma vertex vertUniversalForward
//
//            //we use this instead, this will inlcude positionOS change in UserGeometryDataOutputFunction, include isExtraCustomPass(outlinePass)'s vertex logic.
//            //we only do this due to the fact that this shader's extra pass is an opaque outline pass
//            //where opaque outline should affacet depth write also
//            #pragma vertex vertExtraCustomPass
//            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//            #pragma fragment fragDoAlphaClipOnlyAndEarlyExit
//
//            ENDHLSL
//        }
    }
	CustomEditor "CustomizableGridShaderGUI"
}
