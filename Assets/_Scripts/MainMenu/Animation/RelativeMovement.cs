using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeMovement : MonoBehaviour
{
    [SerializeField] private Transform _objectToMove;

    private void Update()
    {
        Vector2 mousePosRaw = Input.mousePosition;
        Vector2 mousePosCordinates = Camera.main.ScreenToWorldPoint(mousePosRaw) * -.01f;
        _objectToMove.transform.position = mousePosCordinates;
    }
}
