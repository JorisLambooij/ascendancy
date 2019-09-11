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

    private void Start()
    {
        economy = GetComponent<Economy>();
        economy.Initialize();

        techLevel = GetComponent<TechnologyLevel>();
        techLevel.Initialize();
    }
}
