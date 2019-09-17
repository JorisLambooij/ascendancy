using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNo;
    public string playerName;
    public Color playerColor;

    public Economy economy;
    public TechnologyLevel techLevel;
    public GameObject buildingsGO;
    public GameObject unitsGO;

    public void Initialize()
    {
        Debug.Log("init player " + playerNo);

        economy = GetComponent<Economy>();
        economy.Initialize();

        techLevel = GetComponent<TechnologyLevel>();
        techLevel.Initialize();
    }
}
