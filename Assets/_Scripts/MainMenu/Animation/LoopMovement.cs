using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LoopMovement : MonoBehaviour
{
    [SerializeField] private Transform _objectToLoop;
    [SerializeField] private Vector2 _loopPosition;
    [SerializeField] private float _delay;
    private Vector2 _startPos;
 
    private void OnEnable()
    {
        _startPos = _objectToLoop.position;
        Invoke(nameof(StartMove),_delay);
        //_objectToLoop.DOLocalMove(_loopPosition, 1f).OnComplete(() => _objectToLoop.DOLocalMove(_startPos,1f)).SetLoops(-1);
    }

    private void StartMove()
    {
        _objectToLoop.DOLocalMove(_loopPosition, 2f).SetLoops(-1, loopType: LoopType.Yoyo).SetEase(Ease.Linear);
    }
}
