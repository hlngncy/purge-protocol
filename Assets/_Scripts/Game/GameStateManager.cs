using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager I;
    public GameStates CurrentGameState { get; private set; }
    public bool isInputsEnabled = false;
    
    [Networked(OnChanged = nameof(OnStateChange))]
    private int CurrentStateIndex { get; set; }
    [SerializeReference] private List<BaseStateBehaviour> _states;
    private BaseStateBehaviour _currentState;
    private NetworkRunner _runner;
    private List<NetworkPlayer> _agents;
    private bool _statesInitialized = false;
    
    private void Awake()
    {
        if(I != null)
            Destroy(this);
        else
        {
            I = this;
        }
    }

    public void OnPlayerSpawned()
    {
        foreach (BaseStateBehaviour state in _states)
        {
            state.InitializeState();
        }
    }
    
    public override void Spawned()
    {
        base.Spawned();
        _runner = GameObject.FindGameObjectWithTag("NetworkRunner").GetComponent<NetworkRunner>();
        _agents = _runner.GetComponent<PlayerSpawner>().Agents;
    }
    
    private void Update()
    {
        /*if (_runner.IsServer)
        {
            if (CurrentStateIndex == 1)
            {
                CheckAlivePlayers();
            }
        }*/
    }

    private void CheckAlivePlayers()
    {
        foreach (NetworkPlayer agent in _agents)
        {
            //TODO check if dead;
        }
    }

    private static void OnStateChange(Changed<GameStateManager> changed)
    {
        Debug.Log("On state change");
        changed.Behaviour.SetNextState();
    }
    
    public void ChangeStateForce(GameStates state)
    {
        switch (state)
        {
            case GameStates.Await:
                CurrentStateIndex = 0;
                break;
            case GameStates.Fight:
                CurrentStateIndex = 1;
                break;
            case GameStates.Preparation:
                CurrentStateIndex = 2;
                break;
            case GameStates.End:
                CurrentStateIndex = 3;
                break;
        }
        SetNextState();
    }

    public void OnStartFight()
    {
        CurrentStateIndex = 1;
    }
    
    private void SetNextState()
    {
        GameStates tempState = _currentState == null ? GameStates.Await : _currentState.GetNextState();
        switch (tempState)
        {
            case GameStates.Await:
                _currentState = _states[CurrentStateIndex];
                _currentState.EnterState();
                break;
            case GameStates.Fight:
                ChangeState();
                break;
            case GameStates.Preparation:
                ChangeState();
                break;
            case GameStates.End:
                ChangeState();
                break;
        }
        CurrentGameState = _currentState.StateName;
    }
    private void ChangeState()
    {
        _currentState.ExitState();
        _currentState = _states[CurrentStateIndex];
        _currentState.EnterState();
        Debug.Log("change state");
    }
}

public enum GameStates
{
    Await,
    Fight,
    Preparation,
    End
}