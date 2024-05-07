using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Random = UnityEngine.Random;

public class PlayerPreferences : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _namePlaceHolder;
    [SerializeField] private TextMeshProUGUI _nameInput;
    [SerializeField] private Button _saveButton;
    [SerializeField] private List<GameObject> _agentIconUIObjects;

    private void Awake()
    {
        CheckPlayerName();
        CheckAgentIcon();
    }

    private void CheckAgentIcon()
    {
        int currentAgent = PlayerPrefs.GetInt("AgentID", 0);
        _agentIconUIObjects[currentAgent].SetActive(true);
    }

    private void CheckPlayerName()
    {
        _namePlaceHolder.text = PlayerPrefs.GetString("PlayerName", GeneratePlayerName());
        SavePlayerName();
    }

    private string GeneratePlayerName()
    {
        return "Soldier" + Random.Range(0, 126);
    }

    public void SavePlayerName()
    {
        string playerName = new string(_nameInput.text.Where(c => !char.IsWhiteSpace(c)).ToArray());
        
        if (playerName.Length < 2)
            playerName = _namePlaceHolder.text;
        
        if (playerName.Length >= 4 & playerName.Length < 20)
        {
            PlayerPrefs.SetString("PlayerName",playerName);
        }
    }

    public void SaveAgentPreference(int previous, int current)
    {
        PlayerPrefs.SetInt("AgentID",current);
        UpdateIconUI(previous,current);
    }
    private void UpdateIconUI(int previous, int current)
    {
        Debug.Log("SelectedagentıdCurrent" + current);
        Debug.Log("SelectedagentıdPrevious" + previous);
        _agentIconUIObjects[current].SetActive(true);
        _agentIconUIObjects[previous].SetActive(false);
    }
}
