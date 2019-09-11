using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class TechTreeEditor : EditorWindow
{
    public const int GRID_SNAP = 100;
    private Vector2 initialOffset;

    private List<Node> nodes;
    private List<Connection> connections;

    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Rect rectButtonClear;
    private Rect rectButtonSave;
    private Rect rectButtonLoad;

    private Vector2 offset;
    private Vector2 drag;

    private int nodeWidth = 160;
    private int nodeHeight = 100;

    private string techPath = TechTreeReader.techPath;
    private string nodePath = TechTreeReader.nodePath;

    private int id;
    private int nodeCount;
    private Dictionary<int, JSON_Technology> techDictionary;

    public static TechTreeEditor instance;

    public SubscribableProperty<Node> selectedNode;

    public Vector2 Offset { get => offset; }

    [MenuItem("Window/Tech Tree Editor")]
    private static void OpenWindow()
    {
        TechTreeEditor window = GetWindow<TechTreeEditor>();
        window.titleContent = new GUIContent("Tech Tree Editor");

        instance = window;
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnEnable()
    {
        instance = this;
        id = 0;
        nodeCount = 0;
        selectedNode = new SubscribableProperty<Node>(null);
        offset = initialOffset;

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        // Create buttons for clear, save and load
        rectButtonClear = new Rect(new Vector2(10, 10), new Vector2(60, 20));
        rectButtonSave = new Rect(new Vector2(80, 10), new Vector2(60, 20));
        rectButtonLoad = new Rect(new Vector2(150, 10), new Vector2(60, 20));

        // Initialize nodes with saved data
        LoadNodes();
    }

    private void ClearNodes()
    {
        selectedNode.Value = null;
        offset = initialOffset;
        nodeCount = 0;
        id = 0;
        if (nodes != null && nodes.Count > 0)
        {
            Node node;
            while (nodes.Count > 0)
            {
                node = nodes[0];
                OnClickRemoveNode(node);
            }
        }
    }

    private void LoadNodes()
    {
        ClearNodes();
        JSON_Technology[] _techTree;
        List<JSON_Technology> originNode = new List<JSON_Technology>();
        techDictionary = new Dictionary<int, JSON_Technology>();
        NodeDataCollection loadedNodeData = null;
        bool nodeDataExists = false;
        string dataAsJson;
        Vector2 pos = Vector2.zero;
        if (File.Exists(techPath))
        {
            if (File.Exists(nodePath))
            {
                dataAsJson = File.ReadAllText(nodePath);
                loadedNodeData = JsonUtility.FromJson<NodeDataCollection>(dataAsJson);
                initialOffset = loadedNodeData.initialOffset;
                offset = initialOffset;
                nodeDataExists = true;
            }
            else
                Debug.LogError("Node Data for '" + techPath + "' not found. Please check '" + nodePath + "'");
            
            // Read the json from the file into a string
            dataAsJson = File.ReadAllText(techPath);

            // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
            JSONTechTree techData = JsonUtility.FromJson<JSONTechTree>(dataAsJson);
            
            // Store the SkillTree as an array of Skill
            _techTree = new JSON_Technology[techData.technologies.Length];
            _techTree = techData.technologies;

            // Create nodes
            for (int i = 0; i < _techTree.Length; ++i)
            {
                Vector2 position = Vector2.zero;
                if (nodeDataExists)
                    for (int j = 0; j < loadedNodeData.nodeDataCollection.Length; j++)
                        if (loadedNodeData.nodeDataCollection[j].id_Node == _techTree[i].id)
                        {
                            position = loadedNodeData.nodeDataCollection[j].position + offset;
                            break;
                        }
                
                if (nodes == null)
                    nodes = new List<Node>();
                
                nodes.Add(new Node(position, nodeWidth, nodeHeight, nodeStyle, selectedNodeStyle, inPointStyle,
                    outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode,
                    _techTree[i]));
                ++nodeCount;

                if (_techTree[i].dependencies.Length == 0)
                    originNode.Add(_techTree[i]);
                
                techDictionary.Add(_techTree[i].id, _techTree[i]);
            }

            JSON_Technology outSkill;
            Node outNode;
            // Create connections
            for (int i = 0; i < nodes.Count; ++i)
                for (int j = 0; j < nodes[i].tech.dependencies.Length; ++j)
                    if (techDictionary.TryGetValue(nodes[i].tech.dependencies[j], out outSkill))
                    {
                        for (int k = 0; k < nodes.Count; ++k)
                            if (nodes[k].tech.id == outSkill.id)
                            {
                                outNode = nodes[k];
                                OnClickOutPoint(outNode.outPoint);
                                break;
                            }
                        
                        OnClickInPoint(nodes[i].inPoint);
                    }
            id = nodeCount;
        }
    }

    private Vector2 SnappedOffset
    {
        get
        {
            Vector2 divPos = offset / GRID_SNAP;
            Vector2 roundedPos = new Vector2(Mathf.Round(divPos.x), Mathf.Round(divPos.y)) * GRID_SNAP;

            return roundedPos;
        }
    }

    // Save data from the window to the file
    private void SaveTechTree()
    {
        if (nodes.Count > 0)
        {
            // We fill with as many techs as we have nodes
            JSONTechTree newTechTree = new JSONTechTree();
            newTechTree.technologies = new JSON_Technology[nodes.Count];
            int[] dependencies;
            List<int> dependenciesList = new List<int>();

            // Iterate over all of the nodes. Populating the tech with the node info
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (connections != null)
                {
                    List<Connection> connectionsToRemove = new List<Connection>();
                    List<ConnectionPoint> connectionsPointsToCheck = new List<ConnectionPoint>();

                    for (int j = 0; j < connections.Count; j++)
                        if (connections[j].inPoint == nodes[i].inPoint)
                        {
                            for (int k = 0; k < nodes.Count; ++k)
                                if (connections[j].outPoint == nodes[k].outPoint)
                                {
                                    dependenciesList.Add(nodes[k].tech.id);
                                    break;
                                }
                            
                            connectionsToRemove.Add(connections[j]);
                            connectionsPointsToCheck.Add(connections[j].outPoint);
                        }
                }
                dependencies = dependenciesList.ToArray();
                dependenciesList.Clear();
                newTechTree.technologies[i] = nodes[i].tech;
                newTechTree.technologies[i].dependencies = dependencies;
            }

            string json = JsonUtility.ToJson(newTechTree, true);

            // Finally, we write the JSON string with the SkillTree data in our file
            using (FileStream fs = new FileStream(techPath, FileMode.Create))
                using (StreamWriter writer = new StreamWriter(fs))
                    writer.Write(json);
            
            UnityEditor.AssetDatabase.Refresh();
        }
    }

    // Save data from the nodes (position in our custom editor window)
    private void SaveNodes()
    {
        NodeDataCollection nodeData = new NodeDataCollection();
        nodeData.initialOffset = SnappedOffset;
        nodeData.gridSnap = GRID_SNAP;
        nodeData.nodeDataCollection = new NodeData[nodes.Count];

        for (int i = 0; i < nodes.Count; ++i)
        {
            nodeData.nodeDataCollection[i] = new NodeData();
            nodeData.nodeDataCollection[i].id_Node = nodes[i].tech.id;
            nodeData.nodeDataCollection[i].position = nodes[i].rect.position - offset;
        }

        string json = JsonUtility.ToJson(nodeData, true);

        using (FileStream fs = new FileStream(nodePath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(fs))
                writer.Write(json);
        
        UnityEditor.AssetDatabase.Refresh();
    }
    
    #region GUI

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        DrawAxes(0.6f, Color.red);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        DrawButtons();

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);
        
        if (GUI.changed) Repaint();
    }

    // Draw our new buttons for managing the skill tree
    private void DrawButtons()
    {
        if (GUI.Button(rectButtonClear, "Clear"))
            ClearNodes();
        if (GUI.Button(rectButtonSave, "Save"))
        {
            SaveTechTree();
            SaveNodes();
        }
        if (GUI.Button(rectButtonLoad, "Load"))
            LoadNodes();
    }

    private void DrawAxes(float axisOpacity, Color axisColor)
    {
        Handles.BeginGUI();
        Handles.color = new Color(axisColor.r, axisColor.g, axisColor.b, axisOpacity);
        
        Handles.DrawLine(new Vector3(offset.x, -position.height, 0), new Vector3(offset.x, position.height, 0f));

        Handles.DrawLine(new Vector3(-position.width, offset.y, 0), new Vector3(position.width, offset.y, 0f));
        
        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void ProcessEvents(Event e)
    {

        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }

                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }
    
    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Drag(delta);
        
        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (nodes == null)
            nodes = new List<Node>();
        
        JSON_Technology tech = new JSON_Technology("", id, null, 0, false, null);

        // We create the node with the default info for the node.
        Node n = new Node(mousePosition, nodeWidth, nodeHeight, nodeStyle, selectedNodeStyle,
            inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode,
            tech);
        nodes.Add(n);
        n.StopDrag();
        
        id++;
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
                ClearConnectionSelection();
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
                ClearConnectionSelection();
        }
    }

    private void OnClickRemoveNode(Node node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                    connectionsToRemove.Add(connections[i]);

            for (int i = 0; i < connectionsToRemove.Count; i++)
                connections.Remove(connectionsToRemove[i]);

            connectionsToRemove = null;
        }

        nodes.Remove(node);
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
            connections = new List<Connection>();

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    #endregion
}