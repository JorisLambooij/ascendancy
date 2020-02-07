using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This should be the standard mode for when the game is running.
/// </summary>
public class GameMode : ControlMode
{
    public SubscribableList<EntitySelector> selectedUnits { get; protected set; }

    private Vector3 dragStartPosM1, dragStopPosM1;
    private bool startDragM1, draggingM1;
    private Vector3 rectPos;
    private Vector3 rectSize;

    private bool startDragM2, draggingM2;
    private Vector3 dragStartPosM2, dragStopPosM2;
    private Vector3 lineOffset = new Vector3(0, 0.2f, 0);

    private LineRenderer formationLine;
    private Image selectionBox;
    private Camera cam;
    //private Canvas contextMenuCanvas;
    private ContextMenuHandler conMenuHandler;
    
    //private Vector3[] conMenuButtonPos;

    public GameMode() : base()
    {
        selectedUnits = new SubscribableList<EntitySelector>();
        cam = gameManager.camScript.transform.GetComponent<Camera>();
        selectionBox = GameObject.Find("SelectionRect").GetComponent<Image>();
        selectionBox.enabled = false;

        //contextMenuCanvas = GameObject.Find("Canvas_ConMenu").GetComponent<Canvas>();
        conMenuHandler = GameObject.Find("Canvas_ConMenu").GetComponent<ContextMenuHandler>();

        formationLine = GameObject.Find("FormationLine").GetComponent<LineRenderer>();

        if (selectionBox == null)
            Debug.LogError("SelectionRect not found");

        if (formationLine == null)
            Debug.LogError("FormationLine not found");
        else
            formationLine.enabled = false;
        if (conMenuHandler == null)
            Debug.LogError("ConMenuHandler not found");
        else
        {
            //conMenuHandler.Hide();
        }
    }

    public override void HandleInput()
    {
        Mouse1();
        Mouse2();
        Mouse3();


    }

    public override void Start()
    {
        DeselectAll();

        startDragM1 = false;
        startDragM2 = false;
        draggingM1 = false;
        draggingM2 = false;
    }

    public override void Stop()
    {
        DeselectAll();
    }

