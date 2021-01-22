using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCategory", menuName = "Entity Category")]
public class EntityCategoryInfo : ScriptableObject
{
    new string name;
    public Sprite icon;
    public string description;
}
