using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Video;

public class PlayerInfoCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup _parentCG;
    [SerializeField] private TextMeshProUGUI _nickName;
    [SerializeField] private TextMeshProUGUI _agentName;
    private PlayerUIController _playerUIController;

    private void Start()
    {
        _playerUIController = GameObject.FindWithTag("PlayerUI").GetComponent<PlayerUIController>();
        _playerUIController.DeactivateInfoCanvas.AddListener(Deactivate);
    }

    public void InitializePlayerInfo(string nickName, int agentName)
    {
        _nickName.text = nickName;
        string agentNameStr;
        switch (agentName)
        {
            case 0:
                agentNameStr = "Vidar";
                break;
            case 1:
                agentNameStr = "Valkyrie";
                break;
            case 2:
                agentNameStr = "Valeria";
                break;
            default:
                throw new Exception("Player agent id is not valid.");
        }
        _agentName.text = agentNameStr;
        _parentCG.DOFade(1, 1f);
    }

    private void Deactivate()
    {
        _parentCG.DOFade(0, .3f).OnComplete(() => _parentCG.gameObject.SetActive(false));
    }
}
