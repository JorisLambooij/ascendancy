using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;

namespace Mirror.Discovery
{
    [RequireComponent(typeof(GOPool))]
    public class MP_ServerList : MonoBehaviour
    {
        GOPool serverPool;

        // Start is called before the first frame update
        void Start()
        {
            serverPool = GetComponent<GOPool>();
            
        }

        // Update is called once per frame
        public void CreateList(Dictionary<long, MPMenu_ServerResponse> discoveredServers)
        {
            foreach (MPMenu_ServerResponse info in discoveredServers.Values)
            {
                Debug.Log("Server found: " + info.nameHost);
            }
        }
    }
}
