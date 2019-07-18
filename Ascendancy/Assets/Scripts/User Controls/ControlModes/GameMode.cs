using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            conMenuHandler.Hide();
    }

    public override void HandleInput()
    {
        Mouse1();
        Mouse2();
        Mouse3();
    }

    private void Mouse1()
    {
        if (!conMenuHandler.IsVisible())
        {
            if (Input.GetMouseButtonDown(0))
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

                    foreach (EntitySelector e in player.GetComponentsInChildren<EntitySelector>())
                        if (PositionInSelection(e.transform.position))
                        {
                            selectedUnits.Add(e);
                            e.Selected = true;
                        }
                }
                else
                {
                    // only one click, so raycast -> see if we hit a unit

                    Ray ray = gameManager.camScript.MouseCursorRay();
                    RaycastHit hit;

                    DeselectAll();

                    if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Unit" || hit.collider.tag == "Building"))
                    {
                        EntitySelector e = hit.transform.GetComponent<EntitySelector>();

                        if (e.GetComponentInParent<Entity>().Owner.playerNo == gameManager.playerNo)
                        {
                            e.Selected = true;
                            selectedUnits.Add(e);
                        }
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
                    Vector3 orientation = Vector3.Cross(dragStopPosM2 - dragStartPosM2, Vector3.up).normalized;

                    // Create a copy of the selectedUnits List, so we can later sort the units for shortest paths
                    EntitySelector[] unitsArray = new EntitySelector[selectedUnits.Count];
                    selectedUnits.CopyTo(unitsArray);

                    List<Unit> units = new List<Unit>();
                    foreach (EntitySelector unitSelector in unitsArray)
                        units.Add(unitSelector.GetComponentInParent<Unit>());

                    for (int i = 0; i < count; i++)
                    {
                        float lerpFactor;
                        if (count > 1)
                            lerpFactor = (float)i / (count - 1);
                        else
                            lerpFactor = 0.5f;
                        Vector3 lerpedPos = Vector3.Lerp(dragStartPosM2, dragStopPosM2, lerpFactor);

                        // Find the nearest Unit per position, and issue orders accordingly.
                        // This is a greedy process, so might not be optimal, but should still be good in most cases.
                        Unit nearestUnit = null;
                        float nearestDistance = Mathf.Infinity;

                        foreach (Unit unit in units)
                        {
                            float distance = Vector3.Distance(unit.transform.position, lerpedPos);
                            if (distance < nearestDistance)
                            {
                                nearestDistance = distance;
                                nearestUnit = unit;
                            }
                        }

                        units.Remove(nearestUnit);

                        bool enqueue = Input.GetKey(KeyCode.LeftShift);
                        nearestUnit.IssueOrder(new MoveOrder(nearestUnit, lerpedPos), false);
                        nearestUnit.IssueOrder(new RotateOrder(nearestUnit, orientation), true);
                    }
                }
                else
                {
                    // probably a redundant raycast, can be optimized
                    RaycastHit hit = MouseRaycast();
                    foreach (EntitySelector u in selectedUnits)
                    {
                        bool enqueue = Input.GetKey(KeyCode.LeftShift);
                        u.GetComponentInParent<Unit>().ClickOrder(hit, enqueue);
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
                    DeselectAll();

                    int thismanybuttons = 0;    //zero button as default 

                    if (e.GetComponentInParent<Unit>() != null)
                    {
                        UnitInfo uInfo = e.GetComponentInParent<Unit>().unitInfo;
                        thismanybuttons = uInfo.contextMenuOptions;
                    }
                    else
                    if (e.GetComponentInParent<Building>() != null)
                    {
                        Debug.Log("Building clicked: We need BuildingInfo!");
                    }
                    else
                        Debug.LogError("No Unit found in Object" + e.name);

                    if (thismanybuttons > 0 && thismanybuttons < 9)    //open menu only if options are available
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
