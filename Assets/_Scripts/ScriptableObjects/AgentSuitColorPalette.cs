using System;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AgentSuitColorPalette", order = 1)]
[Obsolete("Obsolete")]
internal class AgentSuitColorPalette : ScriptableObject
{
    [ColorUsage(true,true,0f,8f,0.125f,3f)] public Color[] black;
    [ColorUsage(true,true,0f,8f,0.125f,3f)] public Color[] white;
    [ColorUsage(true,true,0f,8f,0.125f,3f)] public Color[] blue;
    [ColorUsage(true,true,0f,8f,0.125f,3f)]  public Color[] red;
    [ColorUsage(true,true,0f,8f,0.125f,3f)] public Color[] purple;
    [ColorUsage(true,true,0f,8f,0.125f,3f)] public Color[] yellow;
}
