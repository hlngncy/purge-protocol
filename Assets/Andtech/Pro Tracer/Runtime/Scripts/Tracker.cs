using System;
using UnityEngine;

namespace Andtech.ProTracer
{

	/// <summary>
	/// Tracks an body's movement through 3D space.
	/// </summary>
	internal class Tracker
	{

		/// <summary>
		/// The computed position for the current frame.
		/// </summary>
		/// <value>
		/// The position of the tracker.
		/// </value>
		public Vector3 Position { get; private set; } = Vector3.zero;
		/// <summary>
		/// The computed velocity for the current frame.
		/// </summary>
		/// <value>
		/// The velocity of the tracker.
		/// </value>
		public Vector3 Velocity { get; private set; } = Vector3.zero;
		/// <summary>
		/// The total cumulative distance travelled.
		/// </summary>
		/// <value>
		/// The parametric distance travalled along the flight path.
		/// </value>
		public float Distance { get; private set; }
		/// <summary>
		/// The total parametric distance of the flight path.
		/// </summary>
		/// <value>
		/// The parametric length of the flight path.
		/// </value>
		/// <remarks>
		/// In the case that the flight path is unknown, this value defaults to infinity.
		/// </remarks>
		public float Length { get; private set; }
		/// <summary>
		/// Is the tracker finished?
		/// </summary>
		/// <value>
		/// Whether tracker has reached the final point.
		/// </value>
		public bool IsDone => Distance >= Length;

		internal float FallbackSpeed { get; set; } = Mathf.Infinity;
		private Ray StoppingPath { get; set; }
		private bool IsTracking { get; set; } = true;

		private readonly BasePositionCalculator positionCalculator;

		private Tracker(BasePositionCalculator positionCalculator, float timeoutDistance = Mathf.Infinity, float initialDistance = 0.0F)
		{
			Length = timeoutDistance;
			Distance = initialDistance;
			this.positionCalculator = positionCalculator;
		}

		internal bool GetIsDone() => IsDone;

		/// <summary>
		/// Stops the tracking. This tracker will continue to move with the last known velocity.
		/// </summary>
		public virtual void Stop()
		{
			if (!IsTracking)
			{
				return;
			}

			IsTracking = false;

			var direction = Velocity.normalized;
			Length = Distance;
			FallbackSpeed = Velocity.magnitude;

			StoppingPath = new Ray(Position + -Length * direction, direction);
		}

		/// <summary>
		/// Stops the tracking. This tracker will now move toward a destination.
		/// </summary>
		/// <param name="destination">The final stopping position.</param>
		public virtual void Stop(Vector3 destination)
		{
			if (!IsTracking)
			{
				return;
			}

			IsTracking = false;

			var delta = destination - Position;
			var direction = delta.normalized;

			Length = Distance + delta.magnitude;
			FallbackSpeed = Velocity.magnitude;

			StoppingPath = new Ray(destination + -Length * direction, direction);
		}

		/// <summary>
		/// Steps the tracker forward using the default timestep.
		/// </summary>
		public virtual void Step() => Step(Time.deltaTime);

		/// <summary>
		/// Steps the tracker forward.
		/// </summary>
		/// <param name="dt">How much time passed since the last call to <see cref="Step(float)"/>?</param>
		public virtual void Step(float dt)
		{
			if (IsTracking)
			{
				var lastPosition = Position;
				var nextPosition = positionCalculator.ReadPosition(lastPosition);
				if (lastPosition == nextPosition)
				{
					return;
				}

				var delta = nextPosition - lastPosition;
				var step = delta.magnitude;

				// Iteration
				Distance += step;
				Velocity = delta / dt;
				Position = nextPosition;
			}
			else
			{
				var step = FallbackSpeed * dt;

				// Iteration
				Distance += step;
				Position = StoppingPath.GetPoint(Mathf.Min(Distance, Length));
			}
		}

		/// <summary>
		/// Returns a tracker which moves between two points.
		/// </summary>
		/// <param name="from">The starting position.</param>
		/// <param name="to">The end position.</param>
		/// <param name="speed">How fast should the tracker traverse the flight path.</param>
		/// <param name="initialDistance">How much progress along the flight path should the tracker start with?</param>
		/// <returns>The tracker instance.</returns>
		public static Tracker FromTo(Vector3 from, Vector3 to, float speed, float initialDistance = 0.0F)
		{
			var delta = to - from;
			var length = delta.magnitude;
			var direction = delta / length;
			var velocity = direction * speed;

			var ray = new Ray(from, direction);
			var tracker = new Tracker(null, length, initialDistance)
			{
				Position = ray.GetPoint(initialDistance),
				Velocity = velocity,
				FallbackSpeed = speed
			};
			tracker.StoppingPath = ray;
			tracker.IsTracking = false;

			return tracker;
		}

		/// <summary>
		/// Returns a tracker which moves using projectile motion.
		/// </summary>
		/// <param name="origin">The starting position.</param>
		/// <param name="velocity">The initial direction of motion.</param>
		/// <param name="useGravity">Should the tracker obey gravity?</param>
		/// <param name="timeoutDistance">The maximum distance before the tracker times out.</param>
		/// <param name="initialDistance">How much progress along the flight path should the tracker start with?</param>
		/// <returns>The tracker instance.</returns>
		public static Tracker Kinematic(Vector3 origin, Vector3 velocity, bool useGravity = true, float timeoutDistance = Mathf.Infinity, float initialDistance = 0.0F)
		{
			var positionCalculator = new KinematicPositionCalculator(velocity, useGravity);
			var tracker = new Tracker(positionCalculator, timeoutDistance, initialDistance)
			{
				Position = origin + velocity.normalized * initialDistance,
				Velocity = velocity
			};

			return tracker;
		}

		/// <summary>
		/// Returns a tracker which reads positions externally.
		/// </summary>
		/// <param name="readPosition">The strategy to read a position.</param>
		/// <param name="timeoutDistance">How maximum distance before the tracker times out.</param>
		/// <returns>The tracker instance.</returns>
		public static Tracker Tracked(Func<Vector3> readPosition, float timeoutDistance = Mathf.Infinity)
		{
			var positionCalculator = new TrackedPositionCalculator(readPosition);
			var tracker = new Tracker(positionCalculator, timeoutDistance, 0.0F)
			{
				Position = readPosition()
			};

			return tracker;
		}
	}
}
