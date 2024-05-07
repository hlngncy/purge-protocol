using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class LobbySessionListHandler : MonoBehaviour, IPopup
{
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private GameObject _sessionUIObject;
    [SerializeField] private VerticalLayoutGroup _verLayGroup;
    [SerializeField] private TMP_InputField _idToSearch;

    private List<LobbySessionUIItem> _sessionUIItems = new List<LobbySessionUIItem>(); 

    private void Awake()
    {
        ClearList();
    }

    private void SetSessionList(List<SessionInfo> sessionList)
    {
        ClearList();
        _statusText.gameObject.SetActive(false);
        foreach (SessionInfo sessionInfo in sessionList)
        {
            AddToList(sessionInfo);
        }
    }
    private void ClearList()
    {
        foreach (Transform child in _verLayGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddToList(SessionInfo sessionInfo)
    {
        LobbySessionUIItem lobbySessionUIItem =
            Instantiate(_sessionUIObject, _verLayGroup.transform).GetComponent<LobbySessionUIItem>();
        lobbySessionUIItem.SetSessionInfo(sessionInfo);
        _sessionUIItems.Add(lobbySessionUIItem);
        lobbySessionUIItem.OnJoinSession += OnJoinSession;
    }

    private void OnJoinSession(SessionInfo obj)
    {
        MainMenuManager.Instance.JoinLobby(obj);
    }

    public void SetSessionStatus(bool isThereLobby, List<SessionInfo> sessionList)
    {
        _statusText.text = isThereLobby ? "Active Lobbies" : "No lobby found.";
        _statusText.gameObject.SetActive(true);
        if(isThereLobby)  SetSessionList(sessionList);
        else ClearList();
        MainMenuManager.Instance.ListedLobbies(sessionList);
    }

    public void SearchSession(string input)
    {
        FindSession(input);
    }

    private void FindSession(string input)
    {
        foreach (LobbySessionUIItem sessionUIItem in _sessionUIItems)
        {
            sessionUIItem.gameObject.SetActive(sessionUIItem.name.StartsWith(input));
        }
    }
    
    public void OpenPopup()
    {
        MainMenuManager.Instance.FindOpenSessions();
        _statusText.text = "Searching...";
        _statusText.gameObject.SetActive(true);
    }

    public void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }
}
