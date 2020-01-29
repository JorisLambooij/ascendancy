using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSON_To_Tech_Converter
{
    public static Technology Convert(JSON_Technology jsonTech)
    {
        string iconPath = "Sprites/Technologies/" + System.IO.Path.GetFileNameWithoutExtension(jsonTech.iconPath);
        Sprite icon = Resources.Load<Sprite>(iconPath);
        Technology tech = new Technology(jsonTech, icon);

        tech.unitsUnlocked = ConvertList<EntityInfo>(jsonTech.unitsUnlocked);
        tech.buildingsUnlocked = ConvertList<BuildingInfo>(jsonTech.buildingsUnlocked);
        tech.resourcesUnlocked = ConvertList<Resource>(jsonTech.resourcesUnlocked);

        return tech;
    }

    private static T[] ConvertList<T>(string[] strings) where T : ScriptableObject
    {
        T[] infos = new T[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            string path = System.IO.Path.ChangeExtension(strings[i], "").Remove(strings[i].IndexOf('.'));
            T info = Resources.Load<T>(path);
            infos[i] = info;
        }
        return infos;
    }
}
