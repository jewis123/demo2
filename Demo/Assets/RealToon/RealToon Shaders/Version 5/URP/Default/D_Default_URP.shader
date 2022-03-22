//RealToon V5.0.8 (URP)
//MJQStudioWorks
//2021

Shader "Universal Render Pipeline/RealToon/Version 5/Default/Default"
{
    Properties
    {

		[Enum(UnityEngine.Rendering.CullMode)] _Culling("Culling", int) = 2

		[Toggle(N_F_TRANS_ON)] _TRANSMODE("Transparent Mode", Float) = 0.0

        _MainTex ("Texture", 2D) = "white" {}
        [Toggle(NOKEWO)] _TexturePatternStyle ("Texture Pattern Style", Float ) = 0.0
        [HDR] _MainColor ("Main Color", Color) = (1.0,1.0,1.0,1.0)
		_MaiColPo("Main Color Power", Float) = 0.8

		[Toggle(NOKEWO)] _MVCOL ("Mix Vertex Color", Float ) = 0.0

		[Toggle(NOKEWO)] _MCIALO ("Main Color In Ambient Light Only", Float ) = 0.0

		[HDR] _HighlightColor ("Highlight Color", Color) = (1.0,1.0,1.0,1.0)
        _HighlightColorPower ("Highlight Color Power", Float ) = 1.0

		_MCapIntensity ("Intensity", Range(0, 1)) = 1.0
		_MCap ("MatCap", 2D) = "white" {}
		[Toggle(NOKEWO)] _SPECMODE ("Specular Mode", Float ) = 0.0
		_SPECIN ("Specular Power", Float ) = 1
		_MCapMask ("Mask MatCap", 2D) = "white" {}

        _Cutout ("Cutout", Range(0, 1)) = 0.0
		[Toggle(NOKEWO)] _AlphaBaseCutout ("Alpha Base Cutout", Float ) = 1.0
        [Toggle(NOKEWO)] _UseSecondaryCutout ("Use Secondary Cutout Only", Float ) = 0.0
        _SecondaryCutout ("Secondary Cutout", 2D) = "white" {}

		_Opacity("Opacity", Range(0, 1)) = 1.0
		_TransparentThreshold("Transparent Threshold", Float) = 0.0

		[Enum(UnityEngine.Rendering.BlendMode)] _BleModSour("Blend - Source", int) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _BleModDest("Blend - Destination", int) = 0

		_MaskTransparency("Mask Transparency", 2D) = "black" {}

		[Toggle(N_F_TRANSAFFSHA_ON)] _TransAffSha("Affect Shadow", Float) = 1.0

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalMapIntensity ("Normal Map Intensity", Float ) = 1.0

        _Saturation ("Saturation", Range(0, 2)) = 1.0

        _OutlineWidth ("Width", Float ) = 0.5
        _OutlineWidthControl ("Width Control", 2D) = "white" {}

		[Enum(Normal,0,Origin,1)] _OutlineExtrudeMethod("Outline Extrude Method", int) = 0

		_OutlineOffset ("Outline Offset", Vector) = (0,0,0)
		_OutlineZPostionInCamera ("Outline Z Position In Camera", Float) = 0.0

		[Enum(Off,1,On,0)] _DoubleSidedOutline("Double Sided Outline", int) = 1

        [HDR] _OutlineColor ("Color", Color) = (0.0,0.0,0.0,1.0)

		[Toggle(NOKEWO)] _MixMainTexToOutline ("Mix Main Texture To Outline", Float ) = 0.0

        _NoisyOutlineIntensity ("Noisy Outline Intensity", Range(0, 1)) = 0.0
		[Toggle(N_F_DNO_ON)] _DynamicNoisyOutline ("Dynamic Noisy Outline", Float ) = 0.0
        [Toggle(NOKEWO)] _LightAffectOutlineColor ("Light Affect Outline Color", Float ) = 0.0

        [Toggle(NOKEWO)] _OutlineWidthAffectedByViewDistance ("Outline Width Affected By View Distance", Float ) = 0.0
		_FarDistanceMaxWidth ("Far Distance Max Width", Float ) = 10.0

        [Toggle(NOKEWO)] _VertexColorBlueAffectOutlineWitdh ("Vertex Color Blue Affect Outline Witdh", Float ) = 0.0

		_DepthThreshold("Depth Threshold", Float) = 900.0

		[Toggle(N_F_O_MOTTSO_ON)] _N_F_MSSOLTFO("Mix Outline To The Shader Output", Float) = 0.0

        _SelfLitIntensity ("Intensity", Range(0, 1)) = 0.0
        [HDR] _SelfLitColor ("Color", Color) = (1.0,1.0,1.0,1.0)
        _SelfLitPower ("Power", Float ) = 2.0
		_TEXMCOLINT ("Texture and Main Color Intensity", Float ) = 1.0
        [Toggle(NOKEWO)] _SelfLitHighContrast ("High Contrast", Float ) = 1.0
        _MaskSelfLit ("Mask Self Lit", 2D) = "white" {}

        _GlossIntensity ("Gloss Intensity", Range(0, 1)) = 1.0
        _Glossiness ("Glossiness", Range(0, 1)) = 0.6
        _GlossSoftness ("Softness", Range(0, 1)) = 0.0
        [HDR] _GlossColor ("Color", Color) = (1.0,1.0,1.0,1.0)
        _GlossColorPower ("Color Power", Float ) = 10.0
        _MaskGloss ("Mask Gloss", 2D) = "white" {}

        _GlossTexture ("Gloss Texture", 2D) = "black" {}
        _GlossTextureSoftness ("Softness", Float ) = 0.0
		[Toggle(NOKEWO)] _PSGLOTEX ("Pattern Style", Float ) = 0.0
        _GlossTextureRotate ("Rotate", Float ) = 0.0
        [Toggle(NOKEWO)] _GlossTextureFollowObjectRotation ("Follow Object Rotation", Float ) = 0.0
        _GlossTextureFollowLight ("Follow Light", Range(0, 1)) = 0.0

        [HDR] _OverallShadowColor ("Overall Shadow Color", Color) = (0.0,0.0,0.0,1.0)
        _OverallShadowColorPower ("Overall Shadow Color Power", Float ) = 1.0

        [Toggle(NOKEWO)] _SelfShadowShadowTAtViewDirection ("Self Shadow & ShadowT At View Direction", Float ) = 0.0

		_ReduSha ("Reduce Shadow", Float ) = 0.5

		_ShadowHardness ("Shadow Hardness", Range(0, 1)) = 0.0

        _SelfShadowRealtimeShadowIntensity ("Self Shadow & Realtime Shadow Intensity", Range(0, 1)) = 1.0
        _SelfShadowThreshold ("Threshold", Range(0, 1)) = 0.930
        [Toggle(NOKEWO)] _VertexColorGreenControlSelfShadowThreshold ("Vertex Color Green Control Self Shadow Threshold", Float ) = 0.0
        _SelfShadowHardness ("Hardness", Range(0, 1)) = 1.0
        [HDR] _SelfShadowRealTimeShadowColor ("Self Shadow & Real Time Shadow Color", Color) = (1.0,1.0,1.0,1.0)
        _SelfShadowRealTimeShadowColorPower ("Self Shadow & Real Time Shadow Color Power", Float ) = 1.0
		[Toggle(NOKEWO)] _LigIgnoYNorDir("Light Ignore Y Normal Direction", Float) = 0
		[Toggle(NOKEWO)] _SelfShadowAffectedByLightShadowStrength ("Self Shadow Affected By Light Shadow Strength", Float ) = 0.0

        _SmoothObjectNormal ("Smooth Object Normal", Range(0, 1)) = 0.0
        [Toggle(NOKEWO)] _VertexColorRedControlSmoothObjectNormal ("Vertex Color Red Control Smooth Object Normal", Float ) = 0.0
        _XYZPosition ("XYZ Position", Vector) = (0.0,0.0,0.0,0.0)
        [Toggle(NOKEWO)] _ShowNormal ("Show Normal", Float ) = 0.0

        _ShadowColorTexture ("Shadow Color Texture", 2D) = "white" {}
        _ShadowColorTexturePower ("Power", Float ) = 0.0

        _ShadowTIntensity ("ShadowT Intensity", Range(0, 1)) = 1.0
        _ShadowT ("ShadowT", 2D) = "white" {}
        _ShadowTLightThreshold ("Light Threshold", Float ) = 50.0
        _ShadowTShadowThreshold ("Shadow Threshold", Float ) = 0.0
		_ShadowTHardness ("Hardness", Range(0, 1)) = 1.0
        [HDR] _ShadowTColor ("Color", Color) = (1.0,1.0,1.0,1.0)
        _ShadowTColorPower ("Color Power", Float ) = 1.0

		[Toggle(NOKEWO)] _STIL ("Ignore Light", Float ) = 0.0

		[Toggle(N_F_STIS_ON)] _N_F_STIS ("Show In Shadow", Float ) = 0.0

		[Toggle(N_F_STIAL_ON )] _N_F_STIAL ("Show In Ambient Light", Float ) = 0.0
        _ShowInAmbientLightShadowIntensity ("Show In Ambient Light & Shadow Intensity", Range(0, 1)) = 1.0
        _ShowInAmbientLightShadowThreshold ("Show In Ambient Light & Shadow Threshold", Float ) = 0.4

        [Toggle(NOKEWO)] _LightFalloffAffectShadowT ("Light Falloff Affect ShadowT", Float ) = 0.0

        _PTexture ("PTexture", 2D) = "white" {}
		_PTCol("Color", Color) = (0.0, 0.0, 0.0, 1.0) //
        _PTexturePower ("Power", Float ) = 1.0

		[Toggle(N_F_RELGI_ON)] _RELG ("Receive Environmental Lighting and GI", Float ) = 1.0
        _EnvironmentalLightingIntensity ("Environmental Lighting Intensity", Float ) = 1.0

        [Toggle(NOKEWO)] _GIFlatShade ("GI Flat Shade", Float ) = 0.0
        _GIShadeThreshold ("GI Shade Threshold", Range(0, 1)) = 0.0

        [Toggle(NOKEWO)] _LightAffectShadow ("Light Affect Shadow", Float ) = 0.0
        _LightIntensity ("Light Intensity", Float ) = 1.0

		[Toggle(N_F_USETLB_ON)] _UseTLB ("Use Traditional Light Blend", Float ) = 0.0
		[Toggle(N_F_EAL_ON)] _N_F_EAL ("Enable Additional Lights", Float ) = 1.0

		_DirectionalLightIntensity ("Directional Light Intensity", Float ) = 0.0
		_PointSpotlightIntensity ("Point and Spot Light Intensity", Float ) = 0.0
		_LightFalloffSoftness ("Light Falloff Softness", Range(0, 1)) = 1.0

        _CustomLightDirectionIntensity ("Intensity", Range(0, 1)) = 0.0
        [Toggle(NOKEWO)] _CustomLightDirectionFollowObjectRotation ("Follow Object Rotation", Float ) = 0.0
        _CustomLightDirection ("Custom Light Direction", Vector) = (0.0,0.0,10.0,0.0)

        _ReflectionIntensity ("Intensity", Range(0, 1)) = 0.0
        _ReflectionRoughtness ("Roughness", Float ) = 0.0
		_RefMetallic ("Metallic", Range(0, 1) ) = 0.0

        _MaskReflection ("Mask Reflection", 2D) = "white" {}

        _FReflection ("FReflection", 2D) = "black" {}

		_RimLigInt("Rim Light Intensity", Range(0, 1)) = 1.0
        _RimLightUnfill ("Unfill", Float ) = 1.5
        [HDR] _RimLightColor ("Color", Color) = (1.0,1.0,1.0,1.0)
        _RimLightColorPower ("Color Power", Float ) = 10.0
        _RimLightSoftness ("Softness", Range(0, 1)) = 1.0
        [Toggle(NOKEWO)] _RimLightInLight ("Rim Light In Light", Float ) = 1.0
        [Toggle(NOKEWO)] _LightAffectRimLightColor ("Light Affect Rim Light Color", Float ) = 0.0

		_RefVal ("ID", int ) = 0
        [Enum(Blank,8,A,0,B,2)] _Oper("Set 1", int) = 0
        [Enum(Blank,8,None,4,A,6,B,7)] _Compa("Set 2", int) = 4

		[Toggle(N_F_MC_ON)] _N_F_MC ("MatCap", Float ) = 0.0
		[Toggle(N_F_NM_ON)] _N_F_NM ("Normal Map", Float ) = 0.0
		[Toggle(N_F_CO_ON)] _N_F_CO ("Cutout", Float ) = 0.0
		[Toggle(N_F_O_ON)] _N_F_O ("Outline", Float ) = 1.0
		[Toggle(N_F_CA_ON)] _N_F_CA ("Color Adjustment", Float ) = 0.0
		[Toggle(N_F_SL_ON)] _N_F_SL ("Self Lit", Float ) = 0.0
		[Toggle(N_F_GLO_ON)] _N_F_GLO ("Gloss", Float ) = 0.0
		[Toggle(N_F_GLOT_ON)] _N_F_GLOT ("Gloss Texture", Float ) = 0.0
		[Toggle(N_F_SS_ON)] _N_F_SS ("Self Shadow", Float ) = 1.0
		[Toggle(N_F_SON_ON)] _N_F_SON ("Smooth Object Normal", Float ) = 0.0
		[Toggle(N_F_SCT_ON)] _N_F_SCT ("Shadow Color Texture", Float ) = 0.0
		[Toggle(N_F_ST_ON)] _N_F_ST ("ShadowT", Float ) = 0.0
		[Toggle(N_F_PT_ON)] _N_F_PT ("PTexture", Float ) = 0.0
		[Toggle(N_F_CLD_ON)] _N_F_CLD ("Custom Light Direction", Float ) = 0.0
		[Toggle(N_F_R_ON)] _N_F_R ("Relfection", Float ) = 0.0
		[Toggle(N_F_FR_ON)] _N_F_FR ("FRelfection", Float ) = 0.0
		[Toggle(N_F_RL_ON)] _N_F_RL ("Rim Light", Float ) = 0.0

		[Toggle(N_F_HDLS_ON)] _N_F_HDLS ("Hide Directional Light Shadow", Float ) = 0.0
		[Toggle(N_F_HPSS_ON)] _N_F_HPSS ("Hide Point & Spot Light Shadow", Float ) = 0.0

		[Toggle(N_F_DCS_ON)] _N_F_DCS ("Disable Cast Shadow", Float ) = 0.0
		[Toggle(N_F_NLASOBF_ON)] _N_F_NLASOBF ("No Light and Shadow On BackFace", Float ) = 0.0

		[Toggle(N_F_OFLMB_ON)] _N_F_OFLMB("Optimize for [Light Mode: Baked]", Float) = 0.0

		[Enum(On, 1, Off, 0)] _ZWrite("ZWrite", int) = 1

		//Others
		[HideInInspector]_SkinMatrixIndex("Skin Matrix Index Offset", Float) = 0
		[HideInInspector]_ComputeMeshIndex("Compute Mesh Buffer Index Offset", Float) = 0

    }

    SubShader
    {

        Tags{"Queue" = "Geometry" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 300

		Pass {

Name"Outline"
Tags{"LightMode"="SRPDefaultUnlit"}
//OL_NRE

Cull [_DoubleSidedOutline]//OL_RCUL
Blend[_BleModSour][_BleModDest]

			Stencil {
            	Ref[_RefVal]
            	Comp [_Compa]
            	Pass [_Oper]
            	Fail [_Oper]
            }

            HLSLPROGRAM

            #pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 xboxseries playstation wiiu switch
#pragma target 2.0 //targetol

			#pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fog

            #pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Assets/RealToon/RealToon Shaders/RealToon Core/URP/RT_URP_Core.hlsl"

			#pragma shader_feature_local_fragment N_F_TRANS_ON
			#pragma shader_feature_local_fragment N_F_CO_ON
			#pragma shader_feature_local_fragment N_F_EAL_ON
			#pragma shader_feature_local N_F_O_ON
			#pragma shader_feature_local_vertex N_F_DNO_ON

			struct Attributes
            {

                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
				float4 vertexColor	: COLOR;
				float2 uvLM         : TEXCOORD1;
			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				float4 tangentOS    : TANGENT;
float4 weights : BLENDWEIGHTS;//DOTS_LiBleSki_OL
uint4 indices : BLENDINDICES;//DOTS_LiBleSki_OL
//uint vertexID : SV_VertexID;//DOTS_CompDef_OL
			#endif
                UNITY_VERTEX_INPUT_INSTANCE_ID

            };

            struct Varyings
            {

                float2 uv                       : TEXCOORD0;
                float4 positionWSAndFogFactor   : TEXCOORD2; 
				float4 projPos					: TEXCOORD7;
				float4 vertexColor				: COLOR;
                float4 positionCS               : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO

            };


			Varyings LitPassVertex(Attributes input)
            {

				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID (input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.uv = input.uv;
                output.vertexColor = input.vertexColor;

			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				float4 _LBS_CD_Position = 0;
				float3 _LBS_CD_Normal = 0;
				float4 _LBS_CD_Tangent = 0;
DOTS_LiBleSki(input.indices, input.weights, input.positionOS.xyz, input.normalOS.xyz, input.tangentOS.xyz, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_LiBleSki_OL
//DOTS_CompDef(input.vertexID, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_CompDef_OL
			#else
				float4 _LBS_CD_Position = input.positionOS;
				float3 _LBS_CD_Normal = input.normalOS;
				//float4 _LBS_CD_Tangent = input.tangentOS; //not currently needed
			#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(_LBS_CD_Position.xyz);

				float4 objPos = mul ( unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) );

				half RTD_OB_VP_CAL = distance(objPos.rgb,_WorldSpaceCameraPos);

				half RTD_OL_VCRAOW_OO = lerp( _OutlineWidth, (_OutlineWidth*(1.0 - output.vertexColor.b)), _VertexColorBlueAffectOutlineWitdh );

				half RTD_OL_OLWABVD_OO = lerp( RTD_OL_VCRAOW_OO, ( clamp(RTD_OL_VCRAOW_OO*RTD_OB_VP_CAL, RTD_OL_VCRAOW_OO, _FarDistanceMaxWidth) ), _OutlineWidthAffectedByViewDistance );

				#if defined(SHADER_API_GLES)
					half4 _OutlineWidthControl_var = tex2Dlod(_OutlineWidthControl, float4(TRANSFORM_TEX(output.uv, _OutlineWidthControl), 0.0, 0));
				#else
					half4 _OutlineWidthControl_var = SAMPLE_TEXTURE2D_LOD(_OutlineWidthControl, sampler_OutlineWidthControl, TRANSFORM_TEX(output.uv, _OutlineWidthControl), 0.0);
				#endif

				#if N_F_DNO_ON

					float4 _3726 = _Time;
					float _8530_ang = _3726.g;
					float _8530_spd = 0.002;
					float _8530_cos = cos(_8530_spd * _8530_ang);
					float _8530_sin = sin(_8530_spd * _8530_ang);
					float2 _8530_piv = float2(0.5, 0.5);
					half2 _8530 = (mul(output.uv - _8530_piv, float2x2(_8530_cos, -_8530_sin, _8530_sin, _8530_cos)) + _8530_piv);

					half2 RTD_OL_DNOL_OO = _8530;

				#else

					half2 RTD_OL_DNOL_OO = output.uv;

				#endif


				half2 _8743 = RTD_OL_DNOL_OO;

                float2 _1283_skew = _8743 + 0.2127+_8743.x*0.3713*_8743.y;
                float2 _1283_rnd = 4.789*sin(489.123*(_1283_skew));
                half _1283 = frac(_1283_rnd.x*_1283_rnd.y*(1+_1283_skew.x));

				float3 _OEM = lerp(_LBS_CD_Normal.xyz, normalize(_LBS_CD_Position.xyz), _OutlineExtrudeMethod);

				half RTD_OL = ( RTD_OL_OLWABVD_OO*0.01 )*_OutlineWidthControl_var.r*lerp(1.0,_1283,_NoisyOutlineIntensity);
                output.positionCS = mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), float4( (_LBS_CD_Position.xyz + _OutlineOffset.xyz * 0.01) + _OEM * RTD_OL,1.0) ) );

				#if defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
					output.positionCS.z += _OutlineZPostionInCamera * 0.0005;
				#else
					output.positionCS.z -= _OutlineZPostionInCamera * 0.0005;
				#endif

                output.projPos = ComputeScreenPos (output.positionCS);
				float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
				output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);

                return output;

            }

            half4 LitPassFragment(Varyings input) : SV_Target
            {

				UNITY_SETUP_INSTANCE_ID (input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 positionWS = input.positionWSAndFogFactor.xyz;

                Light mainLight = GetMainLight();
				half3 color = (half3)1.0;

				float4 objPos = mul ( unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) );
                float2 sceneUVs = (input.projPos.xy / input.projPos.w);

				half RTD_OB_VP_CAL = distance(objPos.rgb,_WorldSpaceCameraPos);
				half2 RTD_VD_Cal = (float2((sceneUVs.x * 2.0 - 1.0)*(_ScreenParams.r/_ScreenParams.g), sceneUVs.y * 2.0 - 1.0).rg*RTD_OB_VP_CAL);

				half2 RTD_TC_TP_OO = lerp( input.uv, RTD_VD_Cal, _TexturePatternStyle );

				half4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(RTD_TC_TP_OO, _MainTex));

				//RT_TRANS_CO
				half RTD_TRAN_OPA_Sli;
				half RTD_CO;
				RT_TRANS_CO(input.uv, _MainTex_var, RTD_TRAN_OPA_Sli, RTD_CO);

				#if N_F_TRANS_ON
					#ifndef N_F_CO_ON
						clip(RTD_TRAN_OPA_Sli - 0.5);
					#endif
				#endif



				#ifndef N_F_OFLMB_ON
					half3 lightColor = mainLight.color.rgb;
				#else
					half3 lightColor = (half3)1.0;
				#endif



				#ifndef N_F_OFLMB_ON
					#ifdef _ADDITIONAL_LIGHTS
						#if N_F_EAL_ON
							int additionalLightsCount = GetAdditionalLightsCount();
							for (int i = 0; i < additionalLightsCount; ++i)
							{
								Light light = GetAdditionalLight(i, positionWS);
								lightColor += light.color * light.distanceAttenuation;
							}
						#endif
					#endif
				#endif



                float fogFactor = input.positionWSAndFogFactor.w;


				//
				#ifdef UNITY_COLORSPACE_GAMMA
					_OutlineColor = float4(LinearToGamma22(_OutlineColor.rgb), _OutlineColor.a);
				#endif

				half3 RTD_OL_LAOC_OO = lerp( lerp(_OutlineColor.rgb,_OutlineColor.rgb * _MainTex_var.rgb, _MixMainTexToOutline) , lerp(half3(0.0, 0.0, 0.0), lerp(_OutlineColor.rgb,_OutlineColor.rgb * _MainTex_var.rgb, _MixMainTexToOutline) ,lightColor.rgb), _LightAffectOutlineColor );
				//


				half3 finalRGBA = RTD_OL_LAOC_OO;

                color = MixFog(finalRGBA, fogFactor);

				#if defined(N_F_TRANS_ON) & !defined(N_F_CO_ON)
					return half4(color, RTD_TRAN_OPA_Sli);
				#else
					return half4(color, 1.0);
				#endif

            }

			ENDHLSL
        }

        Pass
        {

            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Cull [_Culling]
			Blend [_BleModSour] [_BleModDest]
			ZWrite[_ZWrite]

			Stencil {
            	Ref[_RefVal]
            	Comp [_Compa]
            	Pass [_Oper]
            	Fail [_Oper]
            }

            HLSLPROGRAM

            #pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 xboxseries playstation wiiu switch
#pragma target 2.0 //targetfl

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

            #pragma multi_compile_fog

            #pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Assets/RealToon/RealToon Shaders/RealToon Core/URP/RT_URP_Core.hlsl"

			#pragma shader_feature_local_fragment N_F_USETLB_ON
			#pragma shader_feature_local_fragment N_F_TRANS_ON

			#pragma shader_feature_local_fragment N_F_O_ON
			#pragma shader_feature_local_fragment N_F_O_MOTTSO_ON
			#pragma shader_feature_local_fragment N_F_MC_ON
			#pragma shader_feature_local_fragment N_F_NM_ON
			#pragma shader_feature_local_fragment N_F_CO_ON
			#pragma shader_feature_local_fragment N_F_SL_ON
			#pragma shader_feature_local_fragment N_F_CA_ON
			#pragma shader_feature_local_fragment N_F_GLO_ON
			#pragma shader_feature_local_fragment N_F_GLOT_ON
			#pragma shader_feature_local_fragment N_F_SS_ON
			#pragma shader_feature_local_fragment N_F_SCT_ON
			#pragma shader_feature_local_fragment N_F_ST_ON
			#pragma shader_feature_local_fragment N_F_STIS_ON
			#pragma shader_feature_local_fragment N_F_STIAL_ON 
			#pragma shader_feature_local N_F_SON_ON
			#pragma shader_feature_local_fragment N_F_PT_ON
			#pragma shader_feature_local_fragment N_F_RELGI_ON
			#pragma shader_feature_local_fragment N_F_CLD_ON
			#pragma shader_feature_local_fragment N_F_R_ON
			#pragma shader_feature_local_fragment N_F_FR_ON
			#pragma shader_feature_local_fragment N_F_RL_ON
			#pragma shader_feature_local_fragment N_F_HDLS_ON
			#pragma shader_feature_local_fragment N_F_HPSS_ON
			#pragma shader_feature_local_fragment N_F_EAL_ON
			#pragma shader_feature_local_fragment N_F_NLASOBF_ON
			#pragma shader_feature_local_fragment N_F_OFLMB_ON

			#define _EMISSION

            struct Attributes
            {

                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
				float2 lightmapUV   : TEXCOORD1;
				float4 vertexColor	: COLOR;
			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
float4 weights : BLENDWEIGHTS;//DOTS_LiBleSki_FL
uint4 indices : BLENDINDICES;//DOTS_LiBleSki_FL
//uint vertexID : SV_VertexID;//DOTS_CompDef_FL
			#endif
                UNITY_VERTEX_INPUT_INSTANCE_ID

            };

            struct Varyings
            {

                float2 uv						: TEXCOORD0;
				float2 lightmapUV				: TEXCOORD1;
                float4 positionWSAndFogFactor   : TEXCOORD2; 
                half3  normalWS                 : TEXCOORD3;
                half3 tangentWS                 : TEXCOORD4;
                half3 bitangentWS               : TEXCOORD5;

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                float4 shadowCoord              : TEXCOORD6; 
#endif
				float4 projPos					: TEXCOORD7;
				float4 posWorld					: TEXCOORD8;
				float3 smoNorm					: TEXCOORD9;
				float4 vertexColor				: COLOR;
                float4 positionCS               : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO

            };

            Varyings LitPassVertex(Attributes input)
            {

                Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID (input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				float4 _LBS_CD_Position = 0;
				float3 _LBS_CD_Normal = 0;
				float4 _LBS_CD_Tangent = 0;
DOTS_LiBleSki(input.indices, input.weights, input.positionOS.xyz, input.normalOS.xyz, input.tangentOS.xyz, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_LiBleSki_FL
//DOTS_CompDef(input.vertexID, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_CompDef_FL
			#else
				float4 _LBS_CD_Position = input.positionOS;
				float3 _LBS_CD_Normal = input.normalOS;
				float4 _LBS_CD_Tangent = input.tangentOS;
			#endif

                VertexPositionInputs vertexInput = GetVertexPositionInputs(_LBS_CD_Position.xyz);
				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(_LBS_CD_Normal, _LBS_CD_Tangent);

				float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
				
                output.normalWS = vertexNormalInput.normalWS;
                output.tangentWS = vertexNormalInput.tangentWS;
                output.bitangentWS = vertexNormalInput.bitangentWS;

				output.posWorld = float4(vertexInput.positionWS, 1.0);
				output.uv = input.uv;
                output.vertexColor = input.vertexColor;
				output.positionCS = vertexInput.positionCS;

				output.projPos = ComputeScreenPos (output.positionCS);

				OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);

				#if N_F_SON_ON
					output.smoNorm = calcNorm(float3(_LBS_CD_Position.x + ((_XYZPosition.x) * 0.1), _LBS_CD_Position.y + ((_XYZPosition.y) * 0.1), _LBS_CD_Position.z + ((_XYZPosition.z) * 0.1)));
				#endif

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)

                output.shadowCoord = GetShadowCoord(vertexInput);
#endif

                return output;
            }

            half4 LitPassFragment(Varyings input, float facing : VFACE) : SV_Target
            {

				UNITY_SETUP_INSTANCE_ID (input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 positionWS = input.positionWSAndFogFactor.xyz;

				//=========

				float4 shadow_mask = SAMPLE_SHADOWMASK(input.lightmapUV);

				#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
					half4 shadowMask = shadow_mask;
				//#elif !defined (LIGHTMAP_ON)
					//half4 shadowMask = unity_ProbesOcclusion;
				#else
					half4 shadowMask = half4(1.0, 1.0, 1.0, 1.0);
				#endif

				//==========

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					Light mainLight = GetMainLight(input.shadowCoord, positionWS, shadowMask);
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					Light mainLight = GetMainLight(TransformWorldToShadowCoord(positionWS), positionWS, shadowMask);
				#else
					Light mainLight = GetMainLight();
				#endif

				//RT_NM
				float3 normalLocal = RT_NM(input.uv);

				half3 color = (half3)0.0;
				float3 A_L_O = (float3)0.0;
				float3 baked_GI = (float3)1.0;

				half isFrontFace = ( facing >= 0 ? 1 : 0 );
				float4 objPos = mul ( unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) );
				float2 sceneUVs = (input.projPos.xy / input.projPos.w);

				input.normalWS = normalize(input.normalWS);
				float3x3 tangentTransform = float3x3( input.tangentWS, input.bitangentWS, input.normalWS);
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - input.posWorld.xyz);
				float3 normalDirection = normalize(mul( normalLocal, tangentTransform ));
				float3 viewReflectDirection = reflect( -viewDirection, normalDirection );

				half RTD_OB_VP_CAL = distance(objPos.rgb,_WorldSpaceCameraPos);
				half2 RTD_VD_Cal = (float2((sceneUVs.x * 2.0 - 1.0)*(_ScreenParams.r/_ScreenParams.g), sceneUVs.y * 2.0 - 1.0).rg*RTD_OB_VP_CAL);

				half2 RTD_TC_TP_OO = lerp(input.uv, RTD_VD_Cal, _TexturePatternStyle);

				half4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(RTD_TC_TP_OO, _MainTex));
				half3 _RTD_MVCOL = lerp( (half3)1.0, input.vertexColor.rgb, _MVCOL);



				//
				#ifdef UNITY_COLORSPACE_GAMMA
					_OverallShadowColor = float4(LinearToGamma22(_OverallShadowColor.rgb), _OverallShadowColor.a);
				#endif

				#ifndef N_F_OFLMB_ON
					half3 RTD_OSC = (_OverallShadowColor.rgb*_OverallShadowColorPower);
				#else
					half3 RTD_OSC = (half3)0.0;
				#endif
				//



				//RT_MCAP
				half3 MCapOutP = RT_MCAP(input.uv, normalDirection);


				//
				#ifdef UNITY_COLORSPACE_GAMMA
					_MainColor = float4(LinearToGamma22(_MainColor.rgb),_MainColor.a);
				#endif
				//
				half3 _MC_MCP = (_MainColor.rgb * _MaiColPo);



				//RT_MCAP_SUB1
				half3 RTD_TEX_COL;
				half3 RTD_MCIALO_IL = RT_MCAP_SUB1(MCapOutP, _MainTex_var, _RTD_MVCOL, RTD_TEX_COL);
				//

				//RT_TRANS_CO
				half RTD_TRAN_OPA_Sli;
				half RTD_CO;
				RT_TRANS_CO(input.uv, _MainTex_var, RTD_TRAN_OPA_Sli, RTD_CO);

				//RT_SON
				float3 RTD_SON_CHE_1;
				float3 RTD_SON = RT_SON(input.vertexColor, input.smoNorm, normalDirection, RTD_SON_CHE_1);

				//RT_RELGI
				float3 RTD_GI_FS_OO = RT_RELGI(RTD_SON);

				//RT_SCT
				half3 RTD_SCT = RT_SCT(input.uv, RTD_MCIALO_IL);

				//RT_PT
				half3 RTD_PT_COL;
				half RTD_PT = RT_PT(RTD_VD_Cal, RTD_PT_COL);



				//
				#ifdef UNITY_COLORSPACE_GAMMA
					_SelfShadowRealTimeShadowColor = float4(LinearToGamma22(_SelfShadowRealTimeShadowColor.rgb), _SelfShadowRealTimeShadowColor.a);
				#endif

				#ifndef N_F_OFLMB_ON
					half3 ss_col = lerp( RTD_PT_COL, (_SelfShadowRealTimeShadowColor.rgb * _SelfShadowRealTimeShadowColorPower) * RTD_OSC * RTD_SCT, RTD_PT);
				#else
					half3 ss_col = (half3)0.0;
				#endif
				//



				#ifndef N_F_OFLMB_ON
					#if N_F_NLASOBF_ON
						half3 lightColor = lerp( (float3)0.0 ,mainLight.color.rgb,isFrontFace);
					#else
						half3 lightColor = mainLight.color.rgb;
					#endif
				#else
					half3 lightColor = (half3)0.0;
				#endif



				#ifndef N_F_OFLMB_ON
					#if N_F_HDLS_ON
						half attenuation = 1.0; 
					#else
						half dlshmin = lerp( 0.0, 0.6 ,_ShadowHardness);
						half dlshmax = lerp( 1.0, 0.6 ,_ShadowHardness);

						#if N_F_NLASOBF_ON
							half FB_Check = lerp( 1.0 , mainLight.shadowAttenuation,isFrontFace);
						#else
							half FB_Check = mainLight.shadowAttenuation;
						#endif

						half attenuation = smoothstep(dlshmin,dlshmax,FB_Check);
					#endif
				#else
					half attenuation = 1.0;
				#endif



				#ifndef N_F_OFLMB_ON
					float3 lightDirection = mainLight.direction;
					float3 halfDirection = normalize(viewDirection + lightDirection);
				#else
					float3 lightDirection = (float3)0.0;
					float3 halfDirection = (float3)0.0;
				#endif



				//RT_CLD
				float3 RTD_CLD = RT_CLD(lightDirection);



				#ifndef N_F_OFLMB_ON
					half3 RTD_ST_SS_AVD_OO = lerp(RTD_CLD, viewDirection, _SelfShadowShadowTAtViewDirection);
					half RTD_NDOTL = 0.5 * dot(RTD_ST_SS_AVD_OO, float3(RTD_SON.x, RTD_SON.y * (1 - _LigIgnoYNorDir), RTD_SON.z)) + 0.5;
					half RTD_LVLC = RTD_LVLC_F(lightColor.rgb);
					half3 lig_col_int = (_LightIntensity * lightColor.rgb);
					half3 RTD_LAS = lerp(ss_col * RTD_LVLC, (ss_col * lig_col_int), _LightAffectShadow);
				#else
					half3 RTD_ST_SS_AVD_OO = (half3)0.0;
					half RTD_NDOTL = 0.0;
					half RTD_LVLC = 0.0;
					half3 lig_col_int = (half3)0.0;
					half3 RTD_LAS = (half3)0.0;
				#endif



				//
				#ifdef UNITY_COLORSPACE_GAMMA
					_HighlightColor = float4(LinearToGamma22(_HighlightColor.rgb), _HighlightColor.a);
				#endif

				#ifndef _N_F_OFLMB
					half3 RTD_HL = (_HighlightColor.rgb*_HighlightColorPower+_DirectionalLightIntensity);
				#else
					half3 RTD_HL = (half3)0.0;
				#endif
				//



				half3 RTD_MCIALO = lerp(RTD_TEX_COL , lerp( lerp( (RTD_TEX_COL * _MC_MCP), (RTD_TEX_COL + _MC_MCP), _SPECMODE) , _MainTex_var.rgb * MCapOutP * _RTD_MVCOL * 0.7 , clamp((RTD_LVLC*1.0),0.0,1.0) ) , _MCIALO );

				//RT_GLO
				half RTD_GLO;
				half3 RTD_GLO_COL;
				RT_GLO(input.uv, RTD_VD_Cal, halfDirection, normalDirection, viewDirection, RTD_GLO, RTD_GLO_COL);
				half3 RTD_GLO_OTHERS = RTD_GLO;

				//RT_RL
				half3 RTD_RL_LARL_OO;
				half RTD_RL_MAIN;
				half RTD_RL_CHE_1 = RT_RL(viewDirection, normalDirection, lightColor, RTD_RL_LARL_OO, RTD_RL_MAIN);

				//RT_ST
				half3 RTD_SHAT_COL;
				half RTD_STIAL;
				half RTD_ST_IS;
				half3 RTD_ST_LAF;
				half RTD_ST = RT_ST(input.uv, RTD_NDOTL, attenuation, RTD_LVLC, RTD_PT_COL, lig_col_int, RTD_SCT, RTD_OSC, RTD_PT, RTD_SHAT_COL, RTD_STIAL, RTD_ST_IS, RTD_ST_LAF);

				//RT_SS
				half RTD_SS = RT_SS(input.vertexColor, RTD_NDOTL, attenuation, _MainLightShadowData.x);

				//RT_RELGI_SUB1
				half ref_int_val;
				half3 RTD_SL_OFF_OTHERS = RT_RELGI_SUB1(input.lightmapUV, RTD_GI_FS_OO, RTD_SHAT_COL, RTD_MCIALO, RTD_STIAL, mainLight, normalDirection);



				#ifndef N_F_OFLMB_ON
					half3 RTD_R_OFF_OTHERS = lerp( lerp( RTD_ST_LAF, RTD_LAS, RTD_ST_IS) , lerp( RTD_ST_LAF, lerp( lerp( RTD_MCIALO_IL * RTD_HL , RTD_GLO_COL , RTD_GLO_OTHERS) , RTD_RL_LARL_OO , RTD_RL_CHE_1 ) * lightColor.rgb, RTD_ST) , RTD_SS ) ;
				#else
					half3 RTD_R_OFF_OTHERS = (half3)0.0;
				#endif



				//RT_R
				half3 RTD_R = RT_R(input.uv, viewReflectDirection, viewDirection, normalDirection, RTD_TEX_COL, RTD_R_OFF_OTHERS);

				//RT_SL
				half3 RTD_SL_CHE_1;
				half3 RTD_SL = RT_SL(input.uv, RTD_SL_OFF_OTHERS, RTD_TEX_COL, RTD_R, RTD_SL_CHE_1);

				//RT_RL_SUB1
				half3 RTD_RL = RT_RL_SUB1(RTD_SL_CHE_1, RTD_RL_LARL_OO, RTD_RL_MAIN);

				half3 RTD_CA_OFF_OTHERS = (RTD_RL + RTD_SL);

				half3 main_light_output = RTD_CA_OFF_OTHERS;



				#ifndef N_F_OFLMB_ON

					#ifdef _ADDITIONAL_LIGHTS

						#if N_F_EAL_ON

							uint additionalLightsCount = GetAdditionalLightsCount();

							for (uint i = 0u; i < additionalLightsCount; ++i)
							{
								Light light = GetAdditionalLight(i, input.posWorld.xyz, shadowMask);

								float3 lightDirection = light.direction;


								#if N_F_NLASOBF_ON
									half3 lightColor = lerp((half3)0.0,light.color.rgb,isFrontFace);
								#else
									half3 lightColor = light.color.rgb;
								#endif

								half RTD_LVLC = RTD_LVLC_F(lightColor.rgb);
								float3 halfDirection = normalize(viewDirection+lightDirection);

								#if N_F_HPSS_ON
									half attenuation = 1.0; 
								#else
									half dlshmin = lerp( 0.0 , 0.6 ,_ShadowHardness);
									half dlshmax = lerp( 1.0 , 0.6 ,_ShadowHardness);

									#if N_F_NLASOBF_ON
										half FB_Check = lerp( 1.0 ,light.shadowAttenuation,isFrontFace);
									#else
										half FB_Check = light.shadowAttenuation;
									#endif
									half attenuation = smoothstep(dlshmin, dlshmax ,FB_Check);
								#endif

								half lightfos = smoothstep( 0.0 , _LightFalloffSoftness ,light.distanceAttenuation);

								half3 lig_col_int = (_LightIntensity * lightColor.rgb);

								half3 RTD_LAS = lerp(ss_col * RTD_LVLC, (ss_col * lig_col_int), _LightAffectShadow);
								half3 RTD_HL = (_HighlightColor.rgb*_HighlightColorPower+_PointSpotlightIntensity);

								half3 RTD_MCIALO = lerp(RTD_TEX_COL , lerp(lerp((RTD_TEX_COL * _MC_MCP), (RTD_TEX_COL + _MC_MCP), _SPECMODE) , _MainTex_var.rgb * MCapOutP * _RTD_MVCOL * 0.7 , clamp((RTD_LVLC * 1.0),0.0,1.0)) , _MCIALO);

								//RT_GLO
								half RTD_GLO;
								half3 RTD_GLO_COL;
								RT_GLO(input.uv, RTD_VD_Cal, halfDirection, normalDirection, viewDirection, RTD_GLO, RTD_GLO_COL);
								half3 RTD_GLO_OTHERS = RTD_GLO;

								//RT_RL
								half3 RTD_RL_LARL_OO;
								half RTD_RL_MAIN;
								half RTD_RL_CHE_1 = RT_RL(viewDirection, normalDirection, lightColor, RTD_RL_LARL_OO, RTD_RL_MAIN);

								//RT_CLD
								float3 RTD_CLD = RT_CLD(lightDirection);

								half3 RTD_ST_SS_AVD_OO = lerp( RTD_CLD, viewDirection, _SelfShadowShadowTAtViewDirection );
								half RTD_NDOTL = 0.5*dot(RTD_ST_SS_AVD_OO, float3(RTD_SON.x, RTD_SON.y * (1 - _LigIgnoYNorDir), RTD_SON.z))+0.5;

								//RT_ST
								half3 RTD_SHAT_COL;
								half RTD_STIAL;
								half RTD_ST_IS;
								half3 RTD_ST_LAF;
								half RTD_ST = RT_ST(input.uv, RTD_NDOTL, lightfos, RTD_LVLC, RTD_PT_COL, lig_col_int, RTD_SCT, RTD_OSC, RTD_PT, RTD_SHAT_COL, RTD_STIAL, RTD_ST_IS, RTD_ST_LAF);

								//RT_SS
								half RTD_SS = RT_SS(input.vertexColor, RTD_NDOTL, attenuation, GetAdditionalLightShadowParams(i).x);

								half3 RTD_R_OFF_OTHERS = lerp(lerp(RTD_ST_LAF, RTD_LAS, RTD_ST_IS), lerp(RTD_ST_LAF, lerp(lerp(RTD_MCIALO_IL * RTD_HL, RTD_GLO_COL, RTD_GLO_OTHERS), RTD_RL_LARL_OO, RTD_RL_CHE_1) * lightColor.rgb, RTD_ST), RTD_SS);

								//RT_R
								half3 RTD_R = RT_R(input.uv, viewReflectDirection, viewDirection, normalDirection, RTD_TEX_COL, RTD_R_OFF_OTHERS);

								//RT_SL
								half3 RTD_SL_CHE_1;
								half3 RTD_SL = RT_SL(input.uv, (half3)0.0, RTD_TEX_COL, RTD_R, RTD_SL_CHE_1);

								//RT_RL_SUB1
								half3 RTD_RL = RT_RL_SUB1(RTD_SL_CHE_1, RTD_RL_LARL_OO, RTD_RL_MAIN);

								half3 RTD_CA_OFF_OTHERS = (RTD_RL + RTD_SL);

								half3 add_light_output = RTD_CA_OFF_OTHERS * lightfos;


								#if N_F_USETLB_ON
									A_L_O += add_light_output;
								#else
									A_L_O = max (add_light_output,A_L_O);
								#endif

							}

						#endif

					#endif

				#endif



				#ifndef N_F_OFLMB_ON
					#if N_F_USETLB_ON
						color = main_light_output + A_L_O;
					#else
						color = max (main_light_output,A_L_O);
					#endif
				#else
					color = main_light_output;
				#endif



				#if N_F_TRANS_ON
					float Trans_Val = 1.0;
					#ifndef N_F_CO_ON
						Trans_Val = RTD_TRAN_OPA_Sli;
					#endif
				#else
					float Trans_Val = 1.0;
				#endif

				//RT_CA
				float3 RTD_CA = RT_CA(color);

