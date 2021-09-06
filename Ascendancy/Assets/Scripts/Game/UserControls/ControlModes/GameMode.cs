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
    public SubscribableList<EntitySelector> selectedEntities { get; protected set; }

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

    //private Vector3[] conMenuButtonPos;

    public GameMode() : base()
    {
        selectedEntities = new SubscribableList<EntitySelector>();
        cam = gameManager.camScript.transform.GetComponent<Camera>();
        selectionBox = GameObject.Find("SelectionRect").GetComponent<Image>();
        selectionBox.enabled = false;

        formationLine = GameObject.Find("FormationLine").GetComponent<LineRenderer>();

        if (selectionBox == null)
            Debug.LogError("SelectionRect not found");

        if (formationLine == null)
            Debug.LogError("FormationLine not found");
        else
            formationLine.enabled = false;
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
        bool append = Input.GetKey(KeyCode.LeftShift);
        
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

                rectPos = new Vector2(Mathf.Min(dragStartPosM1.x, dragStopPosM1.x), Mathf.Max(dragStartPosM1.y, dragStopPosM1.y));
                rectSize = new Vector2(Mathf.Abs(dragStartPosM1.x - dragStopPosM1.x), Mathf.Abs(dragStartPosM1.y - dragStopPosM1.y));
                /*
                if (rectSize.x < 0)
                {
                    rectPos.x = dragStopPosM1.x;
                    rectSize.x *= -1;
                }
                if (rectSize.y < 0)
                    rectSize.y *= -1;
                else
                    rectPos.y = dragStopPosM1.y;
                */
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
                    throw new System.Exception("Invalid Player Number (" + gameManager.playerNumber + ")");

                if (!append)
                    DeselectAll();

                int highestPriority = -1;
                foreach (EntitySelector e in player.GetComponentsInChildren<EntitySelector>())
                    if (PositionInSelection(e.transform.position))
                    {
                        if (!selectedEntities.Contains(e))
                            selectedEntities.Add(e);

                        Entity entity = e.GetComponentInParent<Entity>();

                        if (entity.entityInfo.selectionPriority > highestPriority)
                            highestPriority = entity.entityInfo.selectionPriority;
                    }

                // Keep only those Entities with the highest Priority
                List<EntitySelector> filteredList = selectedEntities.AsList.Where(e => e.GetComponentInParent<Entity>().entityInfo.selectionPriority == highestPriority).ToList();
                foreach (EntitySelector e in filteredList)
                    e.Selected = true;
                selectedEntities.FromList(filteredList);
            }
            else if (!EventSystem.current.IsPointerOverGameObject())
            {
                // only one click, so raycast -> see if we hit a unit
                // only if we didnt click in the UI tho

                Ray ray = gameManager.camScript.MouseCursorRay();
                RaycastHit hit;

                if (!append)
                    DeselectAll();

                int layerMask = 1 << LayerMask.NameToLayer("Entities");
                if (Physics.Raycast(ray, out hit, 100, layerMask))
                {
                    if((hit.collider.transform.parent.tag == "Unit" || hit.collider.transform.parent.tag == "Building"))
                    {
                        Entity e = hit.transform.GetComponentInParent<Entity>();
                        EntitySelector es = e.GetComponentInChildren<EntitySelector>();

                        if (e.Owner.playerID == gameManager.playerNumber)
                        {
                            es.Selected = true;
                            selectedEntities.Add(es);
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
        if (selectedEntities.Count > 0)
            Mouse2_UnitOrder();
        else
            Mouse2_NoUnitsSelected();
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

                // right now the formations are Unit-exclusive
                // TODO: handle Buildings
                List<Entity> selectedUnits = new List<Entity>();

                foreach (EntitySelector e in selectedEntities.AsList)
                    selectedUnits.Add(e.ParentEntity);

                Formation formation = new FormationSquare(dragStartPosM2, dragStopPosM2, 1);
                //Formation formation = new FormationLine(dragStartPosM2, dragStopPosM2);
                formation.AssignPositions(selectedUnits);

                foreach (Entity u in selectedUnits)
                {
                    // Issue an order to the nearest unit to move there
                    bool enqueue = Input.GetKey(KeyCode.LeftShift);
                    u.IssueOrder(new MoveOrder(u, formation.assignedPositions[u]), enqueue);
                    u.IssueOrder(new RotateOrder(u, formation.assignedOrientations[u]), true);
                }
            }

            else
            {
                // probably a redundant raycast, can be optimized
                RaycastHit hit = MouseRaycast();

                if (hit.collider != null)
                    foreach (EntitySelector es in selectedEntities.AsList)
                    {
                        // Entity might have been destroyed, so check if it still exists
                        if (es == null)
                            continue;

                        bool enqueue = Input.GetKey(KeyCode.LeftShift);
                        bool ctrl = Input.GetKey(KeyCode.LeftControl);
                        es.ParentEntity.ClickOrder(hit, enqueue, ctrl);
                        //u.GetComponentInParent<Entity>().ClickOrder(hit, enqueue);
                    }
                else
                    Debug.LogError("Raycast missed hit.collider");
            }
        }
    }

    private void Mouse2_NoUnitsSelected()
    {
        // toggle build menu
        if (Input.GetMouseButtonUp(1))
        {
            string buildMenuName = "Build Menu";
            if (!gameManager.Ui_Manager.GetScreenStatus(buildMenuName))
                gameManager.Ui_Manager.OpenScreen(buildMenuName);
            else
                gameManager.Ui_Manager.SetScreen(buildMenuName, false);
        }
    }

    /// <summary>
    /// Bring up ContextMenu
    /// </summary>
    private void Mouse3()
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
                    Debug.LogError("Entity Selector is NULL! (FAIL)Name is " + hit.transform.name);
                }
                else if (e.GetComponentInParent<Entity>().GetType() == typeof(Unit))
                {
                    EntityInfo uInfo = e.GetComponentInParent<Unit>().entityInfo;
                }
                else if (e.GetComponentInParent<Entity>().GetType() == typeof(Building))
                {
                    BuildingInfo bInfo = e.GetComponentInParent<Building>().buildingInfo;
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
        return unit.Owner.playerID != gameManager.playerNumber;
    }

    private void DeselectAll()
    {
        foreach (EntitySelector unitSelector in selectedEntities.AsList)
            unitSelector.Selected = false;

        selectedEntities.Clear();
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
