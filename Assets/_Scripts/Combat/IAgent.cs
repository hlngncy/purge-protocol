using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgent
{
    public abstract void OnFire(bool isFiring);

    public abstract void OnReload();

    public abstract void OnDead();
}
