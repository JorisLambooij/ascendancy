using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Resource_UI : MonoBehaviour, DictionarySubscriber<Resource, float>, ListSubscriber<Resource>
{
    public GameObject resourceEntryPrefab;

    private Player player;
    private Dictionary<Resource, Resource_UI_Entry> resourceEntries;

    private const float rollingAverageUpdateFrequency = 0.5f;
    private Dictionary<Resource, RollingAverage> rollingAverageProduction;

    /// <summary>
    /// Callback for when a resource has been updated.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newValue"></param>
    public void Callback(Resource key, float newValue)
    {
        if (resourceEntries.ContainsKey(key))
            resourceEntries[key].Count = newValue;
        else
            InstantiateNewField(key, newValue);
    }
    
    /// <summary>
    /// Callback for when a new Resource has become available.
    /// </summary>
    /// <param name="updatedValue"></param>
    public void NewElementCallback(Resource updatedValue)
    {
        if (!resourceEntries.ContainsKey(updatedValue))
            InstantiateNewField(updatedValue);
    }

    public void NewListCallback(List<Resource> newList)
    {
        // TODO: Reset the UI to match the new Resources.
    }

    protected void OnResourceChange(SyncDictionary<string, float>.Operation op, string resourcename, float newValue)
    {
        Resource r = ResourceLoader.instance.resourceData[resourcename];
        if (resourceEntries.ContainsKey(r))
        {
            float delta = newValue - resourceEntries[r].Count;
            resourceEntries[r].Count = newValue;
            rollingAverageProduction[r].QueueDatapoint(delta);
        }
        else
            InstantiateNewField(r, newValue);
    }

    protected void OnNewResource(SyncList<string>.Operation op, int index, string oldResource, string newResource)
    {
        if (op != SyncList<string>.Operation.OP_ADD)
        {
            Debug.LogError("Unknown operation on AvailableResources SyncList: " + op.ToString());
            return;
        }

        Resource r = ResourceLoader.instance.resourceData[newResource];
        if (!resourceEntries.ContainsKey(r))
            InstantiateNewField(r);
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.playerScript;

        resourceEntries = new Dictionary<Resource, Resource_UI_Entry>();

        player.PlayerEconomy.resourceSyncDictionary.Callback += OnResourceChange;
        player.PlayerEconomy.availableResources.Callback += OnNewResource;
        rollingAverageProduction = new Dictionary<Resource, RollingAverage>();

        foreach (KeyValuePair<string, float> kvp in player.PlayerEconomy.resourceSyncDictionary)
        {
            Resource r = ResourceLoader.instance.resourceData[kvp.Key];
            InstantiateNewField(r, kvp.Value);
        }
        StartCoroutine(RollingAverage());
    }

    private IEnumerator RollingAverage()
    {
        while (true)
        {
            foreach (KeyValuePair<Resource, RollingAverage> kvp in rollingAverageProduction)
            {
                //rollingAverageProduction[kvp.Key].QueueDatapoint(resourceEntries[kvp.Key].Count);
                float avg = kvp.Value.Calculate();
                resourceEntries[kvp.Key].Production = avg;
            }
            yield return new WaitForSeconds(rollingAverageUpdateFrequency);
        }
    }

    void InstantiateNewField(Resource resource, float amount = 0)
    {
        Resource_UI_Entry entry = Instantiate(resourceEntryPrefab, this.transform).GetComponent<Resource_UI_Entry>();
        entry.Sprite = resource.icon;
        entry.Count = amount;

        resourceEntries.Add(resource, entry);
        rollingAverageProduction.Add(resource, new RollingAverage());
    }

    // Update is called once per frame
    void Update()
    {
        /*
        foreach (KeyValuePair<Resource, float> kvp in player.PlayerEconomy.resourceStorage.AsDictionary)
        {
            // Update the count of each resource
            if (!resourceEntries.ContainsKey(kvp.Key))
            {
                InstantiateNewField(kvp.Key, kvp.Value);
                Debug.Log("Instantiating new field for: " + kvp.Key.name + ". Please check");
            }
            resourceEntries[kvp.Key].Count = kvp.Value;
        }
        */
    }
}
