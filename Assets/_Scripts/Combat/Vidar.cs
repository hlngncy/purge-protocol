using System;
using System.Collections;
using System.Collections.Generic;
using Andtech.ProTracer;
using Fusion;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Vidar : NetworkBehaviour, IAgent
{
    [Networked] public NetworkButtons NetworkButtonsPrev { get; set; }
    [Networked(OnChanged = nameof(OnBulletInstance))] public float shoot { get; set; }
    [Networked(OnChanged = nameof(OnAgentFired))] public bool isFiring { get; set; }
    [SerializeField] private Transform barrelRef;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private EntityAnimationController _animationController;
    [SerializeField] private AgentStatsSO _agentStats;
    [SerializeField] private BulletPool _pool;
    [SerializeField] private Rig _aimRig; 
    private float firerate = .1f;
    private float lastTimeFired;

    public void OnEnable()
    {
        _animationController._muzzleVFX = muzzleFlash;
        firerate = _agentStats.FireRate / 15f;
    }

    private void Update()
    {
        _aimRig.weight = isFiring?
            Mathf.Lerp(_aimRig.weight, 1, Runner.DeltaTime * 10):
            Mathf.Lerp(_aimRig.weight, 0, Runner.DeltaTime*10);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Runner.TryGetInputForPlayer<InputData>(Object.InputAuthority, out var input))
        {
            OnFire(input.isFiring);
            NetworkButtonsPrev = input.networkButtons;
        }
    }

    public void OnFire(bool shooting)
    {
        var options = HitOptions.IgnoreInputAuthority | HitOptions.IncludePhysX;
        isFiring = shooting;
        if(Object.HasInputAuthority)
            CameraManager.Instance.SetCameraStateToAim(isFiring);
        if (isFiring && Time.time-lastTimeFired >= firerate)
        {
            lastTimeFired = Time.time;
            Runner.LagCompensation.Raycast(barrelRef.position, barrelRef.forward, 1000, player: Object.InputAuthority, out var hit, layerMask, options);
            shoot = lastTimeFired;
            _pool.GetProjectile(barrelRef, Vector3.zero);
        }
    }
    public static void OnAgentFired(Changed<Vidar> changed)
    {
        bool newVal = changed.Behaviour.isFiring;
        changed.LoadOld();
        bool oldWal = changed.Behaviour.isFiring;
        changed.LoadNew();

        if (oldWal != newVal)
        {
            changed.Behaviour._animationController.OnShoot(newVal);
        }
            
    }

    public static void OnBulletInstance(Changed<Vidar> changed)
    {
        Debug.Log("VALERİA CHANGED");
        changed.Behaviour._pool.GetProjectile(changed.Behaviour.barrelRef, Vector3.zero);
    }
    
    public void OnReload()
    {
        throw new System.NotImplementedException();
    }
    
    public void OnDead()
    {
        throw new System.NotImplementedException();
    }

    
}
