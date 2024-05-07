using System;
using Fusion;
using UnityEngine;

public class PlayerInputHandler : NetworkBehaviour,IBeforeUpdate
{
    [Networked] public NetworkButtons NetworkButtonsPrev { get; set; }
    [Networked] private Vector2 MovementInput { get; set; }
    [Networked] private float NetworkVerticalRotation { get; set; }
    
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private EntityAnimationController _animationController;
    [SerializeField] private Transform aimPoint;
    private IAgent _agent;
    private Vector3 _direction;
    private NetworkCharacterController _characterController;
    private Transform _t;
    private float _horizontalMovement;
    private float _verticalMovement;
    private float _horizontalRotation;
    private float _verticalRotation;
    private bool _jumpPressed;
    private bool _firePressed;
    private bool _aimPressed;
    private bool _reloadPressed;
    private bool _shooting;
    private bool _horizontalButtonPress;
    private InputData _data;

    public override void Spawned()
    {
        base.Spawned();
        _characterController = GetComponent<NetworkCharacterController>();
        _t = transform;
        _agent = GetComponentInChildren<IAgent>();
    }

    public void BeforeUpdate()
    {
        if(!Object.HasInputAuthority) return;
        _horizontalMovement = Input.GetAxis("Horizontal");
        _horizontalButtonPress = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        _verticalMovement = Input.GetAxis("Vertical");
        _horizontalRotation = _cameraController.TargetYawValue;
        _verticalRotation = _cameraController.TargetPitchValue;
        _jumpPressed = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button14);
        _firePressed = Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Joystick1Button9);
        _shooting = Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Joystick1Button9);
        _aimPressed = Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Joystick1Button8);
        _reloadPressed = Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.Joystick1Button13);
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<InputData>(Object.InputAuthority,out var input))
        {
            NetworkVerticalRotation = input.verticalRotation;
            _characterController.Rotate(input.horizontalRotation);
            _direction = _t.forward * input.verticalMovement + _t.right * input.horizontalMovement;
            _direction.Normalize();
            _characterController.Move(_direction, input.isFiring, input.verticalMovement);
            if (input.networkButtons.WasPressed(NetworkButtonsPrev, PlayerButtons.Jump))
            {
                _characterController.Jump();
            }

            MovementInput = new Vector2(input.horizontalMovement, input.verticalMovement);
            
            NetworkButtonsPrev = input.networkButtons;
        }
    }

    private void Update()
    {
        aimPoint.localRotation = Quaternion.Euler(NetworkVerticalRotation,0, 0.0f);
    }

    public override void Render()
    {
        base.Render();
        _animationController.Move(MovementInput.x,MovementInput.y,MathF.Abs(_characterController.Velocity.magnitude), _characterController.IsGrounded, _horizontalButtonPress);
        _animationController.SetInputs(_data,NetworkButtonsPrev);
    }

    public InputData GetInputData()
    {
        _data = new InputData();
        _data.horizontalMovement = _horizontalMovement;
        _data.horizontalMovementButton = _horizontalButtonPress;
        _data.verticalMovement = _verticalMovement;
        _data.horizontalRotation = _horizontalRotation;
        _data.verticalRotation = _verticalRotation;
        _data.isFiring = _shooting;
        _data.networkButtons.Set(PlayerButtons.Jump, _jumpPressed);
        _data.networkButtons.Set(PlayerButtons.Fire, _firePressed);
        _data.networkButtons.Set(PlayerButtons.Aim, _aimPressed);
        _data.networkButtons.Set(PlayerButtons.Reload, _reloadPressed);
        return _data;
    }
}

public enum PlayerButtons
{
    None,
    Jump,
    Fire,
    Aim,
    Reload
}
