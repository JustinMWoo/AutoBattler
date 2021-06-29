using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Purchasable : Draggable
{
    public int Cost { get; set; }

    protected override void OnMouseUp()
    {
        if (TurnManager.Instance.IsInCombat() || !active)
        {
            return;
        }
        if (PlayerManager.Instance.Currency >= Cost && PlayerManager.Instance.CurNumUnits + 1 <= PlayerManager.Instance.MaxUnits)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
            {
                //Debug.Log("Above tile");

                Tuple<int, int> pos = Board.Instance.PositionPlayerUnit((int)hit.transform.position.x, (int)hit.transform.position.z, gameObject, (int)startPos.x, (int)startPos.z);
                if (pos != null)
                {
                    transform.position = new Vector3(pos.Item1, hit.point.y, pos.Item2);
                    //Board.Instance.ReorganizePlayerBoard();

                    // Purchase unit
                    PlayerManager.Instance.Currency -= Cost;
                    PlayerManager.Instance.CurNumUnits += 1;

                    UnitChoiceManager.Instance.RemoveFromChoices(gameObject);

                    StartCoroutine(ChangeScript());
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
        else
        {
            transform.position = startPos;
        }
    }

    IEnumerator ChangeScript()
    {
        yield return new WaitForEndOfFrame();
        gameObject.AddComponent<Draggable>();
        Destroy(GetComponent<Purchasable>());
    }
}
