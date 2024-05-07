using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera _waitStateCamera;
    [SerializeField] private CinemachineVirtualCamera _playerMainCamera;
    [SerializeField] private CinemachineVirtualCamera _playerAimCamera;

    public void SetCameraStateToFollow(Transform player)
    {
        _playerMainCamera.Follow = player.GetChild(0).GetChild(0);
        _playerMainCamera.LookAt = player;
        _playerAimCamera.Follow = player.GetChild(0).GetChild(0);
        _playerAimCamera.LookAt = player.transform;
        
        _waitStateCamera.gameObject.SetActive(false);
    }

    public void SetCameraStateToAim(bool isFiring)
    {
        _playerAimCamera.gameObject.SetActive(isFiring);
    }
}
