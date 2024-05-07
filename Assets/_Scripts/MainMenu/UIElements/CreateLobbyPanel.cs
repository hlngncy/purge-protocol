using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class CreateLobbyPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyNameInput;
    [SerializeField] private TextMeshProUGUI _lobbyNamePH;
    private string _lobbyName;
    private LobbyInfo _lobbyInfo = new LobbyInfo();
    private int _difficulty;
    [SerializeField] private List<GameObject> _selectedDifficultyBG;
    
    private void Start()
    {
        _lobbyNamePH.text = $"{PlayerPrefs.GetString("PlayerName")}'s Lobby";
        SetSelected(1);
    }

    public void CreateLobbyOnClick()
    {
        SetDifficulty();
        SetLobbyName();
        Debug.Log(_lobbyInfo.lobbyName);
        MainMenuManager.Instance.CreateLobby(_lobbyInfo);
    }

    private void SetLobbyName()
    {
        //string volatileLobbyName = new string(_lobbyNameInput.text.Where(c => !char.IsWhiteSpace(c)).ToArray());
        string tempLobbyName = _lobbyNameInput.text;
            
        if (tempLobbyName.Length >= 4 & tempLobbyName.Length < 20)
        {
            _lobbyName = tempLobbyName;
        }
        else
        {
            _lobbyName = _lobbyNamePH.text;
        }
        
        _lobbyInfo.lobbyName = _lobbyName;
    }

    private void SetDifficulty()
    {
        switch (_difficulty)
        {
            case 0:
                _lobbyInfo.difficulty = LobbyDifficulty.Easy;
                break;
            case 1:
                _lobbyInfo.difficulty = LobbyDifficulty.Normal;
                break;
            case 2:
                _lobbyInfo.difficulty = LobbyDifficulty.Hard;
                break;
            case 3:
                _lobbyInfo.difficulty = LobbyDifficulty.Expert;
                break;
        }
    }

    public void SetSelected(int difficulty)
    {
        _difficulty = difficulty;
        foreach (var bg in _selectedDifficultyBG)
        {
            bg.SetActive(false);
        }
        _selectedDifficultyBG[_difficulty].SetActive(true);
    }
}

public struct LobbyInfo
{
    public string lobbyName;
    public LobbyDifficulty difficulty;
    public string lobbyID;
}
