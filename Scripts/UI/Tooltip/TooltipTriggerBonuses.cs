using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipTriggerBonuses : TooltipTrigger, IPointerEnterHandler, IPointerExitHandler
{
    public NodeDisplay nodeDisplay;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipSystem.Instance.IsOpen())
        {
            LeanTween.cancel(delay);
            TooltipSystem.Instance.Hide();
        }

        NPCBoardState layout = gameObject.GetComponent<NodeDisplay>().Node.Layout;
        if (layout != null)
        {

            delay = LeanTween.delayedCall(0.5f, () =>
            {
                TooltipSystem.Instance.ShowBonuses(layout, header);
            }).uniqueId;
        }
        else
        {
            base.OnPointerEnter(eventData);
        }
    }
}
