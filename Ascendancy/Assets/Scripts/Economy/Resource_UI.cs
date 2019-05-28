using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource_UI : MonoBehaviour
{
    public Economy economy;
    public GameObject resourceEntryPrefab;

    private Dictionary<Resource, Resource_UI_Entry> resourceEntries;

    // Start is called before the first frame update
    void Start()
    {
        resourceEntries = new Dictionary<Resource, Resource_UI_Entry>();

        foreach (KeyValuePair<Resource, float> kvp in economy.resourceStorage)
        {
            Resource_UI_Entry entry = Instantiate(resourceEntryPrefab, this.transform).GetComponent<Resource_UI_Entry>();
            entry.Sprite = kvp.Key.icon;
            entry.Count = kvp.Value;

            resourceEntries.Add(kvp.Key, entry);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<Resource, float> kvp in economy.resourceStorage)
        {
            // Update the count of each resource
            resourceEntries[kvp.Key].Count = kvp.Value;
        }
    }
}
