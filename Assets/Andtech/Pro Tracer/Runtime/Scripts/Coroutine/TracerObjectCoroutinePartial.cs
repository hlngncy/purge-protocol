#if !PROTRACER_UNITASK_SUPPORT
using System;
using System.Collections;
using UnityEngine;

namespace Andtech.ProTracer
{

	public partial class TracerObject
	{

		/// <summary>
		/// Asychronously draws the tracer between two points.
		/// </summary>
		/// <param name="from">The starting position.</param>
		/// <param name="to">The end position.</param>
		/// <param name="speed">How fast should the tracer move?</param>
		/// <param name="strobeOffset">How much of the tracer should be shown initially? (This eliminates the Wagon-Wheel effect)</param>
		/// <remarks>Control is returned to the caller once the tracer has completed the entire animation.</remarks>
		/// <returns>N/A</returns>
		/// <seealso cref="AnimateLineAsync(Vector3, Vector3, float, float)"/>
		/// <seealso cref="ShrinkAsync()"/>
		public IEnumerator DrawLineAsync(Vector3 from, Vector3 to, float speed, float strobeOffset = 0.0F)
		{
			yield return AnimateLineAsync(from, to, speed, strobeOffset: strobeOffset);
			yield return ShrinkAsync();
		}

		/// <summary>
		/// Asynchronously draws the tracer with projectile motion.
		/// </summary>
		/// <param name="origin">The starting position.</param>
		/// <param name="direction">The initial direction of motion.</param>
		/// <param name="speed">How fast should the tracer move?</param>
		/// <param name="timeoutDistance">The maximum distance of the flight path.</param>
		/// <param name="strobeOffset">How much of the tracer should be shown initially? (This eliminates the Wagon-Wheel effect)</param>
		/// <remarks>Control is returned to the caller once the tracer has completed the entire animation.</remarks>
		/// <returns>N/A</returns>
		/// <seealso cref="AnimateRayAsync(Vector3, Vector3, float, float, float)"/>
		/// <seealso cref="ShrinkAsync()"/>
		public IEnumerator DrawRayAsync(Vector3 origin, Vector3 direction, float speed, float timeoutDistance = Mathf.Infinity, float strobeOffset = 0.0F)
		{
			yield return AnimateRayAsync(origin, direction, speed, timeoutDistance: timeoutDistance, strobeOffset: strobeOffset);
			yield return ShrinkAsync();
		}

		/// <summary>
		/// Asynchronously draws the tracer by following (tracking) a point.
		/// </summary>
		/// <param name="readPosition">How should we obtain the position?</param>
		/// <param name="timeoutDistance">How far can the tracer travel before timing out?</param>
		/// <remarks>Control is returned to the caller once the tracer has completed the entire animation.</remarks>
		/// <returns>N/A</returns>
		/// <seealso cref="AnimateTrackAsync(Func{Vector3}, float)"/>
		/// <seealso cref="ShrinkAsync()"/>
		public IEnumerator TrackAsync(Func<Vector3> readPosition, float timeoutDistance = Mathf.Infinity)
		{
			yield return AnimateTrackAsync(readPosition, timeoutDistance);
			yield return ShrinkAsync();
		}

		/// <summary>
		/// Asychronously draws the tracer between two points.
		/// </summary>
		/// <param name="from">The starting position.</param>
		/// <param name="to">The end position.</param>
		/// <param name="speed">How fast should the tracer move?</param>
		/// <param name="strobeOffset">How much of the tracer should be shown initially? (This eliminates the Wagon-Wheel effect)</param>
		/// <remarks>Control is returned to the caller once the tracer has arrived at the destination.</remarks>
		/// <returns>N/A</returns>
		public IEnumerator AnimateLineAsync(Vector3 from, Vector3 to, float speed, float strobeOffset = 0.0F)
		{
			DrawLine(from, to, speed, strobeOffset: strobeOffset);

			yield return waitWhileAirborne;
		}

		/// <summary>
		/// Asynchronously draws the tracer with projectile motion.
		/// </summary>
		/// <param name="origin">The starting position.</param>
		/// <param name="direction">The initial direction of motion.</param>
		/// <param name="speed">How fast should the tracer move?</param>
		/// <param name="timeoutDistance">The maximum distance of the flight path.</param>
		/// <param name="strobeOffset">How much of the tracer should be shown initially? (This eliminates the Wagon-Wheel effect)</param>
		/// <remarks>Control is returned to the caller once the tracer has arrived at the destination.</remarks>
		/// <returns>N/A</returns>
		public IEnumerator AnimateRayAsync(Vector3 origin, Vector3 direction, float speed, float timeoutDistance = Mathf.Infinity, float strobeOffset = 0.0F)
		{
			DrawRay(origin, direction, speed: speed, timeoutDistance: timeoutDistance, strobeOffset: strobeOffset);

			yield return waitWhileAirborne;
		}

		/// <summary>
		/// Asynchronously draws the tracer by following (tracking) a point.
		/// </summary>
		/// <param name="readPosition">How should we obtain the position?</param>
		/// <param name="timeoutDistance">How far can the tracer travel before timing out?</param>
		/// <remarks>Control is returned to the caller once the tracer has arrived at the destination.</remarks>
		/// <returns>N/A</returns>
		public IEnumerator AnimateTrackAsync(Func<Vector3> readPosition, float timeoutDistance = Mathf.Infinity)
		{
			Track(readPosition, timeoutDistance: timeoutDistance);

			yield return waitWhileAirborne;
		}

		/// <summary>
		/// Forces the tracer to begin shrinking.
		/// </summary>
		/// <remarks>Control is returned to the caller once the tracer completely shrinks.</remarks>
		/// <returns>N/A</returns>
		public IEnumerator ShrinkAsync()
		{
			Shrink();

			yield return waitUntilComplete;
		}
	}
}
#endif