    private void Mouse1()
    {
        // context menu out or pointer over UI element;
        if (conMenuHandler.IsVisible())
            return;
        
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragStartPosM1 = Input.mousePosition;
            startDragM1 = true;
        }
        if (startDragM1)
        {
            if (!draggingM1 && Vector3.Distance(dragStartPosM1, Input.mousePosition) > 4)
            {
                draggingM1 = true;
                selectionBox.enabled = true;
            }

            if (draggingM1)
            {
                dragStopPosM1 = Input.mousePosition;
                rectPos = dragStartPosM1;
                rectSize = dragStopPosM1 - dragStartPosM1;
                if (rectSize.x < 0)
                {
                    rectPos.x = dragStopPosM1.x;
                    rectSize.x *= -1;
                }
                if (rectSize.y < 0)
                    rectSize.y *= -1;
                else
                    rectPos.y = dragStopPosM1.y;

                selectionBox.rectTransform.position = rectPos;
                selectionBox.rectTransform.sizeDelta = new Vector2(rectSize.x, rectSize.y);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            startDragM1 = false;
            selectionBox.enabled = false;
            if (draggingM1)
            {
                // dragging selection, so check all units if in area
                draggingM1 = false;

                Matrix4x4 selectionMatrix = selectionBox.rectTransform.worldToLocalMatrix;
                Player player = gameManager.playerScript;
                if (player == null)
                    throw new System.Exception("Invalid Player Number (" + gameManager.playerNo + ")");

                DeselectAll();

                int highestPriority = -1;
                foreach (EntitySelector e in player.GetComponentsInChildren<EntitySelector>())
                    if (PositionInSelection(e.transform.position))
                    {
                        selectedUnits.Add(e);

                        Entity entity = e.GetComponentInParent<Entity>();

                        if (entity.entityInfo.selectionPriority > highestPriority)
                            highestPriority = entity.entityInfo.selectionPriority;
                    }

                // Keep only those Entities with the highest Priority
                List<EntitySelector> filteredList = selectedUnits.AsList.Where(e => e.GetComponentInParent<Entity>().entityInfo.selectionPriority == highestPriority).ToList();
                foreach(EntitySelector e in filteredList)
                    e.Selected = true;
                selectedUnits.FromList(filteredList);
            }
            else if (!EventSystem.current.IsPointerOverGameObject())
            {
                // only one click, so raycast -> see if we hit a unit
                // only if we didnt click in the UI tho

                Ray ray = gameManager.camScript.MouseCursorRay();
                RaycastHit hit;

                DeselectAll();

                int layerMask = 1 << LayerMask.NameToLayer("Entities");
                if (Physics.Raycast(ray, out hit, 100, layerMask) && (hit.collider.tag == "Unit" || hit.collider.tag == "Building"))
                {
                    Entity e = hit.transform.GetComponentInParent<Entity>();
                    EntitySelector es = e.GetComponentInChildren<EntitySelector>();

                    if (e.Owner.playerNo == gameManager.playerNo)
                    {
                        es.Selected = true;
                        selectedUnits.Add(es);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Give Orders to Units
    /// </summary>
    private void Mouse2()
    {
        if (!conMenuHandler.IsVisible())
        {
            if (selectedUnits.Count > 0)
                Mouse2_UnitOrder();
            else
                Mouse2_NoUnitsSelected();
            
        }
        else //if context menu is open
        {
            if (Input.GetMouseButtonUp(1))
            {
                conMenuHandler.Hide();
            }
        }
    }

    private void Mouse2_UnitOrder()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragStartPosM2 = MouseRaycast().point;
            formationLine.SetPosition(0, dragStartPosM2 + lineOffset);
            startDragM2 = true;
        }
        if (startDragM2)
        {
            dragStopPosM2 = MouseRaycast().point;
            if (!draggingM2 && Vector3.Distance(dragStartPosM2, dragStopPosM2) > 0.01f)
            {
                    draggingM2 = true;
                    formationLine.enabled = true;
                }

            if (draggingM2)
            {
                formationLine.SetPosition(1, dragStopPosM2 + lineOffset);
                Debug.DrawLine(dragStartPosM2, dragStopPosM2);
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            startDragM2 = false;
            formationLine.enabled = false;

            if (draggingM2)
            {
                draggingM2 = false;

                int count = selectedUnits.Count;
                Vector3 dragLineDirection = (dragStopPosM2 - dragStartPosM2);
                Vector3 orientation = Vector3.Cross(dragLineDirection, Vector3.up).normalized;

                SortedDictionary<float, Unit> unitsSorted = new SortedDictionary<float, Unit>();

                    SortedDictionary<float, Entity> unitsSorted = new SortedDictionary<float, Entity>();

                    // sort units, then issue commands according to units' relative position towards the goal line
                    foreach (EntitySelector es in selectedUnits.AsList)
                    {
                        Entity e = es.ParentEntity as Entity;

                        if (e == null)
                            continue;

                        // Project the Unit's position onto the drag line
                        Vector3 startToUnitPos = e.transform.position - dragStartPosM2;
                        Vector3 projectedVector = Vector3.Project(startToUnitPos, dragLineDirection);
                        float projectedDistance = projectedVector.magnitude;

                    // if, by some chance, two units happen to have the same projected dictance, just move the second one slightly further down.
                    while (unitsSorted.ContainsKey(projectedDistance))
                        projectedDistance += 0.0001f;
                    // sort by length of the projected vector
                    unitsSorted.Add(projectedDistance, u);
                }

                        // if, by some chance, two units happen to have the same projected dictance, just move the second one slightly further down.
                        while (unitsSorted.ContainsKey(projectedDistance))
                            projectedDistance += 0.001f;
                        // sort by length of the projected vector
                        unitsSorted.Add(projectedDistance, e);
                    }

                    // Make sure nothing has gone horribly wrong (no units missing or counted twice)
                    Debug.Assert(unitsSorted.Count == count);
                    if (unitsSorted.Count != count)
                        Debug.Log("UnitsSorted.Count: " + unitsSorted.Count + "; SelectedUnits.Count: " + count);

                    int i = 0;
                    foreach (KeyValuePair<float, Entity> kvp in unitsSorted)
                    {
                        // Determine the lerped position on the drag line
                        float lerpFactor;
                        if (count > 1)
                            lerpFactor = (float)i / (count - 1);
                        else
                            lerpFactor = 0.5f;
                        i++;
                        Vector3 lerpedPos = Vector3.Lerp(dragStartPosM2, dragStopPosM2, lerpFactor);

                        // Issue an order to the nearest unit to move there
                        Entity nearestUnit = kvp.Value;
                        bool enqueue = Input.GetKey(KeyCode.LeftShift);
                        nearestUnit.IssueOrder(new MoveOrder(nearestUnit, lerpedPos), false);
                        nearestUnit.IssueOrder(new RotateOrder(nearestUnit, orientation), true);
                    }
                }
            }
            else
            {
                // probably a redundant raycast, can be optimized
                RaycastHit hit = MouseRaycast();

                if (hit.collider != null)
                {
                    foreach (EntitySelector u in selectedUnits)
                    {
                        foreach (EntitySelector u in selectedUnits.AsList)
                        {
                            bool enqueue = Input.GetKey(KeyCode.LeftShift);
                            u.ParentEntity.ClickOrder(hit, enqueue);
                            Debug.Log("click " + u.ParentEntity.gameObject.name);
                            //u.GetComponentInParent<Entity>().ClickOrder(hit, enqueue);
                        }
                    }
                    else
                        Debug.LogError("Raycast missed hit.collider");
                }
                else
                {
                    Debug.LogError("Raycast missed hit.collider");
                }
            }
        }
        else //if context menu is open
        {
            if (Input.GetMouseButtonUp(1))
                conMenuHandler.Hide();
        }
    }

    private void Mouse2_NoUnitsSelected()
    {
        // toggle build menu
        if (Input.GetMouseButtonUp(1))
            gameManager.UICanvas.ToggleBuildMenu();
    }

    /// <summary>
    /// Bring up ContextMenu
    /// </summary>
    private void Mouse3()
    {
        if (!conMenuHandler.IsVisible())
        {
            if (Input.GetMouseButtonUp(2))
            {
                Ray ray = gameManager.camScript.MouseCursorRay();
                RaycastHit hit;

                DeselectAll();

                //we only open the context menu on valid targets:
                if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Unit" || hit.collider.tag == "Building"))
                {
                    EntitySelector e = hit.transform.GetComponent<EntitySelector>();
                    DeselectAll(); int thismanybuttons = 0;
                    //zero button as default                                     
                    if (e == null)
                    {
                        Debug.LogError("Entity Selector is NULL!");
                        Debug.LogError("(FAIL)Name is " + hit.transform.name);
                    }
                    else if (e.GetComponentInParent<Entity>().GetType() == typeof(Unit))
                    {
                        EntityInfo uInfo = e.GetComponentInParent<Unit>().entityInfo;
                        thismanybuttons = uInfo.ContextMenuOptions;
                    }
                    else if (e.GetComponentInParent<Entity>().GetType() == typeof(Building))
                    {
                        BuildingInfo bInfo = e.GetComponentInParent<Building>().buildingInfo;
                        thismanybuttons = bInfo.ContextMenuOptions;
                    }

                    if (thismanybuttons > 0 && thismanybuttons < 9)
                        //open menu only if options are available  
                        if (e.GetComponentInParent<Entity>().Owner.playerNo == gameManager.playerNo)
                            //here we open the context menu    
                            conMenuHandler.Show(thismanybuttons);
                }
            }
        }
    }

    private RaycastHit MouseRaycast()
    {
        Ray ray = gameManager.camScript.MouseCursorRay();
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Entities") | 1 << LayerMask.NameToLayer("Ground");
        Physics.Raycast(ray, out hit, 100, layerMask);
        return hit;
    }

    private bool IsHostileUnit(Unit unit)
    {
        return unit.Owner.playerNo != gameManager.playerNo;
    }

    private void DeselectAll()
    {
        foreach (EntitySelector unitSelector in selectedUnits.AsList)
            unitSelector.Selected = false;

        selectedUnits.Clear();
    }

    /// <summary>
    /// Checks whether position is within the Selection Rectangle (Screen Space).
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True or false.</returns>
    private bool PositionInSelection(Vector3 position)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(cam, position);

        if (screenPos.x > rectPos.x && screenPos.x < rectPos.x + rectSize.x
            && screenPos.y < rectPos.y && screenPos.y > rectPos.y - rectSize.y)
            return true;

        return false;

    }
}
