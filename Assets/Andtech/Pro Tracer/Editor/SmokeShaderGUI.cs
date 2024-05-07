using UnityEditor;
using UnityEngine;

namespace Andtech.ProTracer.Editor
{

	public class SmokeShaderGUI : BaseShaderGUI
	{

		protected static class Keywords
		{
			public const string _FADING_ON = nameof(_FADING_ON);
		}

		// Main Options
		private ShaderProperty _Color;
		private ShaderProperty _AlphaCutoff;
		private ShaderProperty _MainTex;
		private ShaderProperty _SoftParticlesEnabled;
		private ShaderProperty _SoftParticlesNearFadeDistance;
		private ShaderProperty _SoftParticlesFarFadeDistance;
		private ShaderProperty _CameraFadingEnabled;
		private ShaderProperty _CameraNearFadeDistance;
		private ShaderProperty _CameraFarFadeDistance;
		// Lighting Options
		private ShaderProperty _EmissionColor;
		private ShaderProperty _ShadowIntensity;
		// Miscellaneous Options
		private ShaderProperty _MainTexScaleX;
		private ShaderProperty _MainTexScaleY;
		private ShaderProperty _ScrollSpeedU;
		private ShaderProperty _ScrollSpeedV;
		// Internal
		private ShaderProperty _SoftParticleFadeParams;
		private ShaderProperty _CameraFadeParams;
		private ShaderProperty _SmokeParams;

		protected override void FindProperties()
		{
			// Main Options
			_Color = GetProperty(nameof(_Color))
				.SetTextContent("Color", "The color of the smoke.");
			_AlphaCutoff = GetProperty(nameof(_AlphaCutoff))
				.SetTextContent("Alpha Cutoff", "The opacity value under which to hide pixels. (Use this for a \"dissolving\" smoke effect)");
			_MainTex = GetProperty(nameof(_MainTex))
				.SetTextContent("Albedo", "Albedo (RGBA)");
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
			// Lighting Options
			_EmissionColor = GetProperty(nameof(_EmissionColor))
				.SetTextContent("Emission Color", "Emission (RGB)");
			_ShadowIntensity = GetProperty(nameof(_ShadowIntensity))
				.SetTextContent("Shadow Intensity", "The opacity of shadows casted by this material");
			// Miscellaneous Options
			_MainTexScaleX = GetProperty(nameof(_MainTexScaleX))
				.SetTextContent("Tiling (X)", "The X scale of the texture");
			_MainTexScaleY = GetProperty(nameof(_MainTexScaleY))
				.SetTextContent("Tiling (Y)", "The Y scale of the texture");
			_ScrollSpeedU = GetProperty(nameof(_ScrollSpeedU))
				.SetTextContent("Scroll Speed (Horizontal)", "Speed at which the U texture coordinate is scrolled.");
			_ScrollSpeedV = GetProperty(nameof(_ScrollSpeedV))
				.SetTextContent("Scroll Speed (Vertical)", "Speed at which the V texture coordinate is scrolled.");
			// Internal
			_SoftParticleFadeParams = GetProperty(nameof(_SoftParticleFadeParams));
			_CameraFadeParams = GetProperty(nameof(_CameraFadeParams));
			_SmokeParams = GetProperty(nameof(_SmokeParams));
		}

		protected override void DrawGUI()
		{
			// Main Options
			GUILayout.Label("Main Options", EditorStyles.boldLabel);
			Color();
			MainTex();
			SoftParticles();
			CameraFading();
			EditorGUILayout.Space();

			// Lighting Options
			GUILayout.Label("Lighting Options", EditorStyles.boldLabel);
			DrawProperty(_EmissionColor);
			DrawProperty(_ShadowIntensity);
			EditorGUILayout.Space();

			// Miscellaneous Options
			GUILayout.Label("Miscellaneous Options", EditorStyles.boldLabel);
			//DrawProperty(_MainTexScaleX);
			//DrawProperty(_MainTexScaleY);
			DrawProperty(_ScrollSpeedU);
			DrawProperty(_ScrollSpeedV);
			EditorGUILayout.Space();

			// Advanced Options
			GUILayout.Label("Advanced Options", EditorStyles.boldLabel);
			Instancing();

			void Color()
			{
				DrawProperty(_Color);
				DrawProperty(_AlphaCutoff);
			}

			void MainTex()
			{
				DrawProperty(_MainTex);
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
			TextureScrolling();

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

			void TextureScrolling()
			{
				var mainTexScaleOffset = _MainTex.MaterialProperty.textureScaleAndOffset;

				var scrollParams = new Vector4
				{
					x = _MainTexScaleX.Float,
					y = _MainTexScaleY.Float,
					z = _ScrollSpeedU.Float / mainTexScaleOffset.x,
					w = _ScrollSpeedV.Float / mainTexScaleOffset.y
				};
				_SmokeParams.Vector = scrollParams;
			}
		}
	}
}
