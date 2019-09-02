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

    // Two Rect for the name field (1 for the label and other for the checkbox)
    public Rect rectNameLabel;
    public Rect rectName;

    // Two Rect for the icon path field (1 for the label and other for the checkbox)
    public Rect rectIconLabel;
    public Rect rectIcon;

    // Two Rect for the start tech field (1 for the label and other for the checkbox)
    public Rect rectUnlockLabel;
    public Rect rectUnlocked;

    // Two Rect for the cost field (1 for the label and other for the text field)
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
    public Technology tech;

    // Bool for checking if the node is whether unlocked or not
    private bool startTech = false;

    // StringBuilder to create the node's title
    private StringBuilder nodeTitle;

    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle,
        GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint,
        Action<Node> OnClickRemoveNode, Technology technology)
    {
        position += NodeBasedEditor.instance.Offset;

        Vector2 divPos = position / NodeBasedEditor.GRID_SNAP;
        Vector2 roundedPos = new Vector2(Mathf.Round(divPos.x), Mathf.Round(divPos.y));

        position = roundedPos * NodeBasedEditor.GRID_SNAP;
        position -= NodeBasedEditor.instance.Offset;

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
        float wf = 1 / 2.5f;

        rectName = new Rect(position.x + width * wf,
            position.y + 3 * rowHeight, width * (1 - wf - 0.1f), rowHeight);

        rectNameLabel = new Rect(position.x,
            position.y + 3 * rowHeight, width * wf, rowHeight);

        rectUnlocked = new Rect(position.x + width * wf,
            position.y + 4 * rowHeight, width * wf, rowHeight);

        rectUnlockLabel = new Rect(position.x,
            position.y + 4 * rowHeight, width * wf, rowHeight);

        styleField = new GUIStyle();
        styleField.alignment = TextAnchor.UpperRight;

        rectCostLabel = new Rect(position.x,
            position.y + 5 * rowHeight, width * wf, rowHeight);

        rectCost = new Rect(position.x + width * wf,
            position.y + 5 * rowHeight, width * wf / 2, rowHeight);
        /*
        rectIconLabel = new Rect(position.x,
            position.y + 6 * rowHeight, width * wf, rowHeight);

        rectIcon = new Rect(position.x + width * wf,
            position.y + 6 * rowHeight, width * (1 - wf - 0.2f), rowHeight);
        */
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
        rectUnlocked.position += delta;
        rectUnlockLabel.position += delta;
        rectCost.position += delta;
        rectCostLabel.position += delta;
        rectIcon.position += delta;
        rectIconLabel.position += delta;
    }
    
    public void StopDrag()
    {
        Vector2 divPos = (rect.position - NodeBasedEditor.instance.Offset) / NodeBasedEditor.GRID_SNAP;
        Vector2 roundedPos = new Vector2(Mathf.Round(divPos.x), Mathf.Round(divPos.y)) * NodeBasedEditor.GRID_SNAP;
        roundedPos += NodeBasedEditor.instance.Offset;

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
        GUI.Label(rectUnlockLabel, "Start Tech: ", styleField);
        if (GUI.Toggle(rectUnlocked, startTech, ""))
            startTech = true;
        else
            startTech = false;

        tech.startTech = startTech;

        // Print the cost field
        GUI.Label(rectCostLabel, "Cost: ", styleField);
        tech.cost = int.Parse(GUI.TextField(rectCost, tech.cost.ToString()));
        /*
        // Print the icon path field
        GUI.Label(rectIconLabel, "Icon: ", styleField);
        try
        {
            tech.icon = GUI.TextField(rectIcon, tech.icon.ToString());
        }
        catch
        {
            tech.icon = GUI.TextField(rectIcon, "");
            Debug.LogError("Error while reading this tech's icon. Please check " + tech.id);
        }
        */
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