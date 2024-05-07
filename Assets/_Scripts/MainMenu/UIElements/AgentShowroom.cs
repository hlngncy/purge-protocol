using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class AgentShowroom : MonoBehaviour
{
    [SerializeField] private List<GameObject> _agents;
    [SerializeField] private TextMeshProUGUI _agentName;

    private void Awake()
    {
        int currentAgent = PlayerPrefs.GetInt("AgentID", 0);
        _agents[currentAgent].SetActive(true);
        ChangeAgentName(currentAgent);
    }

    public void OnAgentSelected(int previous, int current)
    {
        _agents[previous].SetActive(false);
        _agents[current].SetActive(true);
        ChangeAgentName(current);
    }

    private void ChangeAgentName(int agentID)
    {
        switch (agentID)
        {
            case 0:
                _agentName.text = "Vidar";
                break;
            case 1:
                _agentName.text = "Valkyrie";
                break;
            case 2:
                _agentName.text = "Valeria";
                break;
            default:
                throw new InvalidDataException("Agent ID is invalid!");
        }
    }
}
