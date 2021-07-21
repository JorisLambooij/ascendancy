using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    public static ResourceLoader instance;
    public Dictionary<string, EntityInfo> entityInfoData = new Dictionary<string, EntityInfo>();
    public Dictionary<string, Resource> resourceData = new Dictionary<string, Resource>();
    public GameObject constructionSitePrefab;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

        Object[] allSOs = Resources.LoadAll("ScriptableObjects");

        FillDict(entityInfoData, allSOs, (info => info.name));
        FillDict(resourceData, allSOs, (info => info.name));

        Debug.Log(entityInfoData.Count);
    }

    private void FillDict<T>(Dictionary<string, T> dictionary, Object[] allSOs, System.Func<T, string> getName) where T : ScriptableObject
    {
        //dictionary = new Dictionary<string, T>();
        foreach (Object obj in allSOs)
            if (obj is T)
            {
                T t = obj as T;
                string name = getName(t);
                if (name == "")
                {
                    Debug.LogError(name + " name is empty");
                    continue;
                }

                if (entityInfoData.ContainsKey(name))
                    Debug.Log(name + " already in Dict " + nameof(dictionary));

                dictionary.Add(name, t);
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
