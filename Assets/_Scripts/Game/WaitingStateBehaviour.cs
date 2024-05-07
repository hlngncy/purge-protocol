using System.Collections;
using System.Collections.Generic;
using System.Net;
using Fusion;
using UnityEngine;

public class WaitingStateBehaviour : BaseStateBehaviour
{
    protected override void SetCanvas(bool isActive)
    {
        if (_authority.HasInputAuthority)
        {
            InitializeCanvas(isActive);
            _playerUIController.InitializeWaitingCanvas(_runner.SessionInfo);
            
        }
    }

    public override void EnterState()
    {
        _nextState = GameStates.Fight;
        StateName = GameStates.Await;
        base.EnterState();
    }
    
    public override void ExitState()
    {
        base.ExitState();
        InitializeCanvas(false);
    }
    
}
