using System;
using System.Text;
using UnityEditor;
using UnityEngine;

public class Node
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    // Rect for the title of the node 
    public Rect rectID;

    // Two Rect for the name field (1 for the label and other for the text field)
    public Rect rectNameLabel;
    public Rect rectName;

    // Two Rect for the icon path field
    public Rect rectIconLabel;
    public Rect rectIcon;

    // Two Rect for the Technology ScriptableObject field
    public Rect rectSOLabel;
    public Rect rectSO;

    // Two Rect for the start tech field
    public Rect rectStartTechLabel;
    public Rect rectStartTech;

    // Two Rect for the cost field
    public Rect rectCostLabel;
    public Rect rectCost;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    // GUI Style for the title
    public GUIStyle styleID;

    // GUI Style for the fields
    public GUIStyle styleField;

    public Action<Node> OnRemoveNode;

    // Skill linked with the node
    public JSON_Technology tech;

    // Bool for checking if the node is whether unlocked or not
    private bool startTech = false;

    // StringBuilder to create the node's title
    private StringBuilder nodeTitle;

    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle,
        GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint,
        Action<Node> OnClickRemoveNode, JSON_Technology technology)
    {
        position += TechTreeEditor.instance.Offset;

        Vector2 divPos = position / TechTreeEditor.GRID_SNAP;
        Vector2 roundedPos = new Vector2(Mathf.Round(divPos.x), Mathf.Round(divPos.y));

        position = roundedPos * TechTreeEditor.GRID_SNAP;
        position -= TechTreeEditor.instance.Offset;

        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        OnRemoveNode = OnClickRemoveNode;

        // Create new Rect and GUIStyle for our title and custom fields
        float rowHeight = height / 8;

        rectID = new Rect(position.x, position.y + rowHeight, width, rowHeight);
        styleID = new GUIStyle();
        styleID.alignment = TextAnchor.UpperCenter;

        // width factor
        float wf = 1 / 3f;
        float mid = position.x + width * wf - 2;

        rectName = new Rect(mid,
            position.y + 3 * rowHeight, width * (1 - wf - 0.1f), rowHeight);

        rectNameLabel = new Rect(position.x,
            position.y + 3 * rowHeight, width * wf, rowHeight);

        rectCost = new Rect(mid,
            position.y + 4 * rowHeight, width * wf / 2 + 20, rowHeight);

        rectCostLabel = new Rect(position.x,
            position.y + 4 * rowHeight, width * wf, rowHeight);

        rectStartTech = new Rect(mid + 30,
            position.y + 5 * rowHeight, 20, rowHeight);

        rectStartTechLabel = new Rect(position.x + 30,
            position.y + 5 * rowHeight, width * wf, rowHeight);

        styleField = new GUIStyle();
        styleField.alignment = TextAnchor.UpperRight;
        
        this.startTech = technology.startTech;

        //// We create the skill with current node info
        tech = technology;

        // Create string with ID info
        nodeTitle = new StringBuilder();
        nodeTitle.Append("ID: ");
        nodeTitle.Append(technology.id);
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;

        rectID.position += delta;
        rectName.position += delta;
        rectNameLabel.position += delta;
        rectStartTech.position += delta;
        rectStartTechLabel.position += delta;
        rectCost.position += delta;
        rectCostLabel.position += delta;
        rectIcon.position += delta;
        rectIconLabel.position += delta;
        rectSO.position += delta;
        rectSOLabel.position += delta;
    }
    
    public void StopDrag()
    {
        Vector2 divPos = (rect.position - TechTreeEditor.instance.Offset) / TechTreeEditor.GRID_SNAP;
        Vector2 roundedPos = new Vector2(Mathf.Round(divPos.x), Mathf.Round(divPos.y)) * TechTreeEditor.GRID_SNAP;
        roundedPos += TechTreeEditor.instance.Offset;

        Vector2 delta = roundedPos - rect.position;

        Drag(delta);
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, style);

        // Print the title
        GUI.Label(rectID, nodeTitle.ToString(), styleID);
        
        // Print the name field
        GUI.Label(rectNameLabel, "Name: ", styleField);
        try
        {
            tech.name = GUI.TextField(rectName, tech.name.ToString());
        }
        catch
        {
            tech.name = GUI.TextField(rectName, "");
            Debug.LogError("Error while reading this tech's name. Please check " + tech.id);
        }

        // Print the unlock field
        GUI.Label(rectStartTechLabel, "StartTech: ", styleField);
        if (GUI.Toggle(rectStartTech, startTech, ""))
            startTech = true;
        else
            startTech = false;

        tech.startTech = startTech;

        // Print the cost field
        GUI.Label(rectCostLabel, "Cost: ", styleField);
        tech.cost = int.Parse(GUI.TextField(rectCost, tech.cost.ToString()));
        
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;

                        TechTreeEditor.instance.selectedNode.Value = this;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (isDragged)
                    StopDrag();
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
            OnRemoveNode(this);
        
    }
}