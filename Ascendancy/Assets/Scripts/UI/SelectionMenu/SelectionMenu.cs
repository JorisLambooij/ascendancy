using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMenu : MonoBehaviour, ListSubscriber<EntitySelector>
{
    //List<Entity> entities;
    List<List<Entity>> stacks;

    public GameObject selectionStackPrefab;
    public GameObject stackElementPrefab;
    public GameObject content;

    public void NewElementCallback(EntitySelector updatedValue)
    {
        //throw new System.NotImplementedException();
        Debug.Log("New element");
    }

    public void Clear()
    {
        stacks.Clear();
        //entities.Clear();

        //delete all UI stacks
        foreach (Transform child in content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void NewListCallback(List<EntitySelector> newList)
    {
        Clear();
        List<string> typeList = new List<string>();

        //building the Data structure
        if (newList.Count > 0)
        {
            foreach (EntitySelector ent in newList)
            {
                if (typeList.Contains(ent.ParentEntity.entityInfo.name))
                {
                    foreach (List<Entity> stack in stacks)
                    {
                        if (stack[0].name == ent.ParentEntity.entityInfo.name)
                        {
                            stack.Add(ent.ParentEntity);
                                break;
                        }
                    }
                }
                else
                {
                    typeList.Add(ent.ParentEntity.entityInfo.name);

                    List<Entity> entList = new List<Entity>
                    {
                        ent.ParentEntity
                    };
                    stacks.Add(entList);
                }
            }

            //building the UI
            foreach (List<Entity> stack in stacks)
            {
                GameObject newStack = Instantiate(selectionStackPrefab, content.transform);
                GameObject panel = null;

                foreach (Transform propablyPanel in newStack.transform)
                {
                    if (propablyPanel.name == "Panel")
                    {
                        panel = propablyPanel.gameObject;
                        break;
                    }
                }

                if (panel == null)
                {
                    Debug.LogError("Panel not found in Selection Menu!");
                    return;
                }

                int number = 0;

                foreach (Entity ent in stack)
                {
                    if (number < 4)
                    {
                        GameObject stackElem = Instantiate(stackElementPrefab, panel.transform);
                        Image img = stackElem.GetComponent<StackElementScript>().thumbnailImage;
                        img.sprite = ent.entityInfo.thumbnail;

                        stackElem.GetComponent<StackElementScript>().healthbar.GetComponent<Healthbar>().SetEntity(ent);

                    }

                    number++;
                }
                Text t = newStack.GetComponentInChildren<Text>();
                t.text = number + "x";

            }

            string selection = "|";

            foreach (List<Entity> stack in stacks)
            {
                selection += stack[0].name + " x " + stack.Count;
            }

                Debug.Log("Selected " + selection);
        }
        else
        {

        }
    }



    public void Start()
    {
        (GameManager.Instance.controlModeDict[ControlModeEnum.gameMode] as GameMode).selectedUnits.Subscribe(this);
        //entities = new List<Entity>();
        stacks = new List<List<Entity>>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
