using Fusion;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private GameObject _cinemachineCameraTarget;
    [SerializeField] private Transform _orientation;
    [SerializeField] private float _sensX;
    [SerializeField] private float _sensY;
    [SerializeField] private float _topClamp = 70.0f;
    [SerializeField] private float _bottomClamp = -30.0f;
    [SerializeField] private float _cameraAngleOverride = 0.0f;
    private NetworkRunner _runner;
    private NetworkObject _agent;

    public float TargetYawValue => _cinemachineTargetYaw;
    public float TargetPitchValue => _cinemachineTargetPitch;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    
    private float _mouseX;
    private float _mouseY;
    private float _rotationX;
    private float _rotationY;
    
    private Transform _holder;
   
    void OnEnable()
    {
        _runner = GameObject.FindWithTag("NetworkRunner").GetComponent<NetworkRunner>();
        _agent = GetComponent<NetworkObject>();
        _cinemachineTargetYaw = transform.eulerAngles.y;
        _cinemachineTargetPitch = _orientation.eulerAngles.x;
    }

    void Update()
    {
        if (_agent.HasInputAuthority)
        {
            _mouseX = Input.GetAxis("Mouse X") * _runner.DeltaTime * _sensX;
            _mouseY = Input.GetAxis("Mouse Y") * _runner.DeltaTime * _sensY;

            _cinemachineTargetYaw += _mouseX;
            _cinemachineTargetPitch -= _mouseY;
        
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
           
            _orientation.localRotation = Quaternion.Euler(_cinemachineTargetPitch,0, 0.0f);
        }
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

}