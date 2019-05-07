using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Unit", menuName = "Unit SO", order = 0)]
public class UnitInfo : ScriptableObject
{
    public string unitName;
    public string description;
    
    public float maxHealth;
    
    public Sprite thumbnail;
}
