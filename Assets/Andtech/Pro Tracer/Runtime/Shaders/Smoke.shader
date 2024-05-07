
Shader "Andtech/Smoke" {
	Properties {
		// Main Options
		_Color("_Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_AlphaCutoff("Color", Range(0.0, 1.0)) = 1.0
		_MainTex("_MainTex", 2D) = "white" {}
		_SoftParticlesNearFadeDistance("_SoftParticlesNearFadeDistance", Float) = 0.0
		_SoftParticlesFarFadeDistance("_SoftParticlesFarFadeDistance", Float) = 1.0
		_CameraNearFadeDistance("_CameraNearFadeDistance", Float) = 1.0
		_CameraFarFadeDistance("_CameraFadeDistance", Float) = 2.0
		// Lighting Options
		_EmissionColor("_EmissionColor", Color) = (0.2, 0.2, 0.2, 1.0)
		_ShadowIntensity("_ShadowIntensity", Range(0.0, 1.0)) = 1.0
		// Miscellaneous Options
		_MainTexScaleX("_MainTexScaleX", Float) = 1.0
		_MainTexScaleY("_MainTexScaleY", Float) = 1.0
		_ScrollSpeedU("_ScrollSpeedU", Float) = 1.0
		_ScrollSpeedV("_ScrollSpeedV", Float) = 1.0
		// Internal
		[HideInInspector]
		[Toggle] _SoftParticlesEnabled("_SoftParticlesEnabled", Float) = 0.0
		[HideInInspector]
		[Toggle] _CameraFadingEnabled("_CameraFadingEnabled", Float) = 0.0
		[HideInInspector]
		_SoftParticleFadeParams("_SoftParticleFadeParams", Vector) = (0.0, 999, 0.0, 0.0)
		[HideInInspector]
		_CameraFadeParams("_CameraFadeParams", Vector) = (0.0, 999, 0.0, 0.0)
		[HideInInspector]
		_SmokeParams("_SmokeParams", Vector) = (1.0, 1.0, 0.125, 0.25)
	}

	SubShader {

		Pass {
			Name "ShadowCaster"
			Tags {
				"LightMode" = "ShadowCaster"
			}

			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			Cull Off

			CGPROGRAM
			#pragma target 3.0
			#pragma shader_feature_local _ _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityStandardParticleShadow.cginc"

			float _ShadowIntensity;
			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(half4, _Color)
			UNITY_DEFINE_INSTANCED_PROP(float, _AlphaCutoff)
			UNITY_DEFINE_INSTANCED_PROP(half4, _SmokeParams)
			UNITY_INSTANCING_BUFFER_END(Props)

			void vert(VertexInput v,
#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
				out VertexOutputShadowCaster o,
#endif
				out float4 opos : SV_POSITION) {
				UNITY_SETUP_INSTANCE_ID(v);
				TRANSFER_SHADOW_CASTER_NOPOS(o, opos)
#ifdef UNITY_STANDARD_USE_SHADOW_UVS
				float4 scrollParams = UNITY_ACCESS_INSTANCED_PROP(Props, _SmokeParams);
				o.texcoord = v.texcoords.xy;
				o.texcoord.x = o.texcoord.x * scrollParams.x - _Time[1] * scrollParams.z;
				o.texcoord.y = o.texcoord.y * scrollParams.y - _Time[1] * scrollParams.w;
				o.texcoord = TRANSFORM_TEX(o.texcoord.xy, _MainTex);
				// Transfer color
				o.color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * v.color;
				// Performance hack: place instanced _AlphaCutoff in red channel
				o.color.r = UNITY_ACCESS_INSTANCED_PROP(Props, _AlphaCutoff);
#endif
			}

			half4 frag(
#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
				VertexOutputShadowCaster i
#endif
#ifdef UNITY_STANDARD_USE_DITHER_MASK
				, UNITY_VPOS_TYPE vpos : VPOS
#endif
			) : SV_Target{
			#ifdef UNITY_STANDARD_USE_SHADOW_UVS
					half4 albedo = tex2D(_MainTex, i.texcoord);
					half alpha = i.color.r - (1.0 - albedo.a);
					alpha = saturate(alpha) * i.color.a * _ShadowIntensity;

#ifdef UNITY_STANDARD_USE_DITHER_MASK
					half alphaRef = tex3D(_DitherMaskLOD, float3(vpos.xy * 0.25, alpha * 0.9375)).a;
					clip(alphaRef - 0.01);
#else
					clip(alpha - 0.5);
#endif
				#endif

				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}

		Tags {
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"PreviewType" = "Plane"
			"PerformanceChecks" = "False"
		}

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off

		LOD 300

		CGPROGRAM
		#pragma target 3.5
		#pragma surface surf Standard vertex:vert alpha:fade
		#pragma multi_compile_instancing
		#pragma multi_compile SOFTPARTICLES_ON
		#pragma shader_feature_local _FADING_ON

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		half4 _EmissionColor;
		float4 _SoftParticleFadeParams;
		float4 _CameraFadeParams;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_DEFINE_INSTANCED_PROP(half4, _Color)
		UNITY_DEFINE_INSTANCED_PROP(float, _AlphaCutoff)
		UNITY_DEFINE_INSTANCED_PROP(half4, _SmokeParams)
		UNITY_INSTANCING_BUFFER_END(Props)
		UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

		static const half4 COLOR_CLEAR = half4(0.0, 0.0, 0.0, 0.0);
		static const float4 TANGENT_STANDARD = fixed4(0.0, 1.0, 0.0, 1.0);

		#define UNITY_MATRIX_M unity_ObjectToWorld
		#define SOFT_PARTICLE_NEAR_FADE _SoftParticleFadeParams.x
		#define SOFT_PARTICLE_INV_FADE_DISTANCE _SoftParticleFadeParams.y
		#define CAMERA_NEAR_FADE _CameraFadeParams.x
		#define CAMERA_INV_FADE_DISTANCE _CameraFadeParams.y

		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			half4 color : COLOR;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct Input {
			float3 worldNormal : NORMAL;
			half4 color : COLOR;
			float cutoff;

			float3 view;
			float2 uv_MainTex : TEXCOORD0;
#if defined(_FADING_ON)
			float4 projectedPosition;
#endif
		};

		float3 reject(float3 a, float3 b) {
			return a - dot(a, b) / dot(b, b) * b;
		}

		void vert(inout appdata v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);

			UNITY_SETUP_INSTANCE_ID(v);

			// Apply custom scale/offset
			float4 scrollParams = UNITY_ACCESS_INSTANCED_PROP(Props, _SmokeParams);
			v.texcoord.x = v.texcoord.x * scrollParams.x - _Time[1] * scrollParams.z;
			v.texcoord.y = v.texcoord.y * scrollParams.y - _Time[1] * scrollParams.w;
			v.color *= UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
			// Compute view vector (for fresnel)
			o.view = reject(
				WorldSpaceViewDir(v.vertex),
				UnityObjectToWorldDir(TANGENT_STANDARD)
			);
			o.cutoff = UNITY_ACCESS_INSTANCED_PROP(Props, _AlphaCutoff);
#if defined(_FADING_ON)
			float4 clipPosition = UnityObjectToClipPos(v.vertex);
			o.projectedPosition = ComputeScreenPos(clipPosition);
			COMPUTE_EYEDEPTH(o.projectedPosition.z);
#endif
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			// Compute fresnel
			fixed fresnel = dot(normalize(IN.worldNormal), normalize(IN.view));
			fresnel = fresnel * fresnel * fresnel * fresnel;
			fresnel = saturate(fresnel);

			// Compile final color
			half4 albedo = tex2D(_MainTex, IN.uv_MainTex);
			float cutoff = IN.cutoff; 
			o.Albedo = IN.color.rgb * albedo.rgb;
			o.Alpha = cutoff - (1.0 - albedo.a);
			o.Emission = _EmissionColor;
#if defined(_FADING_ON)
			// Per-pixel camera fade
			float cameraFade = saturate((IN.projectedPosition.z - CAMERA_NEAR_FADE) * CAMERA_INV_FADE_DISTANCE);
			o.Alpha *= cameraFade;
#if defined(SOFTPARTICLES_ON)
			// Depth-based soft particles
			float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.projectedPosition)));
			float softFade = saturate(((sceneZ - SOFT_PARTICLE_NEAR_FADE) - IN.projectedPosition.z) * SOFT_PARTICLE_INV_FADE_DISTANCE);
			o.Alpha *= softFade;
#endif
#endif
			o.Alpha = saturate(o.Alpha * fresnel) * IN.color.a;
		}
		ENDCG
	}

		Fallback "VertexLit"
		CustomEditor "Andtech.ProTracer.Editor.SmokeShaderGUI"
}
