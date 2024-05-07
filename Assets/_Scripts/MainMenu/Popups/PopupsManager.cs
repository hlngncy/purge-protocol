using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PopupsManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _popupsGO;
    private List<IPopup> _popups = new List<IPopup>();

    private void Awake()
    {
        foreach (var popup in _popupsGO)
        {
            _popups.Add(popup.GetComponent<IPopup>());
        }
    }

    public void OpenPopup(int popupID)
    {
        _popupsGO[popupID].SetActive(true);
        _popupsGO[popupID].GetComponent<CanvasGroup>().DOFade(1, .5F);
        _popups[popupID].OpenPopup();
    }

    public void ClosePopup(int popupID)
    {
        _popupsGO[popupID].GetComponent<CanvasGroup>().DOFade(0, .3F).OnComplete(() => _popups[popupID].ClosePopup());
        
    }
}
