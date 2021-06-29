using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTriggerUnits : TooltipTrigger
{
    private Unit unit;

    private void Start()
    {
        unit = GetComponent<Unit>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            // if no menu open yet
            if (!TooltipSystem.Instance.IsOpen())
            {
                TooltipSystem.Instance.DisableCheckForFrame();
                TooltipSystem.Instance.ShowUnit(unit);
            }
            else
            {
                LeanTween.cancel(delay);
                TooltipSystem.Instance.Hide();
            }
        }

        // Close if dragging unit
        if (Input.GetMouseButtonDown(0))
        {
            LeanTween.cancel(delay);
            TooltipSystem.Instance.Hide();
        }
    }
    private void OnMouseExit()
    {
        //LeanTween.cancel(delay);
        //TooltipSystem.Instance.Hide();
    }
}
