using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour
{
    //private const int yOffset = 1;
    protected Vector3 startPos;

    protected LayerMask layerMask;

    protected bool active;

    private Unit unit;

    private void Start()
    {
        layerMask = LayerMask.GetMask("tile");
        unit = GetComponent<Unit>();
    }

    protected virtual void OnMouseDrag()
    {
        if (TurnManager.Instance.IsInCombat() || !active || unit.Moving)
        {
            return;
        }

        if (GameManager.Instance != null && (GameManager.Instance.GetPhase() is BuyingPhase || GameManager.Instance.GetPhase() is ShopPhase) && !(this is Purchasable))
        {
            UnitChoiceManager.Instance.ActivateSellArea();
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.red);
        float planeY = 1.5f;
        Transform draggingObject = transform;

        Plane plane = new Plane(Vector3.up, Vector3.up * planeY); // ground plane

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float distance; // the distance from the ray origin to the ray intersection of the plane
        if (plane.Raycast(ray, out distance))
        {
            draggingObject.position = ray.GetPoint(distance); // distance along the ray
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
        {
            Tuple<int, int> previewImageXZ = Board.Instance.FindNewPosition((int)hit.transform.position.x, (int)hit.transform.position.z);

            if (previewImageXZ != null)
            {
                Vector3 previewImagePos = new Vector3(previewImageXZ.Item1, hit.point.y, previewImageXZ.Item2);

                // TODO: add the preview image
                Debug.DrawRay(previewImagePos, Vector3.up);
            }
        }

    }

    protected virtual void OnMouseDown()
    {
        if (TurnManager.Instance.IsInCombat() || EventSystem.current.IsPointerOverGameObject() || unit.Moving)
        {
            active = false;
            return;
        }
        active = true;
        startPos = gameObject.transform.position;

        // Add and remove unit to make preview look more natural (currently doesnt work for moving units in a line)
        //Board.Instance.RemovePlayerUnit((int)transform.position.x, (int)transform.position.z);

    }

    protected virtual void OnMouseUp()
    {
        if (TurnManager.Instance.IsInCombat() || !active || unit.Moving)
        {
            return;
        }

        if (UnitChoiceManager.Instance.CheckSell(gameObject.GetComponent<Unit>()))
        {
            UnitChoiceManager.Instance.DeactivateSellArea();
            return;
        }
        UnitChoiceManager.Instance.DeactivateSellArea();

        // Add and remove unit to make preview look more natural (currently doesnt work for moving units in a line)
        //Board.Instance.AddPlayerUnit((int)startPos.x, (int)startPos.z , gameObject);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
        {
            //Debug.Log("Above tile");

            Tuple<int, int> pos = Board.Instance.PositionPlayerUnit((int)hit.transform.position.x, (int)hit.transform.position.z, gameObject, (int)startPos.x, (int)startPos.z);
            if (pos != null)
            {
                transform.position = new Vector3(pos.Item1, hit.point.y, pos.Item2);
                Board.Instance.ReorganizePlayerBoard();
            }
            else
            {
                //Debug.Log("Not above tile");
                transform.position = startPos;
            }

        }
        else
        {
            //Debug.Log("Not above tile");
            transform.position = startPos;
        }
    }

}
