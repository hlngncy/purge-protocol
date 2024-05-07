#if PROTRACER_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Andtech.ProTracer
{

	public partial class Bullet
	{

		/// <inheritdoc cref="DrawRayAsync(Vector3, Vector3, float, float, float, bool)"/>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		public virtual async UniTask DrawRayAsync(Vector3 origin, Vector3 direction, float speed, float timeoutDistance = float.PositiveInfinity, float strobeOffset = 0.0F, bool useGravity = true, CancellationToken cancellationToken = default)
		{
			await AnimateRayAsync(origin, direction, speed, timeoutDistance: timeoutDistance, strobeOffset: strobeOffset, useGravity: useGravity, cancellationToken: cancellationToken);
			await ShrinkAsync(cancellationToken: cancellationToken);
		}

		/// <inheritdoc cref="AnimateRayAsync(Vector3, Vector3, float, float, float, bool)"/>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		public async UniTask AnimateRayAsync(Vector3 origin, Vector3 direction, float speed, float timeoutDistance = float.PositiveInfinity, float strobeOffset = 0.0F, bool useGravity = true, CancellationToken cancellationToken = default)
		{
			DrawRay(origin, direction, speed: speed, timeoutDistance: timeoutDistance, strobeOffset: strobeOffset, useGravity: useGravity);

			await UniTask.WaitWhile(cachedIsAirborne, cancellationToken: cancellationToken);
		}
	}
}
#endif
