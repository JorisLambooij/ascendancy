using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSON_To_Tech_Converter
{
    public static Technology Convert(JSON_Technology jsonTech)
    {
        string path = "Sprites/Technologies/" + System.IO.Path.GetFileNameWithoutExtension(jsonTech.iconPath);
        Sprite icon = Resources.Load<Sprite>(path);
        Technology tech = new Technology(jsonTech, icon);

        return tech;
    }
}
