using System;
using UnityEngine;

namespace Andtech.ProTracer
{

	/// <summary>
	/// Choose how textures are applied to a smoke trail.
	/// </summary>
	/// <seealso cref="SmokeTrail"/>
	public enum SmokeTextureMode
	{
		/// <summary>
		/// Map the texture once along the entire length of the line.
		/// </summary>
		Stretch,
		/// <summary>
		/// Repeat the texture along the line, based on its length in world units. To set the tiling rate, use Material.SetTextureScale.
		/// </summary>
		Tile
	}

	/// <summary>
	/// Choose how a smoke trail disappears.
	/// </summary>
	/// <seealso cref="SmokeTrail"/>
	[Flags]
	public enum SmokeFadeMode
	{
		/// <summary>
		/// No fade.
		/// </summary>
		None = 0,
		/// <summary>
		/// Fade by shrinking the transform scale.
		/// </summary>
		Shrink = 1,
		/// <summary>
		/// Fade by growing the transform scale.
		/// </summary>
		Grow = 2,
		/// <summary>
		/// Fade by opacity.
		/// </summary>
		FadeOut = 4,
		/// <summary>
		/// Fade by alpha erosion.
		/// </summary>
		Dissolve = 8
	}

	/// <summary>
	/// A smoke trail graphic object.
	/// </summary>
	public class SmokeTrail : TracerObject
	{

		/// <summary>
		/// The maximum allowed width of the smoke trail.
		/// </summary>
		/// <value>
		/// The maximum allowed width of the smoke trail.
		/// </value>
		/// <remarks>
		/// The actual width may be less depending on tracer's actual length.
		/// </remarks>
		public float MaxWidth
		{
			get => maxWidth;
			set => maxWidth = value;
		}
		/// <summary>
		/// The smoke trail's lifetime (in seconds).
		/// </summary>
		/// <value>
		/// The smoke trail's lifetime.
		/// </value>
		public float Duration
		{
			get => duration;
			set => duration = value;
		}
		/// <summary>
		/// Describes how the graphic should disappear.
		/// </summary>
		/// <value>
		/// How the graphic should disappear.
		/// </value>
		/// <remarks>
		/// Use bit masking to combine multiple fade modes.
		/// </remarks>
		/// <seealso cref="SmokeFadeMode"/>
		public SmokeFadeMode FadeOutMode
		{
			get => fadeOut;
			set => fadeOut = value;
		}
		/// <summary>
		/// Choose whether the texture coordinates of the trail are tiled or stretched.
		/// </summary>
		/// <value>
		/// How texture coordinates of the trail texture are modified.
		/// </value>
		/// <seealso cref="SmokeTextureMode"/>
		public SmokeTextureMode TextureMode
		{
			get => textureMode;
			set => textureMode = value;
		}
		/// <inheritdoc />
		public override bool IsComplete => lifetime >= duration;

		[Header("Smoke Trail Settings")]
		[SerializeField]
		[Tooltip("The maximum allowed width of the smoke trail")]
		private float maxWidth = 0.15F;
		[SerializeField]
		[Tooltip("The smoke trail's lifetime (in seconds).")]
		private float duration = 0.8F;
		[SerializeField]
		[Tooltip("Describes how the graphic should disappear.")]
		private SmokeFadeMode fadeOut = SmokeFadeMode.Shrink | SmokeFadeMode.Dissolve;
		[Tooltip("Choose whether the texture coordinates of the trail texture are tiled or stretched.")]
		[SerializeField]
		private SmokeTextureMode textureMode = SmokeTextureMode.Tile;

		private float lifetime;
		private static readonly int _AlphaCutoff_ID = Shader.PropertyToID("_AlphaCutoff");
		private static readonly int _SmokeParams_ID = Shader.PropertyToID("_SmokeParams");
		private static readonly float FadeOutGrowFactor = 6.0F;

		private Quaternion roll = Quaternion.identity;

		/// <inheritdoc />
		protected override void Awake()
		{
			base.Awake();

			MaterialPropertyBlock.SetFloat(_AlphaCutoff_ID, Renderer.sharedMaterial.GetFloat(_AlphaCutoff_ID));
			MaterialPropertyBlock.SetVector(_SmokeParams_ID, Renderer.sharedMaterial.GetVector(_SmokeParams_ID));
		}

		protected virtual void OnEnable()
		{
			// Randomize roll rotation
			roll = Quaternion.Euler(0.0F, 0.0F, UnityEngine.Random.Range(0.0F, 360.0F));
		}

		/// <inheritdoc />
		protected override void Step(float dt)
		{
			base.Step(dt);

			lifetime += dt;
		}

		/// <inheritdoc />
		protected override void Rebuild()
		{
			var lifetimeAlpha = Mathf.InverseLerp(0.0F, Duration, lifetime);
			var length = GetLength();
			var width = Mathf.Min(length, MaxWidth);

			// Apply fade out animation
			if (FadeOutMode.HasFlag(SmokeFadeMode.Shrink))
			{
				width *= 1.0F - lifetimeAlpha;
			}
			else if (FadeOutMode.HasFlag(SmokeFadeMode.Grow))
			{
				width *= Mathf.Lerp(1.0F, FadeOutGrowFactor, lifetimeAlpha);
			}
			if (FadeOutMode.HasFlag(SmokeFadeMode.FadeOut))
			{
				var color = Renderer.sharedMaterial.GetColor(_Color_ID);
				color.a = 1.0F - lifetimeAlpha;
				MaterialPropertyBlock.SetColor(_Color_ID, color);
			}
			if (FadeOutMode.HasFlag(SmokeFadeMode.Dissolve))
			{
				MaterialPropertyBlock.SetFloat(_AlphaCutoff_ID, 1.0F - lifetimeAlpha);
			}
			// Apply UV rescaling
			if (TextureMode == SmokeTextureMode.Tile)
			{
				var vector = Renderer.sharedMaterial.GetVector(_SmokeParams_ID);
				vector.y = length;
				MaterialPropertyBlock.SetVector(_SmokeParams_ID, vector);
			}

			// Upload to transform
			Transform.position = Tracker.Position;
			Transform.rotation = Tracker.Velocity.sqrMagnitude == 0.0F ? Quaternion.identity : Quaternion.LookRotation(Tracker.Velocity) * roll;
			Transform.localScale = new Vector3(width, width, length);

			// Upload to renderer
			if (FadeOutMode.HasFlag(SmokeFadeMode.FadeOut) || FadeOutMode.HasFlag(SmokeFadeMode.Dissolve) || TextureMode == SmokeTextureMode.Tile)
			{
				Renderer.SetPropertyBlock(MaterialPropertyBlock);
			}

			float GetLength() => Clamp(Tracker.Distance);
		}

		/// <inheritdoc />
		public override void DrawLine(Vector3 from, Vector3 to, float speed, float strobeOffset = 0.0F)
		{
			base.DrawLine(from, to, speed, strobeOffset);

			lifetime = 0.0F;
		}

		/// <inheritdoc />
		public override void DrawRay(Vector3 origin, Vector3 direction, float speed, float timeoutDistance = float.PositiveInfinity, float strobeOffset = 0.0F) => DrawLine(origin, origin + direction * timeoutDistance, speed, strobeOffset: strobeOffset);
	}
}
