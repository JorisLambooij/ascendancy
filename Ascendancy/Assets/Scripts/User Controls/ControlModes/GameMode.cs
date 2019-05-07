using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMode : ControlMode
{
    private List<UnitSelector> selectedUnits;
    
    private Vector3 startScreenPos;
    private Vector3 stopScreenPos;
    private Vector3 rectPos;
    private Vector3 rectSize;
    private bool startDrag;
    private bool dragging;

    private Image selectionBox;
    private Camera cam;

    public GameMode() : base()
    {
        selectedUnits = new List<UnitSelector>(8);
        cam = gameManager.camScript.transform.GetComponent<Camera>();
        selectionBox = GameObject.Find("SelectionRect").GetComponent<Image>();
        selectionBox.enabled = false;

        if (selectionBox == null)
            Debug.LogError("SelectionRect not found");

    }

    public override void HandleInput()
    {
        Mouse1();
        Mouse2();
    }

    private void Mouse1()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startScreenPos = Input.mousePosition;
            startDrag = true;
        }
        if (startDrag)
        {
            if (!dragging && Vector3.Distance(startScreenPos, Input.mousePosition) > 4)
            {
                dragging = true;
                selectionBox.enabled = true;
            }

            if (dragging)
            {
                stopScreenPos = Input.mousePosition;
                rectPos = startScreenPos;
                rectSize = stopScreenPos - startScreenPos;
                if (rectSize.x < 0)
                {
                    rectPos.x = stopScreenPos.x;
                    rectSize.x *= -1;
                }
                if (rectSize.y < 0)
                    rectSize.y *= -1;
                else
                    rectPos.y = stopScreenPos.y;

                selectionBox.rectTransform.position = rectPos;
                selectionBox.rectTransform.sizeDelta = new Vector2(rectSize.x, rectSize.y);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (dragging)
            {
                // dragging selection, so check all units if in area
                selectionBox.enabled = false;
                startDrag = false;
                dragging = false;

                Matrix4x4 selectionMatrix = selectionBox.rectTransform.worldToLocalMatrix;
                GameObject player = GameObject.Find("Player " + gameManager.playerNo);
                if (player == null)
                    throw new System.Exception("Invalid Player Number (" + gameManager.playerNo + ")");

                DeselectAll();

                foreach (UnitSelector u in player.GetComponentsInChildren<UnitSelector>())
                    if (PositionInSelection(u.transform.position))
                    {
                        selectedUnits.Add(u);
                        u.Selected = true;
                    }
            }
            else
            {
                // only one click, so raycast -> see if we hit a unit
                startDrag = false;
                selectionBox.enabled = false;

                Ray ray = gameManager.camScript.MouseCursorRay();
                RaycastHit hit;

                DeselectAll();

                if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Unit")
                {
                    UnitSelector u = hit.transform.GetComponent<UnitSelector>();
                    if (u.GetComponentInParent<Unit>().owner == gameManager.playerNo)
                    {
                        u.Selected = true;
                        selectedUnits.Add(u);
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
        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = gameManager.camScript.MouseCursorRay();
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                foreach (UnitSelector u in selectedUnits)
                {
                    bool enqueue = Input.GetKey(KeyCode.LeftShift);
                    u.GetComponentInParent<Unit>().ClickOrder(hit, enqueue);
                }
            }
        }
    }

    private bool HostileUnit(Unit unit)
    {
        return unit.owner != gameManager.playerNo;
    }
    
    private void DeselectAll()
    {
        foreach (UnitSelector unitSelector in selectedUnits)
            unitSelector.Selected = false;

        selectedUnits.Clear();
    }

    private void SelectOneUnit(UnitSelector unit)
    {

    }

    private void SelectMultipleUnits(List<UnitSelector> units)
    {

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
