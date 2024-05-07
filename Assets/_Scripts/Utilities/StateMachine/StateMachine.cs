using System;
using System.Collections.Generic;
using Fusion;

public abstract class StateMachine<T>: NetworkBehaviour where T : Enum
{
    protected Dictionary<T, BaseState<T>> _states = new Dictionary<T, BaseState<T>>();
    protected BaseState<T> _currentState;

    protected bool _isTransitoningState = false;

    private void Start()
    {
        _currentState.EnterState();
    }

    private void Update()
    {
        if (_isTransitoningState) return;
        
        T nextStateKey = _currentState.GetNextState();

        if (nextStateKey.Equals(_currentState.StateKey))
            _currentState.UpdateState();
        else
            TransitionToState(nextStateKey);
    }

    private void TransitionToState(T nextStateKey)
    {
        _isTransitoningState = true;
        _currentState.ExitState();
        _currentState = _states[nextStateKey];
        _currentState.EnterState();
        _isTransitoningState = false;
    }
}