//SSOL_NU
//SSOL
//#ifdef UNITY_COLORSPACE_GAMMA//SSOL
//_OutlineColor=float4(LinearToGamma22(_OutlineColor.rgb),_OutlineColor.a);//SSOL
//#endif//SSOL
//#if N_F_O_ON//SSOL
//float3 SSOLi=(float3)EdgDet(sceneUVs.xy);//SSOL
//#if N_F_O_MOTTSO_ON//SSOL
//float3 Init_FO=((RTD_CA*RTD_SON_CHE_1))*lerp((float3)1.0,_OutlineColor.rgb,SSOLi);//SSOL
//#else//SSOL
//float3 Init_FO=lerp((RTD_CA*RTD_SON_CHE_1),_OutlineColor.rgb,SSOLi);//SSOL
//#endif//SSOL
//#else//SSOL
float3 Init_FO=RTD_CA*RTD_SON_CHE_1;
//#endif//SSOL

				float fogFactor = input.positionWSAndFogFactor.w;

				color = MixFog(Init_FO, fogFactor);
				return half4(color, Trans_Val);

            }

            ENDHLSL
			 
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            Cull Off

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 xboxseries playstation wiiu switch 
#pragma target 2.0 //targetsc

            #pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma shader_feature_local_fragment N_F_TRANS_ON
			#pragma shader_feature_local_fragment N_F_TRANSAFFSHA_ON
			#pragma shader_feature_local_fragment N_F_CO_ON

			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

			#include "Assets/RealToon/RealToon Shaders/RealToon Core/URP/RT_URP_Core.hlsl"

			////uniform half _ReduceShadowPointLight; //Temporarily Removed
			////uniform half _PointLightSVD; //Temporarily Removed

			float3 _LightDirection;
			float3 _LightPosition;

			struct Attributes
			{

				float4 positionOS   : POSITION;
				float3 normalOS     : NORMAL;
				float2 texcoord     : TEXCOORD0;
			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				float4 tangentOS    : TANGENT;
float4 weights : BLENDWEIGHTS;//DOTS_LiBleSki_SC
uint4 indices : BLENDINDICES;//DOTS_LiBleSki_SC
//uint vertexID : SV_VertexID;//DOTS_CompDef_SC
			#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID

			};

			struct Varyings
			{

				float2 uv           : TEXCOORD0;
				float4 positionCS   : SV_POSITION;
				float4 projPos		: TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO

			};

			float4 GetShadowPositionHClip(Attributes input, float3 posOS, float3 normOS)
			{

				float3 positionWS = TransformObjectToWorld(posOS);
				float3 normalWS = TransformObjectToWorldDir(normOS);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float invNdotL = 1.0 - saturate(dot(lightDirectionWS, normalWS));
				float scale = invNdotL * _ShadowBias.y;

				positionWS = lightDirectionWS * _ShadowBias.xxx + positionWS;
				positionWS = normalWS * scale.xxx + positionWS;
				float4 positionCS = TransformWorldToHClip( positionWS );


				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE) + -_ReduSha * 0.01;
				#else
					positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE) + _ReduSha * 0.01;
				#endif


				return positionCS;

			}

			Varyings ShadowPassVertex(Attributes input)
			{

				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID (input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.uv = input.texcoord;

			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				float4 _LBS_CD_Position = 0;
				float3 _LBS_CD_Normal = 0;
				float4 _LBS_CD_Tangent = 0;
DOTS_LiBleSki(input.indices, input.weights, input.positionOS.xyz, input.normalOS.xyz, input.tangentOS.xyz, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_LiBleSki_SC
//DOTS_CompDef(input.vertexID, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_CompDef_SC
			#else
				float4 _LBS_CD_Position = input.positionOS;
				float3 _LBS_CD_Normal = input.normalOS;
				//float4 _LBS_CD_Tangent = input.tangentOS; //not currently needed
			#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(_LBS_CD_Position.xyz);

                output.positionCS = vertexInput.positionCS;
                output.projPos = ComputeScreenPos (output.positionCS);
				output.positionCS = GetShadowPositionHClip(input, _LBS_CD_Position.xyz, _LBS_CD_Normal);

				return output;

			}

			half4 ShadowPassFragment(Varyings input) : SV_TARGET
			{

				UNITY_SETUP_INSTANCE_ID (input);

				float4 objPos = mul ( unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) );
                float2 sceneUVs = (input.projPos.xy / input.projPos.w);

				half RTD_OB_VP_CAL = distance(objPos.rgb,_WorldSpaceCameraPos);
				half2 RTD_VD_Cal = (float2((sceneUVs.x * 2.0 - 1.0)*(_ScreenParams.r/_ScreenParams.g), sceneUVs.y * 2.0 - 1.0).rg*RTD_OB_VP_CAL);

                half2 _TexturePatternStyle_var = lerp( input.uv, RTD_VD_Cal, _TexturePatternStyle );
                half4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(_TexturePatternStyle_var, _MainTex));

				#if N_F_TRANSAFFSHA_ON

					//RT_TRANS_CO
					half RTD_TRAN_OPA_Sli;
					half RTD_CO;
					RT_TRANS_CO(input.uv, _MainTex_var, RTD_TRAN_OPA_Sli, RTD_CO);

					#if N_F_TRANS_ON
						#ifndef N_F_CO_ON
							float dither = tex3D(_DitherMaskLOD, float3(input.positionCS.xy * 0.25, RTD_TRAN_OPA_Sli * 0.99)).a;
							clip(saturate((0.74 > 0.5 ? (1.0 - (1.0 - 2.0 * (0.74 - 0.5)) * (1.0 - dither)) : (2.0 * 0.74 * dither))) - 0.5);
						#endif
					#endif

				#endif

				return 0;
			}

            ENDHLSL
        }

		Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            Cull [_Culling]

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 xboxseries playstation wiiu switch 
#pragma target 2.0 //targetdo

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "Assets/RealToon/RealToon Shaders/RealToon Core/URP/RT_URP_Core.hlsl"

			struct Attributes
			{
				float4 position     : POSITION;
				float2 texcoord     : TEXCOORD0;
			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				float3 normalOS     : NORMAL;
				float4 tangentOS    : TANGENT;
float4 weights : BLENDWEIGHTS;//DOTS_LiBleSki_DO
uint4 indices : BLENDINDICES;//DOTS_LiBleSki_DO
//uint vertexID : SV_VertexID;//DOTS_CompDef_DO
			#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float2 uv           : TEXCOORD0;
				float4 positionCS   : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings DepthOnlyVertex(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				float4 _LBS_CD_Position = 0;
				float3 _LBS_CD_Normal = 0;
				float4 _LBS_CD_Tangent = 0;
DOTS_LiBleSki(input.indices, input.weights, input.position.xyz, input.normalOS.xyz, input.tangentOS.xyz, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_LiBleSki_DO
//DOTS_CompDef(input.vertexID, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_CompDef_DO
			#else
				float4 _LBS_CD_Position = input.position;
				//float3 _LBS_CD_Normal = input.normalOS; //not currently needed
				//float4 _LBS_CD_Tangent = input.tangentOS; //not currently needed
			#endif

				output.positionCS = TransformObjectToHClip(_LBS_CD_Position.xyz);
				return output;
			}

			half4 DepthOnlyFragment(Varyings input) : SV_TARGET
			{
				return 0.0;
			}


            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
			#pragma prefer_hlslcc gles
			#pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 xboxseries playstation wiiu switch 
#pragma target 2.0 //targetdn

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Assets/RealToon/RealToon Shaders/RealToon Core/URP/RT_URP_Core.hlsl"

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float4 tangentOS    : TANGENT;
				float2 texcoord     : TEXCOORD0;
				float3 normal       : NORMAL;
			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
float4 weights : BLENDWEIGHTS;//DOTS_LiBleSki_DN
uint4 indices : BLENDINDICES;//DOTS_LiBleSki_DN
//uint vertexID : SV_VertexID;//DOTS_CompDef_DN
			#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS   : SV_POSITION;
				float2 uv           : TEXCOORD1;
				float3 normalWS     : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings DepthNormalsVertex(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				float4 _LBS_CD_Position = 0;
				float3 _LBS_CD_Normal = 0;
				float4 _LBS_CD_Tangent = 0;
DOTS_LiBleSki(input.indices, input.weights, input.positionOS.xyz, input.normal.xyz, input.tangentOS.xyz, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_LiBleSki_DN
//DOTS_CompDef(input.vertexID, (float3)_LBS_CD_Position, _LBS_CD_Normal, (float3)_LBS_CD_Tangent);//DOTS_CompDef_DN
			#else
				float4 _LBS_CD_Position = input.positionOS;
				float3 _LBS_CD_Normal = input.normal;
				float4 _LBS_CD_Tangent = input.tangentOS;
			#endif

				output.uv         = TRANSFORM_TEX(input.texcoord, _MainTex);
				output.positionCS = TransformObjectToHClip(_LBS_CD_Position.xyz);

				VertexNormalInputs normalInput = GetVertexNormalInputs(_LBS_CD_Normal, _LBS_CD_Tangent);
				output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);

				return output;
			}

			float4 DepthNormalsFragment(Varyings input) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_MainTex, sampler_MainTex)).a, (_MainColor * _MaiColPo) , _Cutout);
				return float4(PackNormalOctRectEncode(TransformWorldToViewDir(input.normalWS, true)), 0.0, 0.0);
			}

            ENDHLSL
        }

		Pass
		{
			Name "Meta"
			Tags{"LightMode" = "Meta"}

			Cull Off

			HLSLPROGRAM
			#pragma prefer_hlslcc gles
			#pragma only_renderers d3d9 d3d11 vulkan glcore gles3 gles metal xboxone ps4 xboxseries playstation wiiu switch 
#pragma target 2.0 //targetm

			#pragma vertex UniversalVertexMeta
			#pragma fragment UniversalFragmentMeta

			#pragma shader_feature_local_fragment N_F_SL_ON

			#include "Assets/RealToon/RealToon Shaders/RealToon Core/URP/RT_URP_Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float3 normalOS     : NORMAL;
				float2 uv0          : TEXCOORD0;
				float2 uv1          : TEXCOORD1;
				float2 uv2          : TEXCOORD2;
				#ifdef _TANGENT_TO_WORLD
				float4 tangentOS     : TANGENT;
				#endif
			};

			struct Varyings
			{
				float4 positionCS   : SV_POSITION;
				float2 uv           : TEXCOORD0;
			};

			Varyings UniversalVertexMeta(Attributes input)
			{
				Varyings output;
				output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST);
				output.uv = TRANSFORM_TEX(input.uv0, _MainTex);
				return output;
			}

			half4 UniversalFragmentMeta(Varyings input) : SV_Target
			{

				half4 _MainTex_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(input.uv, _MainTex));

				//
				#ifdef UNITY_COLORSPACE_GAMMA
					_MainColor = float4(LinearToGamma22(_MainColor.rgb),_MainColor.a);
				#endif
				//
	
				half4 RTD_TEX_COL = _MainTex_var * (_MainColor * _MaiColPo);

				//RT_SL
				half3 RTD_SL_CHE_1;
				half3 RTD_SL = RT_SL(input.uv, (half3)0.0 , RTD_TEX_COL.rgb, (half3)0.0, RTD_SL_CHE_1);

				MetaInput metaInput;
				metaInput.Albedo = RTD_TEX_COL.rgb;
				metaInput.SpecularColor = float3(0.0, 0.0, 0.0);
				metaInput.Emission = RTD_SL;

				return MetaFragment(metaInput);
			}

			ENDHLSL
		}

    }

    FallBack "Hidden/InternalErrorShader"

    CustomEditor "RealToonShaderGUI_URP_SRP"
}
