﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    public static ResourceLoader instance;
    public Dictionary<string, EntityInfo> entityInfoData;
    public GameObject constructionSitePrefab;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

        Object[] allSOs = Resources.LoadAll("ScriptableObjects");

        entityInfoData = new Dictionary<string, EntityInfo>();
        foreach (Object obj in allSOs)
            if (obj is EntityInfo)
            {
                EntityInfo info = obj as EntityInfo;
                if (info.name == "")
                {
                    Debug.LogError(obj.name + " name is empty");
                    continue;
                }
                if (entityInfoData.ContainsKey(info.name))
                    Debug.Log(info.name + " already in Dict");
                entityInfoData.Add(info.name, info);
            }
    }

    public static EntityInfo GetEntityInfo(string entityName)
    {
        if (!instance.entityInfoData.ContainsKey(entityName))
        {
            Debug.LogError(entityName + " not present in ResourceLoader.EntityInfoData");
            return null;
        }

        return instance.entityInfoData[entityName];
    }
}
