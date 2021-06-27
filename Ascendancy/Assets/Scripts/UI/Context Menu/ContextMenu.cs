using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour, ListSubscriber<EntitySelector>
{
    List<Entity> entities;

    public void NewElementCallback(EntitySelector updatedValue)
    {
        //throw new System.NotImplementedException();
    }

    public void NewListCallback(List<EntitySelector> newList)
    {
        if (newList.Count == 1)
        {
            BuildingConversionFeature f = newList[0].ParentEntity.FindFeature<BuildingConversionFeature>();
            if (f == null)
                return;
            GetComponentInChildren<Button>().GetComponentInChildren<Image>().sprite = f.contextMenuThumbnail;
            entities = new List<Entity>() { newList[0].ParentEntity };
            Debug.Log("New list");
        }
        else
        {

        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        (GameManager.Instance.controlModeDict[ControlModeEnum.gameMode] as GameMode).selectedEntities.Subscribe(this);
        GetComponentInChildren<Button>().onClick.AddListener(OnClick);
        entities = new List<Entity>();
    }

    // Update is called once per frame
    void OnClick()
    {
        entities[0].FindFeature<BuildingConversionFeature>().ContextMenuOption();
    }
}
