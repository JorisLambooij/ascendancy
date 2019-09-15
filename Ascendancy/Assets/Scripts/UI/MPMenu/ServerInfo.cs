using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInfo : MonoBehaviour
{
    [SerializeField]
    public string ServerName { get; set; }

    [SerializeField]
    public float Latency { get; set; }

    [SerializeField]
    public bool Password { get; set; }

    [SerializeField]
    public int PlayerCountCurrent { get; set; }

    [SerializeField]
    public int PlayerCountMax { get; set; }

    [SerializeField]
    public  string IPAddress { get; set; }
}
