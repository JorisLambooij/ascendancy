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

    public static Resource GetResourceFromString(string s)
    {
        if (!instance.resourceData.ContainsKey(s))
        {
            Debug.LogError(s + " not present in ResourceLoader.ResourceData");
            return null;
        }
        return instance.resourceData[s];
    }

    public static List<EntityInfo> BuildingsForProduction(Resource r)
    {
        Dictionary<EntityInfo, float> results = new Dictionary<EntityInfo, float>();
        foreach(var kvp in instance.entityInfoData)
        {
            if (kvp.Value.construction_Method != ConstructionMethod.Building)
                continue;

            float score = 0;

            // go through the features, and if one produces the desired resource, add it as a result
            foreach (EntityFeature feature in kvp.Value.entityFeatures)
                if (feature is ProductionFeature && (feature as ProductionFeature).producedResource == r)
                    score += (feature as ProductionFeature).producedAmount;

            if (score > 0)
                results.Add(kvp.Value, score);
        }

        List<EntityInfo> sortedResults = new List<EntityInfo>(results.Keys);
        sortedResults.Sort((a, b) => results[a].CompareTo(results[b]));
        return sortedResults;
    }
}
