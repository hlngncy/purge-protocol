using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panels;
    [SerializeField] private CanvasGroup _loadingIcon;
    [SerializeField] private Transform _characterTint;
    [SerializeField] private Vector2 characterTintTargetPos;
    [SerializeField] private Transform _panelsTint;
    [SerializeField] private Vector2 panelsTintTargetPos;
    [SerializeField] private Transform _blockImage;
    [SerializeField] private Transform _backGround;


    public void OnJoinLobby()
    {
        _blockImage.gameObject.SetActive(true);
        _backGround.GetComponent<RelativeMovement>().enabled = false;
        _backGround.DOLocalMove(Vector3.zero, .4f);
        _panels.DOFade(0, .5f);
        _characterTint.DOLocalMove(characterTintTargetPos, 1.3f);
        _panelsTint.DOLocalMove(panelsTintTargetPos, 1.3f);
        _loadingIcon.DOFade(1, .5F).SetLoops(-1, LoopType.Yoyo);
    }
}
