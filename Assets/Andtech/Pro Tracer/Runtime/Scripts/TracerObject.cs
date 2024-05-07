#pragma warning disable 0108

using System;
using UnityEngine;

namespace Andtech.ProTracer
{

	/// <summary>
	/// Tracer graphic base class.
	/// </summary>
	public abstract partial class TracerObject : MonoBehaviour
	{
		/// <summary>
		/// The renderer used to represent the graphic.
		/// </summary>
		/// <value>
		/// The tracer's main renderer.
		/// </value>
		public Renderer Renderer
		{
			get => renderer;
			set => renderer = value;
		}
		/// <summary>
		/// The color of the graphic.
		/// </summary>
		/// <value>
		/// The tracer's main color.
		/// </value>
		public virtual Color Color
		{
			get => propertyBlock.GetColor(_Color_ID);
			set
			{
				propertyBlock.SetColor(_Color_ID, value);
				renderer.SetPropertyBlock(propertyBlock);
			}
		}
		/// <summary>
		/// Is the tracer done with its entire animation? (Readonly)
		/// </summary>
		/// <value>
		/// Whether the graphic is done with its animation.
		/// </value>
		public abstract bool IsComplete { get; }
		/// <summary>
		/// The internal tracking logic.
		/// </summary>
		/// <value>
		/// The internal tracking logic.
		/// </value>
		internal Tracker Tracker { get; set; }

		/// <summary>
		/// The root transform of the graphic.
		/// </summary>
		/// <value>
		/// The root transform of the graphic.</value>
		internal protected Transform Transform { get; set; }
		/// <summary>
		/// The material property block of the graphic.
		/// </summary>
		/// <value>
		/// A reference to the material property block.
		/// </value>
		internal protected MaterialPropertyBlock MaterialPropertyBlock => propertyBlock;
		/// <summary>
		/// The cached ID hash for the color property.
		/// </summary>
		/// <value>
		/// The ID hash of the color property.
		/// </value>
		internal protected static readonly int _Color_ID = Shader.PropertyToID("_Color");
		/// <summary>
		/// Cached function which determines airborne state.
		/// </summary>
		/// <value>
		/// A cached lambda function.
		/// </value>
		internal protected Func<bool> cachedIsAirborne;
		/// <summary>
		/// Cached function which determines completion state.
		/// </summary>
		/// <value>
		/// A cached lambda function.
		/// </value>
		internal protected Func<bool> cachedIsComplete;
		/// <summary>
		/// Cached awaiter which determines airborne state.
		/// </summary>
		/// <value>
		/// A cached yield instruction.
		/// </value>
		internal protected WaitWhile waitWhileAirborne;
		/// <summary>
		/// Cached awaiter which determines completion state.
		/// </summary>
		/// <value>
		/// A cached yield instruction.
		/// </value>
		internal protected WaitUntil waitUntilComplete;

		[Header("Tracer Settings")]
		[Tooltip("The renderer used to represent the graphic.")]
		[SerializeField]
		private Renderer renderer;
		private MaterialPropertyBlock propertyBlock;
		private readonly StateMachine stateMachine = new StateMachine();

		/// <summary>
		/// Resets the graphic to an initial state.
		/// </summary>
		protected virtual void Reset()
		{
			renderer = GetComponentInChildren<Renderer>();
		}

		/// <summary>
		/// Initializes the graphic.
		/// </summary>
		protected virtual void Awake()
		{
			Transform = transform;
			propertyBlock = new MaterialPropertyBlock();
			propertyBlock.SetColor(_Color_ID, Renderer.sharedMaterial.GetColor(_Color_ID));

			stateMachine.Arrived += StateMachine_Arrived;
			stateMachine.Completed += StateMachine_Completed;
			stateMachine.Reset();

			cachedIsAirborne = _IsAirborne;
			cachedIsComplete = _IsComplete;
			waitWhileAirborne = new WaitWhile(cachedIsAirborne);
			waitUntilComplete = new WaitUntil(cachedIsComplete);

			void StateMachine_Arrived(object sender, EventArgs e) => OnArrive(EventArgs.Empty);

			void StateMachine_Completed(object sender, EventArgs e) => OnComplete(EventArgs.Empty);
		}

