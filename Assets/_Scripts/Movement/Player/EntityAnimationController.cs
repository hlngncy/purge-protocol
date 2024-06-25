using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class EntityAnimationController : MonoBehaviour
{
    #region Animation Hashes

    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int OnGround = Animator.StringToHash("OnGround");
    private static readonly int Jump = Animator.StringToHash("Roll");
    private static readonly int Aiming = Animator.StringToHash("Aiming");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int Reloading = Animator.StringToHash("Reloading");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Use = Animator.StringToHash("Use");
    private static readonly int StopUse = Animator.StringToHash("StopUse");
    private static readonly int PickUp = Animator.StringToHash("PickUp");
    private static readonly int Shooting = Animator.StringToHash("IsFiring");

    #endregion

    [DoNotSerialize] public ParticleSystem _muzzleVFX;
    [SerializeField] private ProjectileVisual _projectile;
    private float _tempXPos;
    public Animator Animator
    {
        set => _animator = value;
    }
    private Animator _animator;

    private InputData _input;
    private NetworkButtons _buttons;


    public void SetInputs(InputData inputData, NetworkButtons previousNetworkButtons)
    {
        _input = inputData;
        _buttons = previousNetworkButtons;
        SetActions();
    }
    public void Move(float posX, float posY, float speed, bool isGrounded,  bool isHorizontalButtonPressed = true, bool isJumping = false)
    {
        if (isHorizontalButtonPressed)
        {
            posX =  posX < 0 ? posX - 1 : posX + 1;
        }

        _animator.SetFloat(Speed, speed);
        _animator.SetFloat(X, posX);
        _animator.SetFloat(Y, posY);
        _animator.SetBool(OnGround, isGrounded);
        if(isJumping && !_animator.GetBool(Aiming) && speed != 0) _animator.SetTrigger(Jump);
    }

    private void SetActions()
    {
        
    }
    
    public void OnShoot(bool isFiring)
    {
        _animator.SetFloat(X, 0);
        _animator.SetBool(Shooting,isFiring);
        _animator.SetBool(Aiming,isFiring);
        if (isFiring)
        {
            _muzzleVFX.Play();
        }
        else
        {
            _muzzleVFX.Stop();
        }
        

    }
}
