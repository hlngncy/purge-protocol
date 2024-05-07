using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMaterialCustomization : MonoBehaviour
{
    private Color[] _tempColor;
    private List<Material> _materials = new List<Material>();
    [SerializeField] private AgentSuitColorPalette _colorPalette;
    
    public void SetColor(int colorID)
    {
        switch (colorID)
        {
            case 0:
                _tempColor = _colorPalette.black;
                break;
            case 1:
                _tempColor = _colorPalette.white;
                break;
            case 2:
                _tempColor = _colorPalette.purple;
                break;
            case 3:
                _tempColor = _colorPalette.blue;
                break;
            case 4:
                _tempColor = _colorPalette.yellow;
                break;
            case 5:
                _tempColor = _colorPalette.red;
                break;
            default:
                Debug.Log("default");
                break;
        }
        ChangeColor();
    }

    private void ChangeColor()
    {
        GetMaterials();
        foreach (var material in _materials)
        {
            material.SetColor("_Mask_Color1", _tempColor[0]);
            material.SetColor("_Mask_Color2", _tempColor[1]);
            material.SetColor("_Mask_Color3", _tempColor[1]);
        }
    }

    private void GetMaterials()
    {
        for (int i = 1; i < transform.childCount-1; i++)
        {
            Transform temp = transform.GetChild(i);
            Material material = temp.GetComponent<Renderer>().material;
            _materials.Add(material); 
            Debug.Log(material.name);
        }
    }
}
