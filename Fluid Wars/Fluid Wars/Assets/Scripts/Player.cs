﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        transform.name = "Player " + id;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