		bool _IsAirborne() => stateMachine.PresentState == StateMachine.State.Airborne;
		bool _IsComplete() => IsComplete;

		/// <summary>
		/// Renders the graphic.
		/// </summary>
		/// <remarks>
		/// Override <see cref="Step"/> or <see cref="Rebuild"/> instead of this method.
		/// </remarks>
		protected virtual void LateUpdate()
		{
			if (Tracker is null)
			{
				Debug.LogWarning($"There is no tracking logic for component {nameof(TracerObject)} on {gameObject.name}.", gameObject);
				return;
			}

			Step(Time.deltaTime);
			if (Tracker.IsDone)
			{
				Shrink();
			}
			if (IsComplete)
			{
				stateMachine.Complete();
			}
			else
			{
				Rebuild();
			}
		}

		/// <summary>
		/// Internal tracer update.
		/// </summary>
		/// <param name="dt">How much time passed since the last step?</param>
		/// <remarks>
		/// Put internal update code here (e.g. physics, timers, etc.)
		/// </remarks>
		protected virtual void Step(float dt) => Tracker.Step(dt);

		/// <summary>
		/// Rebuilds the graphic.
		/// </summary>
		/// <remarks>
		/// Put rendering update code here (e.g. transform changes, material changes)
		/// </remarks>
		protected virtual void Rebuild() { }

		/// <summary>
		/// Draws the tracer between two points.
		/// </summary>
		/// <param name="from">The starting position.</param>
		/// <param name="to">The end position.</param>
		/// <param name="speed">How fast should the tracer move?</param>
		/// <param name="strobeOffset">How much of the tracer should be shown initially? (This eliminates the Wagon-Wheel effect)</param>
		public virtual void DrawLine(Vector3 from, Vector3 to, float speed, float strobeOffset = 0.0F) => Preset(Tracker.FromTo(from, to, speed, strobeOffset));

		/// <summary>
		/// Draws the tracer with projectile motion.
		/// </summary>
		/// <param name="origin">The starting position.</param>
		/// <param name="direction">The initial direction of motion.</param>
		/// <param name="speed">How fast should the tracer move?</param>
		/// <param name="timeoutDistance">The maximum distance of the flight path.</param>
		/// <param name="strobeOffset">How much of the tracer should be shown initially? (This eliminates the Wagon-Wheel effect)</param>
		public virtual void DrawRay(Vector3 origin, Vector3 direction, float speed, float timeoutDistance = Mathf.Infinity, float strobeOffset = 0.0F) => Preset(Tracker.Kinematic(origin, direction.normalized * speed, timeoutDistance: timeoutDistance, initialDistance: strobeOffset));

		/// <summary>
		/// Draws the tracer by following (tracking) a point.
		/// </summary>
		/// <param name="readPosition">How should we obtain the position?</param>
		/// <param name="timeoutDistance">How far can the tracer travel before timing out?</param>
		public virtual void Track(Func<Vector3> readPosition, float timeoutDistance = Mathf.Infinity) => Preset(Tracker.Tracked(readPosition, timeoutDistance));

		/// <summary>
		/// Helper method which initializes the tracer object.
		/// </summary>
		/// <param name="tracker">The internal tracking logic.</param>
		internal void Preset(Tracker tracker)
		{
			Tracker = tracker;
			Transform.position = tracker.Position;
			Transform.rotation = Quaternion.identity;
			Transform.localScale = Vector3.zero;

			stateMachine.Reset();
		}

		/// <summary>
		/// Forces the tracer to begin shrinking.
		/// </summary>
		public void Shrink()
		{
			if (stateMachine.PresentState == StateMachine.State.Airborne)
			{
				stateMachine.Arrive();
				Tracker.Stop();
			}
		}

