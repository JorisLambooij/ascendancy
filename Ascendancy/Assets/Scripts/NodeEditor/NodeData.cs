using UnityEngine;

[System.Serializable]
public class NodeData
{

    public int id_Node;
    public Vector2 position;
}

[System.Serializable]
public class NodeDataCollection
{
    public Vector2 initialOffset;
    public int gridSnap;
    public NodeData[] nodeDataCollection;
}