using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Resource SO", order = 1)]
public class Resource : ScriptableObject
{
    public string resourceName;

    public Sprite icon;
}
