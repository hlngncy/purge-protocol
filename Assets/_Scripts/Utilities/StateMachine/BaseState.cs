using System;

public abstract class BaseState<T> where T : Enum
{
    public BaseState(T key)
    {
        StateKey = key;
    }
    public T StateKey { get; private set; }
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract T GetNextState();
    
}
