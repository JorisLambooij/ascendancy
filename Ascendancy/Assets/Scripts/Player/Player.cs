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

    // When the NetworkManager creates this Player, do this
    private void Awake()
    {
        Transform playerManager = GameObject.Find("PlayerManager").transform;
        transform.SetParent(playerManager);
    }

    public void Initialize()
    {
        Debug.Log("Init player " + playerNo);

        economy = GetComponent<Economy>();
        economy.Initialize();

        techLevel = GetComponent<TechnologyLevel>();
        techLevel.Initialize();
    }
}
