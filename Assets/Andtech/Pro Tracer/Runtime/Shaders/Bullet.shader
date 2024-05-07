
Shader "Andtech/Bullet" {

	Properties{
		// Main Options
		[HDR] [Gamma] _Color("_Color", Color) = (8.5, 3.8, 1.3, 1.0)
		_SoftParticlesNearFadeDistance("_SoftParticlesNearFadeDistance", Float) = 0.0
		_SoftParticlesFarFadeDistance("_SoftParticlesFarFadeDistance", Float) = 1.0
		_CameraNearFadeDistance("_CameraNearFadeDistance", Float) = 1.0
		_CameraFarFadeDistance("_CameraFarFadeDistance", Float) = 2.0
		// Geometry Options
		_ResizeAmount("_ResizeAmount", Range(0.0, 1.0)) = 0.8
		_GeoNearResizeDistance("_GeoNearResizeDistance", Float) = 2.0
		_GeoFarResizeDistance("_GeoFarResizeDistance", Float) = 500.0
		_GeoNearFadeDistance("_GeoNearFadeDistance", Float) = 300.0
		_GeoFarFadeDistance("_GeoFarFadeDistance", Float) = 500.0
		// Internal
		[HideInInspector]
		[Toggle] _SoftParticlesEnabled("_SoftParticlesEnabled", Float) = 0.0
		[HideInInspector]
		_SoftParticleFadeParams("_SoftParticleFadeParams", Vector) = (0.0, 0.0, 0.0, 0.0)
		[HideInInspector]
		[Toggle] _CameraFadingEnabled("_CameraFadingEnabled", Float) = 0.0
		[HideInInspector]
		_CameraFadeParams("_CameraFadeParams", Vector) = (0.0, 0.0, 0.0, 0.0)
		[HideInInspector]
		[Toggle] _GeoResizingEnabled("_GeoResizingEnabled", Float) = 0.0
		[HideInInspector]
		[Toggle] _GeoFadingEnabled("_GeoFadingEnabled", Float) = 0.0
		[HideInInspector]
		_GeometryParams("_GeometryParams", Vector) = (0.0, 0.0, 0.0, 0.0)
	}

	SubShader {
		Blend SrcAlpha One
		ZWrite Off
		Cull Back

		Tags {
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"LightMode" = "Always"
			"PassFlags" = "OnlyDirectional"
		}
		
		LOD 200

		Pass {
			CGPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#pragma multi_compile SOFTPARTICLES_ON
			#pragma shader_feature_local _FADING_ON
			#pragma shader_feature_local _GEO_FADING_ON
			#pragma shader_feature_local _GEO_RESIZING_ON
			#include "UnityCG.cginc"
			#include "UnityInstancing.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				UNITY_FOG_COORDS(0)
#if defined(_FADING_ON)
				float4 projectedPosition : TEXCOORD1;
#endif
			};

			float4 _SoftParticleFadeParams;
			float4 _CameraFadeParams;
			float4 _GeometryParams;
			float _ResizeAmount;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(half4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)
			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

			static const half4 COLOR_CLEAR = half4(0.0, 0.0, 0.0, 0.0);

			#define UNITY_MATRIX_M unity_ObjectToWorld
			#define SOFT_PARTICLE_NEAR_FADE _SoftParticleFadeParams.x
			#define SOFT_PARTICLE_INV_FADE_DISTANCE _SoftParticleFadeParams.y
			#define CAMERA_NEAR_FADE _CameraFadeParams.x
			#define CAMERA_INV_FADE_DISTANCE _CameraFadeParams.y
			#define GEO_NEAR_RESIZE _GeometryParams.x
			#define GEO_INV_RESIZE_DISTANCE _GeometryParams.y
			#define GEO_NEAR_FADE _GeometryParams.z
			#define	GEO_INV_FADE_DISTANCE _GeometryParams.w

			v2f vert (appdata v) {
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);

				float4 world = mul(UNITY_MATRIX_M, v.vertex);
				float d = distance(_WorldSpaceCameraPos, world);
				// Compute fade out factor
				float geoFade = 1.0 - saturate((d - GEO_NEAR_FADE) * GEO_INV_FADE_DISTANCE);

#if defined(_GEO_RESIZING_ON)
				float resize = saturate((d - GEO_NEAR_RESIZE) * GEO_INV_RESIZE_DISTANCE) * _ResizeAmount;
#if defined(_GEO_FADING_ON)
				// Apply fade out to resize
				resize *= geoFade;
#endif
				// Apply resize to vertex
				float3 normal = UnityObjectToWorldNormal(v.normal);
				world.xyz += resize * normal;
#endif
				o.vertex = UnityWorldToClipPos(world);
				o.color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
#if defined(_GEO_FADING_ON)
				// Apply fade out to color
				o.color.a *= geoFade;
#endif
#if defined(_FADING_ON)
				float4 clipPosition = UnityObjectToClipPos(v.vertex);
				o.projectedPosition = ComputeScreenPos(clipPosition);
				COMPUTE_EYEDEPTH(o.projectedPosition.z);
#endif

				UNITY_TRANSFER_FOG(o, o.vertex);

				return o;
			}
			
			half4 frag(v2f i) : SV_Target{
				half4 col = i.color;
#if defined(_FADING_ON)
				// Per-pixel camera fade
				float cameraFade = saturate((i.projectedPosition.z - CAMERA_NEAR_FADE) * CAMERA_INV_FADE_DISTANCE);
				col.a *= cameraFade;

#if defined(SOFTPARTICLES_ON)
				// Depth-based soft particles
				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projectedPosition)));
				float softFade = saturate(((sceneZ - SOFT_PARTICLE_NEAR_FADE) - i.projectedPosition.z) * SOFT_PARTICLE_INV_FADE_DISTANCE);
				col.a *= softFade;
#endif
#endif

				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, COLOR_CLEAR);

				return col;
			}
			ENDCG
		}
	}

	CustomEditor "Andtech.ProTracer.Editor.BulletShaderGUI"
}
