using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour,INetworkRunnerCallbacks
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private GameStateManager _gameStateManager;
    private LobbySessionListHandler _lobbySessionListHandler;
    public List<NetworkPlayer> Agents { get; private set; }
    private List<NetworkPlayer> _agents = new();

    private void Awake()
    {
        _lobbySessionListHandler = FindObjectOfType<LobbySessionListHandler>(true);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("player join");
        if (runner.IsServer)
        {
            Transform sp = AgentSpawnPointManager.Instance.GetPoint();
            NetworkObject agent = runner.Spawn(_playerPrefab, sp.position, sp.rotation, player);
            _agents.Add(agent.GetComponent<NetworkPlayer>());
            Agents = _agents;
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
       
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
       
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
       
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
       
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("sessionListUpdated");
        if (_lobbySessionListHandler == null)
        {
            Debug.Log("set session status null");
            return;
        }
        Debug.Log("set session status");
        Debug.Log(sessionList.Count != 0);
        _lobbySessionListHandler.SetSessionStatus(sessionList.Count != 0, sessionList);
        MainMenuManager.Instance.ListedLobbies(sessionList);
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
       
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
       
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
       
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
       
    }
}
