using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AgentSpawnPointManager : Singleton<AgentSpawnPointManager>
{
    [SerializeField] private List<Transform> _spawnPoints;
    private int _emptyPointIndex = -1;


    public Transform GetPoint()
    {
        _emptyPointIndex++;
        return _spawnPoints[_emptyPointIndex];
    }
}
