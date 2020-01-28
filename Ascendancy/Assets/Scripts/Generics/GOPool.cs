﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOPool : MonoBehaviour
{
    public Transform targetParent;
    public GameObject prefab;
    public List<GameObject> pool { get; protected set; }

    void Start()
    {
        pool = new List<GameObject>();
        if (targetParent == null)
            targetParent = this.transform;
    }

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

        return returnList;
    }
}