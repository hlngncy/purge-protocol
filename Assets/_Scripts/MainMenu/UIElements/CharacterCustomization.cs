using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomization : MonoBehaviour
{
    [SerializeField] private List<Color> _colorPalette;
    [SerializeField] private Image _colorImage;
    private Color _tempColor;
    private int _colorID;

    private void Start()
    {
        SelectColor(PlayerPrefs.GetInt("AgentColorID",0));
    }

    public void SelectColor(int colorID)
    {
        _colorID = colorID;
        _tempColor = _colorPalette[_colorID];
        ChangeColor();
    }

    private void ChangeColor()
    {
        _colorImage.color = _tempColor;
        PlayerPrefs.SetInt("AgentColorID", _colorID);
    }
    
    private void OnDestroy()
    {
        Debug.Log("mainmenu:"+PlayerPrefs.GetInt("AgentColorID"));
    }
}
