using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOPool : MonoBehaviour
{
    public Transform targetParent;
    public GameObject prefab;
    public List<GameObject> pool { get; protected set; }

    protected int currentlyInUse;

    void Awake()
    {
        pool = new List<GameObject>();
        currentlyInUse = 0;
        if (targetParent == null)
            targetParent = this.transform;
    }

    /// <summary>
    /// Returns a list with the specified amount of GameObjects.
    /// </summary>
    /// <param name="amount">How many GOs?</param>
    /// <returns>List of length 'amount'</returns>
    public List<GameObject> Generate(int amount)
    {
        List<GameObject> returnList = new List<GameObject>(amount);
        int i = 0;
        for (; i < amount; i++)
        {
            if (i >= pool.Count)
                pool.Add(Instantiate(prefab, targetParent));

            pool[i].SetActive(true);
            returnList.Add(pool[i]);
        }
        for (; i < pool.Count; i++)
            pool[i].SetActive(false);

        currentlyInUse = amount;
        return returnList;
    }

    /// <summary>
    /// Adds one GameObject to the Pool. Expands the pool if necessary.
    /// </summary>
    /// <returns>A GameObject ready for use.</returns>
    public GameObject Add()
    {
        GameObject returnObj;
        if (currentlyInUse < pool.Count)
        {
            returnObj = pool[currentlyInUse];
            return returnObj;
        }
        else
        {
            returnObj = Instantiate(prefab, targetParent);
            pool.Add(returnObj);
            currentlyInUse++;
        }

        currentlyInUse++;
        returnObj.SetActive(true);
        return returnObj;
    }
}
