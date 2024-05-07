using System;
using Andtech.ProTracer;
using UnityEngine;

public class ProjectileVisual : MonoBehaviour
	{
		[Header("Projectile Setup")]
		public float Speed = 80f;
		public float MaxDistance = 100f;
		public float LifeTimeAfterHit = 2f;

		[Header("Impact Setup")]
		public GameObject HitEffectPrefab;
		
		[SerializeField]
		[Tooltip("The Bullet prefab to spawn.")]
		private Bullet bulletPrefab = default;
		[SerializeField]
		[Tooltip("The Smoke Trail prefab to spawn.")]
		private SmokeTrail smokeTrailPrefab = default;
		
		private Vector3 _startPosition;
		private Vector3 _targetPosition;
		private Vector3 _hitNormal;

		private bool _showHitEffect;
		private GameObject _hitEffectPrefab;

		private float _startTime;
		private float _duration;

		private Bullet _bullet;
		public BulletPool _bulletPool;
		private void OnEnable()
		{
			_bullet = Instantiate(bulletPrefab);
			_bullet.Completed += OnCompleted;
			_bullet.gameObject.SetActive(false);
		}

		/// <summary>
		/// Set where the projectile visual should land.
		/// </summary>
		/// <param name="hitPosition">Position where projectile hit geometry</param>
		/// <param name="hitNormal">Normal of the hit surface</param>
		/// <param name="showHitEffect">Whether projectile impact should be displayed
		/// (e.g. we don't want static impact effect displayed on other player's body)</param>
		public void SetHit(Vector3 hitPosition, Vector3 hitNormal, bool showHitEffect, BulletPool pool)
		{
			_targetPosition = hitPosition;
			_showHitEffect = showHitEffect;
			_hitNormal = hitNormal;
			_bulletPool = pool;
			Spawn();
		}

		private void Spawn()
		{
			_startPosition = transform.position;

			if (_targetPosition == Vector3.zero)
			{
				_targetPosition = _startPosition + transform.forward * MaxDistance;
			}

			_duration = Vector3.Distance(_startPosition, _targetPosition) / Speed;
			_startTime = Time.timeSinceLevelLoad;
			
			_bullet.gameObject.SetActive(true);
			_bullet.DrawLine(transform.position, _targetPosition, Speed );
			
		}
	
		private void OnCompleted(object sender, System.EventArgs e)
		{
			// Handle complete event here
			if (sender is TracerObject tracerObject)
			{
				tracerObject.gameObject.SetActive(false);
				tracerObject.transform.position = transform.position;
				tracerObject.transform.rotation = transform.rotation;
			}
			_bulletPool.ReleaseProjectile(this);
		} 
	}
