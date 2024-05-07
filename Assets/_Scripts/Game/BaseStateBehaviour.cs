using System;
using Fusion;
using UnityEngine;

public abstract class BaseStateBehaviour : NetworkBehaviour
{
    public GameStates StateName { get; protected set; }
    protected GameStates _nextState;
    protected NetworkRunner _runner;
    protected NetworkPlayer _authority;
    protected PlayerUIController _playerUIController;
    
    public void InitializeState()
    {
        _runner = GameObject.FindGameObjectWithTag("NetworkRunner").GetComponent<NetworkRunner>();
        _authority = GameObject.FindGameObjectWithTag("InputAuthority").GetComponent<NetworkPlayer>();
        _playerUIController = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUIController>();
    }
    
    protected abstract void SetCanvas(bool isActive);
    protected void InitializeCanvas(bool active)
    {
        _playerUIController.ChangeCanvas(active, StateName);
    }

    public virtual void ExitState()
    {
        SetCanvas(false);
    }

    public virtual void EnterState()
    {
        SetCanvas(true);
    }
    public GameStates GetNextState()
    {
        return _nextState;
    }
}
