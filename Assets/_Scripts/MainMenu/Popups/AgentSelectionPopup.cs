using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentSelectionPopup : MonoBehaviour,IPopup
{
    [NonSerialized] public Observable<int> _selectedAgentID = new Observable<int>();
    [NonSerialized] public int _currentAgentID;
    [SerializeField] private List<GameObject> _agentsUIObjects;

    private void Awake()
    {
        int currentAgent = PlayerPrefs.GetInt("AgentID", 0);
        _selectedAgentID.Value = currentAgent;
        _currentAgentID = currentAgent;
        MainMenuManager.Instance.InitializeListenersForAgentSelection();
        UpdatePopupUI(currentAgent);
    }

    //changing the user interface
    public void ChooseAgent(int agentID)
    {
        UpdatePopupUI(agentID);
        _currentAgentID = agentID;
        Debug.Log("choosenagentıd" + agentID);
    }

    //saving selected agent for other components
    public void SelectAgent(int agentID)
    {
        if(_selectedAgentID.Value == agentID) return;
        _selectedAgentID.Value = agentID;
        Debug.Log("Selectedagentıd" + agentID);
    }

    private void UpdatePopupUI(int agentID)
    {
        _agentsUIObjects[_currentAgentID].SetActive(false);
        _agentsUIObjects[agentID].SetActive(true);
        
    }
    

    public void OpenPopup()
    {
        //do some ui anims
    }

    public void ClosePopup()
    {
        //do some ui anims
        this.gameObject.SetActive(false);
    }
}
