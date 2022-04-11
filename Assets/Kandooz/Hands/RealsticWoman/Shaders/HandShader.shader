// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Kandooz/HandShader"

{
	Properties
	{
		[HDR]_AlbedoTint("Albedo Tint", Color) = (1,1,1,0)
		[HDR][Gamma]_AlbedoMap("Albedo Map", 2D) = "white" {}
		_NormalStrength("Normal Strength", Range( 0 , 2)) = 0
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_CombinedMap("Combined Map", 2D) = "white" {}
		_AO("AO", Float) = 1
		_Glossiness("Glossiness", Range( 0 , 1)) = 0
		_Metalness("Metalness", Range( 0 , 1)) = 0
		_FingerTips("Finger tips mask",2d)="white"{}
		_NailsColor("Nails Color",Color)=(0,0,0,1)
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		[HDR]_TranslucencyColor("Translucency Color", Color) = (0,0,0,0)
		_HandSurface("HandSurface", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform sampler2D _NormalMap;
		uniform sampler2D _FingerTips;
		uniform float4 _NormalMap_ST;

		uniform float _HandSurface;
		uniform float _NormalStrength;
		uniform sampler2D _AlbedoMap;
		uniform float4 _AlbedoMap_ST;
		uniform float4 _AlbedoTint;
		uniform sampler2D _CombinedMap;
		uniform float4 _CombinedMap_ST;
		uniform float _Metalness;
		uniform float _Glossiness;
		uniform float _AO;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float4 _TranslucencyColor;
		uniform float4 _NailsColor;
		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
				float3 lightAtten = gi.light.color;
			#else
				float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 lerpResult52 =  UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) ) ;
			float3 lerpResult64 = lerp( float3(0,0,1) , lerpResult52 , _NormalStrength);
			o.Normal = lerpResult64;
			float2 uv_AlbedoMap = i.uv_texcoord * _AlbedoMap_ST.xy + _AlbedoMap_ST.zw;
			float4 lerpResult62 =  tex2D( _AlbedoMap, uv_AlbedoMap ) ;
			float fingerNailsMask = tex2D(_FingerTips,uv_AlbedoMap).r*_NailsColor.a;
			float3 handColor=( lerpResult62 * _AlbedoTint ).rgb;

			o.Albedo = handColor * (1 - fingerNailsMask) + _NailsColor.rgb * fingerNailsMask;
			float2 uv_CombinedMap = i.uv_texcoord * _CombinedMap_ST.xy + _CombinedMap_ST.zw;
			float4 lerpResult82 = tex2D( _CombinedMap, uv_CombinedMap );
			float4 break89 = lerpResult82;
			o.Metallic = ( break89.r * _Metalness );
			o.Smoothness = ( break89.g * _Glossiness );
			o.Occlusion = ( break89.b * _AO );
			o.Translucency = ( _TranslucencyColor * break89.a ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "Kandooz.HandShaderEditor"

}
