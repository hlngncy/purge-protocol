using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FightStateBehaviour : BaseStateBehaviour
{
    [SerializeField] private GameObject _waitVCam;
    [SerializeField] private CinemachineVirtualCamera _playerVCam;
    [SerializeField] public CinemachineVirtualCamera _playerVAimCam;
    public override void EnterState()
    {
        StateName = GameStates.Fight;
        _nextState = GameStates.Preparation;
        _playerUIController.DeactivatePlayerInfo();
        base.EnterState();
        SetCamera();
        SpawnMobs();
        Debug.Log("fight enter state");
    }
    
    protected override void SetCanvas(bool isActive)
    {
        SetCamera();
        InitializeCanvas(true);
        Debug.Log("setcanvas fight");
        //TODO setup orb pool, health bars, wait until camera is done
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void SpawnMobs()
    {
        //TODO spawn mobs from pool
    }

    private void SetCamera()
    {
        if (_authority.HasInputAuthority)
        {
            Cursor.visible = false;
            
            CameraManager.Instance.SetCameraStateToFollow(_authority.transform);
            
            Invoke(nameof(SetInputSytem), 4);
        }
    }
    
    private void SetInputSytem()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameStateManager.I.isInputsEnabled = true;
        _authority.GetComponent<CameraController>().enabled = true;
    }
}
