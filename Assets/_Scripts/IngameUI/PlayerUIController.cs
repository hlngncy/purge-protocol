using System;
using DG.Tweening;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerUIController : MonoBehaviour
{
    [FormerlySerializedAs("_preperationStateCanvas")]
    [Header("WaitingCanvas")]
        [SerializeField] private CanvasGroup _waitCanvasGroup;
        [SerializeField] private LobbyWaitCanvas _waitCanvas;
        
    [Header("IngameCanvas")]
        [SerializeField] private CanvasGroup _fightCanvasGroup;
        [SerializeField] private FightCanvas _fightCanvas;
        
    public PlayerInfoCanvas PlayerInfoCanvas
    {
        set
        {
            _playerInfoCanvas = value;
        }
    }

    public UnityEvent DeactivateInfoCanvas = new();

    private NetworkRunner _runner;
    private CanvasGroup _currentStateCanvas;
    private int _fadeDuration = 1;

    private void Start()
    { 
        _runner = GameObject.FindGameObjectWithTag("NetworkRunner").GetComponent<NetworkRunner>();
    }

    private PlayerInfoCanvas _playerInfoCanvas;

    public void ChangeCanvas(bool activate, GameStates state)
    {
        Debug.Log("change canvas: " + activate + state);
        if (activate) ActivateCurrentState(state);
        else DeactivateCurrentCanvas();
    }
    
    private void ActivateCurrentState(GameStates states)
    {
        switch (states)
        {
            case GameStates.Await:
                _currentStateCanvas = _waitCanvasGroup;
                break;
            case GameStates.Fight:
                _currentStateCanvas = _fightCanvasGroup;
                break;
            case GameStates.Preparation:
                _currentStateCanvas = _waitCanvasGroup;
                break;
            case GameStates.End:
                _currentStateCanvas = _waitCanvasGroup;
                break;
        }

        _currentStateCanvas.alpha = 0;
        _currentStateCanvas.gameObject.SetActive(true);
        _currentStateCanvas.DOFade(1, _fadeDuration);
    }

    private void DeactivateCurrentCanvas()
    {
        _currentStateCanvas.DOFade(0, .3F);
    }
    public void InitializePlayerInfo(string nickName, int agentName)
    {
        _playerInfoCanvas.InitializePlayerInfo(nickName, agentName);
    }

    public void DeactivatePlayerInfo()
    {
        DeactivateInfoCanvas.Invoke();
    }

    public void InitializeWaitingCanvas(SessionInfo sessionInfo)
    {
        _waitCanvas.Setup(sessionInfo, _runner.IsServer);
    }
    
    public void InitializePreperationElements()
    {
        //todo set prep canvas via its script
    }
    
    public void InitializeFightElements()
    {
        //todo set fight canvas via its script
    }
}
