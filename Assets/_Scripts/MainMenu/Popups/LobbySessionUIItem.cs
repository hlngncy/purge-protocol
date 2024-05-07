using System;
using System.Diagnostics;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySessionUIItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyName;
    [SerializeField] private TextMeshProUGUI _lobbyPlayerCountText;
    [SerializeField] private TextMeshProUGUI _lobbyTitle;
    [SerializeField] private TextMeshProUGUI _lobbyDifficulty;
    [SerializeField] private Button _joinButton;

    private SessionInfo _sessionInfo;
    private bool _isButtonActive = true;
    private string _tempDiffString;
    public event Action<SessionInfo> OnJoinSession;

    public void SetSessionInfo(SessionInfo sessionInfo)
    {
        _sessionInfo = sessionInfo;
        gameObject.name = sessionInfo.Name;
        SetLobbyUI();
    }

    private void SetLobbyUI()
    {
        _lobbyName.text = _sessionInfo.Name;
        _lobbyPlayerCountText.text = $"{_sessionInfo.PlayerCount.ToString()}/{_sessionInfo.MaxPlayers.ToString()}";
        _isButtonActive = _sessionInfo.PlayerCount < _sessionInfo.MaxPlayers;
        //TODO handle lobby state too
        _joinButton.interactable = _isButtonActive;
        _lobbyTitle.text = _sessionInfo.Properties["sessionTitle"].PropertyValue.ToString();
        
        switch (_sessionInfo.Properties["difficulty"].PropertyValue)
        {
            case 0:
                _tempDiffString = "Easy";
                break;
            case 1:
                _tempDiffString = "Normal";
                break;
            case 2:
                _tempDiffString = "Hard";
                break;
            case 3:
                _tempDiffString = "Expert";
                break;
        }
        
        _lobbyDifficulty.text = _tempDiffString;
    }

    public void OnJoinButtonClicked()
    {
        OnJoinSession?.Invoke(_sessionInfo);
    }
}
