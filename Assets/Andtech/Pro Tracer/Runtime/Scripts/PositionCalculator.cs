using System;
using UnityEngine;

namespace Andtech.ProTracer
{

	internal abstract class BasePositionCalculator
	{

		public abstract Vector3 ReadPosition(Vector3 previousPosition);
	}

	internal class KinematicPositionCalculator : BasePositionCalculator
	{
		private Vector3 velocity;
		private readonly bool useGravity;
		private readonly int InitialFrameCount = Time.frameCount;

		public KinematicPositionCalculator(Vector3 initialVelocity, bool useGravity)
		{
			velocity = initialVelocity;
			this.useGravity = useGravity;
		}

		public override Vector3 ReadPosition(Vector3 previousPosition)
		{
			if (InitialFrameCount == Time.frameCount)
			{
				return previousPosition;
			}

			if (useGravity)
			{
				velocity += Physics.gravity * Time.deltaTime;
			}

			return previousPosition + velocity * Time.deltaTime;
		}
	}

	internal class TrackedPositionCalculator : BasePositionCalculator
	{
		private readonly Func<Vector3> readPosition;

		public TrackedPositionCalculator(Func<Vector3> readPosition)
		{
			this.readPosition = readPosition;
		}

		public override Vector3 ReadPosition(Vector3 previousPosition) => readPosition();
	}
}
