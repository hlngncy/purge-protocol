using UnityEditor;
using UnityEngine;

namespace Andtech.ProTracer.Editor
{

	public class BulletShaderGUI : BaseShaderGUI
	{

		protected static class Keywords
		{
			public const string _FADING_ON = nameof(_FADING_ON);
			public const string _GEO_RESIZING_ON = nameof(_GEO_RESIZING_ON);
			public const string _GEO_FADING_ON = nameof(_GEO_FADING_ON);
		}

		// Main Options
		private ShaderProperty _Color;
		private ShaderProperty _SoftParticlesEnabled;
		private ShaderProperty _SoftParticlesNearFadeDistance;
		private ShaderProperty _SoftParticlesFarFadeDistance;
		private ShaderProperty _CameraFadingEnabled;
		private ShaderProperty _CameraNearFadeDistance;
		private ShaderProperty _CameraFarFadeDistance;
		// Geometry Options
		private ShaderProperty _GeoResizingEnabled;
		private ShaderProperty _ResizeAmount;
		private ShaderProperty _GeoNearResizeDistance;
		private ShaderProperty _GeoFarResizeDistance;
		private ShaderProperty _GeoFadingEnabled;
		private ShaderProperty _GeoNearFadeDistance;
		private ShaderProperty _GeoFarFadeDistance;
		// Internal
		private ShaderProperty _SoftParticleFadeParams;
		private ShaderProperty _CameraFadeParams;
		private ShaderProperty _GeometryParams;

		protected override void FindProperties()
		{
			// Main Options
			_Color = GetProperty(nameof(_Color))
				.SetTextContent("Color", "The color of the bullet. This value should typically have a high HDR intensity.");
			_SoftParticlesEnabled = GetProperty(nameof(_SoftParticlesEnabled))
				.SetTextContent("Soft Particles", "Fade out geometry when it gets close to the surface of objects written into the depth buffer.");
			_SoftParticlesNearFadeDistance = GetProperty(nameof(_SoftParticlesNearFadeDistance))
				.SetTextContent("Near Fade", "Soft Particles near fade distance.");
			_SoftParticlesFarFadeDistance = GetProperty(nameof(_SoftParticlesFarFadeDistance))
				.SetTextContent("Far Fade", "Soft Particles far fade distance.");
			_CameraFadingEnabled = GetProperty(nameof(_CameraFadingEnabled))
				.SetTextContent("Camera Fading", "Fade out geometry when it gets close to the camera?");
			_CameraNearFadeDistance = GetProperty(nameof(_CameraNearFadeDistance))
				.SetTextContent("Near Fade", "Camera near fade distance.");
			_CameraFarFadeDistance = GetProperty(nameof(_CameraFarFadeDistance))
				.SetTextContent("Far Fade", "Camera far fade distance.");
			// Geometry Options
			_GeoResizingEnabled = GetProperty(nameof(_GeoResizingEnabled))
				.SetTextContent("Scale With Distance", "Enable distance resizing?")
				.SetKeyword(Keywords._GEO_RESIZING_ON);
			_ResizeAmount = GetProperty(nameof(_ResizeAmount))
				.SetTextContent("Intensity", "The amount far vertices will be scaled.");
			_GeoNearResizeDistance = GetProperty(nameof(_GeoNearResizeDistance))
				.SetTextContent("Near Distance", "Minimum resize distance. Vertices begin resizing at this distance.");
			_GeoFarResizeDistance = GetProperty(nameof(_GeoFarResizeDistance))
				.SetTextContent("Far Distance", "Maximum resize distance. Vertices reach their max size at this distance.");
			_GeoFadingEnabled = GetProperty(nameof(_GeoFadingEnabled))
				.SetTextContent("Fade Out", "Fade out geometry when it gets far from the camera? This makes bullets disappear smoothly into the distance.")
				.SetKeyword(Keywords._GEO_FADING_ON);
			_GeoNearFadeDistance = GetProperty(nameof(_GeoNearFadeDistance))
				.SetTextContent("Near Fade", "Camera near fade distance.");
			_GeoFarFadeDistance = GetProperty(nameof(_GeoFarFadeDistance))
				.SetTextContent("Far Fade", "Camera far fade distance.");
			// Internal
			_SoftParticleFadeParams = GetProperty(nameof(_SoftParticleFadeParams));
			_CameraFadeParams = GetProperty(nameof(_CameraFadeParams));
			_GeometryParams = GetProperty(nameof(_GeometryParams));
		}

