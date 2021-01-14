using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Resource", menuName = "Resource SO", order = 1)]
public class Resource : ScriptableObject
{
    public int initialAmount;
    public Sprite icon;
}
