using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource_UI : MonoBehaviour, DictionarySubscriber<Resource, float>, ListSubscriber<Resource>
{
    public GameObject resourceEntryPrefab;

    private Player player;
    private Dictionary<Resource, Resource_UI_Entry> resourceEntries;

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

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.playerScript;

        resourceEntries = new Dictionary<Resource, Resource_UI_Entry>();

        player.PlayerEconomy.resourceStorage.Subscribe(this);
        player.PlayerEconomy.availableResources.Subscribe(this);
        
        foreach (KeyValuePair<Resource, float> kvp in player.PlayerEconomy.resourceStorage.AsDictionary)
            InstantiateNewField(kvp.Key, kvp.Value);
    }

    void InstantiateNewField(Resource resource, float amount = 0)
    {
        Resource_UI_Entry entry = Instantiate(resourceEntryPrefab, this.transform).GetComponent<Resource_UI_Entry>();
        entry.Sprite = resource.icon;
        entry.Count = amount;

        resourceEntries.Add(resource, entry);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<Resource, float> kvp in player.PlayerEconomy.resourceStorage.AsDictionary)
        {
            // Update the count of each resource
            if (!resourceEntries.ContainsKey(kvp.Key))
            {
                InstantiateNewField(kvp.Key, kvp.Value);
                Debug.Log("Instantiating new field for: " + kvp.Key.resourceName + ". Please check");
            }
            resourceEntries[kvp.Key].Count = kvp.Value;
        }
    }
}
