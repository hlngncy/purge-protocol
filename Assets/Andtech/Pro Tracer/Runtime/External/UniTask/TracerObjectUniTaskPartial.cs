#if PROTRACER_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Andtech.ProTracer
{

	public partial class TracerObject
	{

		/// <inheritdoc cref="DrawLineAsync(Vector3, Vector3, float, float)"/>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		/// <seealso cref="AnimateLineAsync(Vector3, Vector3, float, float, CancellationToken)"/>
		/// <seealso cref="ShrinkAsync(CancellationToken)"/>
		public async UniTask DrawLineAsync(Vector3 from, Vector3 to, float speed, float strobeOffset = 0.0F, CancellationToken cancellationToken = default)
		{
			await AnimateLineAsync(from, to, speed, strobeOffset: strobeOffset, cancellationToken: cancellationToken);
			await ShrinkAsync(cancellationToken: cancellationToken);
		}

		/// <inheritdoc cref="DrawRayAsync(Vector3, Vector3, float, float, float)"/>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		/// <seealso cref="AnimateRayAsync(Vector3, Vector3, float, float, float, CancellationToken)"/>
		/// <seealso cref="ShrinkAsync(CancellationToken)"/>
		public async UniTask DrawRayAsync(Vector3 origin, Vector3 direction, float speed, float timeoutDistance = Mathf.Infinity, float strobeOffset = 0.0F, CancellationToken cancellationToken = default)
		{
			await AnimateRayAsync(origin, direction, speed, timeoutDistance: timeoutDistance, strobeOffset: strobeOffset, cancellationToken: cancellationToken);
			await ShrinkAsync(cancellationToken: cancellationToken);
		}

		/// <inheritdoc cref="TrackAsync(Func{Vector3}, float)"/>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		/// <seealso cref="AnimateTrackAsync(Func{Vector3}, float, CancellationToken)"/>
		/// <seealso cref="ShrinkAsync(CancellationToken)"/>
		public async UniTask TrackAsync(Func<Vector3> readPosition, float timeoutDistance = Mathf.Infinity, CancellationToken cancellationToken = default)
		{
			await AnimateTrackAsync(readPosition, timeoutDistance, cancellationToken: cancellationToken);
			await ShrinkAsync(cancellationToken: cancellationToken);
		}

		/// <inheritdoc cref="AnimateLineAsync(Vector3, Vector3, float, float)"/>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		public async UniTask AnimateLineAsync(Vector3 from, Vector3 to, float speed, float strobeOffset = 0.0F, CancellationToken cancellationToken = default)
		{
			DrawLine(from, to, speed, strobeOffset: strobeOffset);

			await UniTask.WaitWhile(cachedIsAirborne, cancellationToken: cancellationToken);
		}

		/// <inheritdoc cref="AnimateRayAsync(Vector3, Vector3, float, float, float)"/>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		public async UniTask AnimateRayAsync(Vector3 origin, Vector3 direction, float speed, float timeoutDistance = Mathf.Infinity, float strobeOffset = 0.0F, CancellationToken cancellationToken = default)
		{
			DrawRay(origin, direction, speed: speed, timeoutDistance: timeoutDistance, strobeOffset: strobeOffset);

			await UniTask.WaitWhile(cachedIsAirborne, cancellationToken: cancellationToken);
		}

		/// <inheritdoc cref="AnimateTrackAsync(Func{Vector3}, float)"/>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		public async UniTask AnimateTrackAsync(Func<Vector3> readPosition, float timeoutDistance = Mathf.Infinity, CancellationToken cancellationToken = default)
		{
			Track(readPosition, timeoutDistance: timeoutDistance);

			await UniTask.WaitWhile(cachedIsAirborne, cancellationToken: cancellationToken);
		}

		/// <inheritdoc cref="ShrinkAsync"/>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		public async UniTask ShrinkAsync(CancellationToken cancellationToken = default)
		{
			Shrink();

			await UniTask.WaitUntil(cachedIsComplete, cancellationToken: cancellationToken);
		}
	}
}
#endif
