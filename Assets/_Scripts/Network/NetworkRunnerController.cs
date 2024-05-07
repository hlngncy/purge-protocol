using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NetworkRunnerController : MonoBehaviour
{

    [SerializeField] private NetworkRunner _networkRunnerPrefab;
    private NetworkRunner _networkRunnerInstance;

    private void Awake()
    {
        NetworkRunner netRunnerInScene = FindObjectOfType<NetworkRunner>();
        if (netRunnerInScene != null) _networkRunnerInstance = netRunnerInScene;
        DontDestroyOnLoad(this);
        OnJoinLobby();
    }

    private void Start()
    {
        if (_networkRunnerInstance == null)
        {
            _networkRunnerInstance = Instantiate(_networkRunnerPrefab);
            if (SceneManager.GetActiveScene().name != "Level0")
            {
                Task clientTask;
                clientTask = InitializeNetworkRunner(_networkRunnerInstance, GameMode.AutoHostOrClient,
                    NetAddress.Any(), "test",0,"testSession", SceneManager.GetActiveScene().buildIndex, null);
            }
        }
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode,
        NetAddress address, string sessionName, SessionProperty difficulty, string sessionTitle, SceneRef scene,
        Action<NetworkRunner> initialized)
    {
        Debug.Log("initialized game");
        var sceneManager = runner.GetComponent<INetworkSceneManager>();

        if (sceneManager == null) runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        runner.ProvideInput = true;
        
        var customProps = new Dictionary<string, SessionProperty>();
        customProps["difficulty"] = difficulty;
        customProps["sessionTitle"] = sessionTitle;
        Debug.Log("diffic"+difficulty.PropertyValue);
        Debug.Log(sessionTitle);
        
        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            CustomLobbyName = "Default",
            SessionName = sessionName,
            Initialized = initialized,
            PlayerCount = 3,
            SessionProperties = customProps,
            SceneManager = sceneManager
        });
    }

    public void OnJoinLobby()
    {
        var clientTask = JoinLobby();
    }
    
    private async Task JoinLobby()
    {
        string lobbyID = "Default";

        var result = await _networkRunnerInstance.JoinSessionLobby(SessionLobby.Custom, lobbyID);
        
        if(!result.Ok) Debug.LogError("unable to join");
        else Debug.Log("able to join");
    }

    public void CreateGame(string sessionName, string sceneName, int difficulty, string sessionTitle)
    {
        var clientTask = InitializeNetworkRunner(_networkRunnerInstance, GameMode.Host,NetAddress.Any() , sessionName, difficulty, sessionTitle,
            SceneUtility.GetBuildIndexByScenePath($"scenes/{sceneName}"), null);
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        var clientTask = InitializeNetworkRunner(_networkRunnerInstance, GameMode.Client,NetAddress.Any() , sessionInfo.Name, sessionInfo.Properties["difficulty"],sessionInfo.Properties["sessionTitle"],
            SceneManager.GetActiveScene().buildIndex, null);
    }
}
