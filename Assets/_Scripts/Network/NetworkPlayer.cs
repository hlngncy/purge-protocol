using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

//players spesific infos and specialities
public class NetworkPlayer : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnAgentInitialized))] private bool isInitialized { get; set; }
    [Networked] private string Nickname { get; set; }
    [Networked] private int ColorCustomizationIndex { get; set; }
    [Networked] private int SelectedAgentIndex { get; set; }
    
    [SerializeField] private List<GameObject> _agentModels;
    
    private SessionInfo _sessionInfo;

    private PlayerUIController _playerUIController;

    private UnityEvent _networkPlayerInitialized = new();

    private void Awake()
    {
        _networkPlayerInitialized.AddListener(GameStateManager.I.OnPlayerSpawned);
    }

    public override void Spawned()
    {
        _playerUIController = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUIController>();
        
        if (Object.HasInputAuthority)
        {
            gameObject.tag = "InputAuthority";
            _sessionInfo = Runner.SessionInfo;
            if(_playerUIController == null) Debug.LogError("ui controller null 1");
            _networkPlayerInitialized.Invoke();
            Debug.Log("ingame:"+PlayerPrefs.GetInt("AgentColorID"));
            RPC_InitializeAgent(PlayerPrefs.GetString("PlayerName"),PlayerPrefs.GetInt("AgentColorID"),PlayerPrefs.GetInt("AgentID") );
            GameStateManager.I.ChangeStateForce(GameStates.Await);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_InitializeAgent(string nickName, int color, int agent)
    {
        Nickname = nickName;
        ColorCustomizationIndex = color;
        SelectedAgentIndex = agent;
        isInitialized = true;
    }
    private static void OnAgentInitialized(Changed<NetworkPlayer> changed)
    {
        bool newVal = changed.Behaviour.isInitialized;
        changed.LoadOld();
        bool oldVal = changed.Behaviour.isInitialized;
        changed.LoadNew();
        if(newVal != oldVal) 
            changed.Behaviour.SetAgent();
    }

    private void SetAgent()
    {
        foreach (GameObject agents in _agentModels)
        {
            agents.gameObject.SetActive(false);
        }
        
        AgentMaterialCustomization materialCustomization = _agentModels[SelectedAgentIndex].GetComponent<AgentMaterialCustomization>();
        materialCustomization.SetColor(ColorCustomizationIndex);
        
        _agentModels[SelectedAgentIndex].SetActive(true);
        
        _playerUIController.PlayerInfoCanvas = GetComponent<PlayerInfoCanvas>();
        _playerUIController.InitializePlayerInfo(Nickname, SelectedAgentIndex);
        
        foreach (GameObject agents in _agentModels)
        {
            if(!agents.activeSelf) Destroy(agents);
        }
        GetComponent<EntityAnimationController>().Animator = _agentModels[SelectedAgentIndex].GetComponent<Animator>();
        //TODO assign the right stats scriptable object of agent to player controller and any other related scripts
    }
}