		protected override void DrawGUI()
		{
			// Main Options
			GUILayout.Label("Main Options", EditorStyles.boldLabel);
			Color();
			SoftParticles();
			CameraFading();
			EditorGUILayout.Space();

			// Geometry Options
			GUILayout.Label("Geometry Options", EditorStyles.boldLabel);
			DistanceResizing();
			GeoFading();
			EditorGUILayout.Space();

			// Advanced Options
			GUILayout.Label("Advanced Options", EditorStyles.boldLabel);
			Instancing();

			void Color()
			{
				DrawProperty(_Color);
			}

			void SoftParticles()
			{
				DrawProperty(_SoftParticlesEnabled);
				if (_SoftParticlesEnabled.Toggle)
				{
					DrawProperty(_SoftParticlesNearFadeDistance, DefaultIndentation);
					DrawProperty(_SoftParticlesFarFadeDistance, DefaultIndentation);
				}
			}

			void CameraFading()
			{
				DrawProperty(_CameraFadingEnabled);
				if (_CameraFadingEnabled.Toggle)
				{
					DrawProperty(_CameraNearFadeDistance, DefaultIndentation);
					DrawProperty(_CameraFarFadeDistance, DefaultIndentation);
				}
			}

			void DistanceResizing()
			{
				DrawProperty(_GeoResizingEnabled);
				if (_GeoResizingEnabled.Toggle)
				{
					DrawProperty(_GeoNearResizeDistance, DefaultIndentation);
					DrawProperty(_GeoFarResizeDistance, DefaultIndentation);
					DrawProperty(_ResizeAmount, DefaultIndentation);
				}
			}

			void GeoFading()
			{
				DrawProperty(_GeoFadingEnabled);
				if (_GeoFadingEnabled.Toggle)
				{
					DrawProperty(_GeoNearFadeDistance, DefaultIndentation);
					DrawProperty(_GeoFarFadeDistance, DefaultIndentation);
				}
			}

			void Instancing()
			{
				MaterialEditor.EnableInstancingField();
			}
		}

		protected override void MaterialChanged()
		{
			SetMaterialKeywords(Target);
		}

		private void SetMaterialKeywords(Material material)
		{
			FadingOn();
			SoftParticles();
			CameraFading();
			DistanceResizing();
			GeoFading();

			void FadingOn()
			{
				if (_SoftParticlesEnabled.Toggle || _CameraFadingEnabled.Toggle)
				{
					material.EnableKeyword(Keywords._FADING_ON);
				}
				else
				{
					material.DisableKeyword(Keywords._FADING_ON);
				}
			}

			void SoftParticles()
			{
				Vector2 softParticleFadeParams;
				if (_SoftParticlesEnabled.Toggle)
				{
					softParticleFadeParams = GetFadeParams(_SoftParticlesNearFadeDistance.Float, _SoftParticlesFarFadeDistance.Float);
				}
				else
				{
					softParticleFadeParams = DefaultFadeParams;
				}

				_SoftParticleFadeParams.Vector = softParticleFadeParams;
			}

			void CameraFading()
			{
				Vector2 cameraFadeParams;
				if (_CameraFadingEnabled.Toggle)
				{
					cameraFadeParams = GetFadeParams(_CameraNearFadeDistance.Float, _CameraFarFadeDistance.Float);
				}
				else
				{
					cameraFadeParams = DefaultFadeParams;
				}

				_CameraFadeParams.Vector = cameraFadeParams;
			}

			void DistanceResizing()
			{
				Vector2 geoResizeParams;
				if (_GeoResizingEnabled.Toggle)
				{
					geoResizeParams = GetFadeParams(_GeoNearResizeDistance.Float, _GeoFarResizeDistance.Float);
					material.EnableKeyword(_GeoResizingEnabled.Keyword);
				}
				else
				{
					geoResizeParams = DefaultFadeParams;
					material.DisableKeyword(_GeoResizingEnabled.Keyword);
				}

				var vector = _GeometryParams.Vector;
				vector.x = geoResizeParams.x;
				vector.y = geoResizeParams.y;
				_GeometryParams.Vector = vector;
			}

			void GeoFading()
			{
				Vector2 geoFadeParams;
				if (_GeoFadingEnabled.Toggle)
				{
					geoFadeParams = GetFadeParams(_GeoNearFadeDistance.Float, _GeoFarFadeDistance.Float);
					material.EnableKeyword(_GeoFadingEnabled.Keyword);
				}
				else
				{
					geoFadeParams = DefaultFadeParams;
					material.DisableKeyword(_GeoFadingEnabled.Keyword);
				}

				var vector = _GeometryParams.Vector;
				vector.z = geoFadeParams.x;
				vector.w = geoFadeParams.y;
				_GeometryParams.Vector = vector;
			}
		}
	}
}