		/// <summary>
		/// Clamps <paramref name="position"/> along the tracer's flight path.
		/// </summary>
		/// <param name="position">The parametric distance along the flight path.</param>
		/// <returns>The clamped distance.</returns>
		internal protected float Clamp(float position) => Mathf.Clamp(position, 0.0F, Tracker.Length);

		/// <summary>
		/// This event is raised when the tracer first arrives at an endpoint.
		/// </summary>
		/// <summary>
		/// The graphic arrival event.
		/// </summary>
		public event EventHandler Arrived;
		/// <summary>
		/// This event is raised when the tracer graphic is completely finished.
		/// </summary>
		/// <value>
		/// The graphic completion event.
		/// </value>
		public event EventHandler Completed;

		/// <summary>
		/// Invokes the <see cref="Arrived"/> event.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <remarks>
		/// Inheritors must use this to raise the event.
		/// </remarks>
		protected virtual void OnArrive(EventArgs e) => Arrived?.Invoke(this, e);

		/// <summary>
		/// Invokes the <see cref="Completed"/> event.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <remarks>
		/// Inheritors must use this to raise the event.
		/// </remarks>
		protected virtual void OnComplete(EventArgs e) => Completed?.Invoke(this, e);

		/// <summary>
		/// State machine helper for tracer objects.
		/// </summary>
		internal class StateMachine
		{

			/// <summary>
			/// Tracer state.
			/// </summary>
			public enum State
			{
				/// <summary>
				/// The tracer is moving along the flight path.
				/// </summary>
				Airborne,
				/// <summary>
				/// The tracer has arrived at the end point (but may still in an animation).
				/// </summary>
				Arrived,
				/// <summary>
				/// The tracer has finished its entire animation.
				/// </summary>
				Complete
			}

			/// <summary>
			/// The present state of the machine.
			/// </summary>
			/// <value>
			/// The present state of the machine.
			/// </value>
			public State PresentState { get; private set; }

			/// <summary>
			/// Constructs a default state machine.
			/// </summary>
			public StateMachine() => PresentState = State.Airborne;

			/// <summary>
			/// Reset the machine.
			/// </summary>
			public void Reset() => PresentState = State.Airborne;

			/// <summary>
			/// Relays the arrival to the state machine.
			/// </summary>
			public void Arrive()
			{
				switch (PresentState)
				{
					case State.Airborne:
						OnArrive(EventArgs.Empty);
						PresentState = State.Arrived;
						break;
				}
			}

			/// <summary>
			/// Relays the completion to the state machine.
			/// </summary>
			public void Complete()
			{
				switch (PresentState)
				{
					case State.Airborne:
						OnArrive(EventArgs.Empty);
						OnComplete(EventArgs.Empty);
						PresentState = State.Complete;
						break;
					case State.Arrived:
						OnComplete(EventArgs.Empty);
						PresentState = State.Complete;
						break;
				}
			}

			/// <summary>
			/// This event is raised when the tracer receives an arrival signal.
			/// </summary>
			/// <value>
			/// The state machine arrival event.
			/// </value>
			public event EventHandler Arrived;
			/// <summary>
			/// This event is raised when the tracer receives a completion signal.
			/// </summary>
			/// <value>
			/// The state machine completion event.
			/// </value>
			public event EventHandler Completed;

			/// <summary>
			/// Invokes the <see cref="Arrived"/> event.
			/// </summary>
			/// <param name="e">The event arguments.</param>
			protected virtual void OnArrive(EventArgs e) => Arrived?.Invoke(this, e);

			/// <summary>
			/// Invokes the <see cref="Completed"/> event.
			/// </summary>
			/// <param name="e">The event arguments.</param>
			protected virtual void OnComplete(EventArgs e) => Completed?.Invoke(this, e);
		}
	}
}
