using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This should be the standard mode for when the game is running.
/// </summary>
public class GameMode : ControlMode
{
    private List<EntitySelector> selectedUnits;

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
        selectedUnits = new List<EntitySelector>(8);
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
        {
            Debug.LogError("ConMenuHandler not found");
        }
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
                GameObject player = GameObject.Find("Player " + gameManager.playerNo);
                if (player == null)
                    throw new System.Exception("Invalid Player Number (" + gameManager.playerNo + ")");

                DeselectAll();

                bool unitsInSelection = false;
                foreach (EntitySelector e in player.GetComponentsInChildren<EntitySelector>())
                    if (PositionInSelection(e.transform.position))
                    {
                        selectedUnits.Add(e);
                        e.Selected = true;
                        if (e.ParentEntity is Unit)
                            unitsInSelection = true;
                    }

                // if there is at least one unit in the selection, do not select buildings
                List<EntitySelector> toRemove = new List<EntitySelector>();
                if (unitsInSelection)
                {
                    foreach (EntitySelector e in selectedUnits)
                        if (e.ParentEntity is Building)
                            toRemove.Add(e);
                    foreach (EntitySelector e in toRemove)
                    {
                        selectedUnits.Remove(e);
                        e.Selected = false;
                    }
                }

            }
            else
            {
                // only one click, so raycast -> see if we hit a unit

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

                    // sort units, then issue commands according to units' relative position towards the goal line
                    foreach (EntitySelector es in selectedUnits)
                    {
                        Unit u = es.ParentEntity as Unit;

                        // Project the Unit's position onto the drag line
                        Vector3 startToUnitPos = u.transform.position - dragStartPosM2;
                        Vector3 projectedVector = Vector3.Project(startToUnitPos, dragLineDirection);
                        float projectedDistance = projectedVector.magnitude;

                        // if the units projected position is in the "before" the drag line starting pos, correct the projected distance
                        if (Vector3.Angle(projectedVector, dragLineDirection) > 90)
                            projectedDistance *= -1;
                        
                        // sort by length of the projected vector
                        unitsSorted.Add(projectedDistance, u);
                    }

                    // Make sure nothing has gone horribly wrong
                    Debug.Assert(unitsSorted.Count == count);

                    int i = 0;
                    foreach (KeyValuePair<float, Unit> kvp in unitsSorted)
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
                        Unit nearestUnit = kvp.Value;
                        bool enqueue = Input.GetKey(KeyCode.LeftShift);
                        nearestUnit.IssueOrder(new MoveOrder(nearestUnit, lerpedPos), false);
                        nearestUnit.IssueOrder(new RotateOrder(nearestUnit, orientation), true);
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
                            //some errors here, I think we should split building and unit selection:
                            //1+ unit among entities: only units selected
                            //TODO seperate selections

                            bool enqueue = Input.GetKey(KeyCode.LeftShift);
                            u.GetComponentInParent<Entity>().ClickOrder(hit, enqueue);


                            //if (u.GetComponentInParent<Entity>().GetType() == typeof(Unit))
                            //{
                            //    bool enqueue = Input.GetKey(KeyCode.LeftShift);
                            //    u.GetComponentInParent<Unit>().ClickOrder(hit, enqueue);
                            //}
                            //else
                            //{
                            //    Debug.Log("Building " + u.GetComponentInParent<Entity>().name + " could not receive order!");
                            //}
                        }
                    }
                    else
                    {
                        Debug.LogError("Raycast missed hit.collider");
                    }
                }
            }
        }
        else //if context menu is open
        {
            if (Input.GetMouseButtonUp(1))
            {
                conMenuHandler.Hide();
            }
        }
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
                        UnitInfo uInfo = e.GetComponentInParent<Unit>().unitInfo;
                        thismanybuttons = uInfo.contextMenuOptions;
                    }
                    else if (e.GetComponentInParent<Entity>().GetType() == typeof(Building))
                    {
                        BuildingInfo bInfo = e.GetComponentInParent<Building>().buildingInfo;
                        thismanybuttons = bInfo.contextMenuOptions;
                    }

                    if (thismanybuttons > 0 && thismanybuttons < 9)
                        //open menu only if options are available  
                        if (e.GetComponentInParent<Entity>().Owner.playerNo == gameManager.playerNo)
                        {
                            //here we open the context menu    
                            conMenuHandler.Show(thismanybuttons);
                        }
                }
            }
        }
    }

    private RaycastHit MouseRaycast()
    {
        Ray ray = gameManager.camScript.MouseCursorRay();
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        return hit;
    }

    private bool IsHostileUnit(Unit unit)
    {
        return unit.Owner.playerNo != gameManager.playerNo;
    }

    private void DeselectAll()
    {
        foreach (EntitySelector unitSelector in selectedUnits)
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
