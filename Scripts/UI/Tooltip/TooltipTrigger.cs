using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected static int delay;

    public string content;
    public string header;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        delay = LeanTween.delayedCall(0.5f, () =>
        {
            TooltipSystem.Instance.Show(content, header);
        }).uniqueId;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay);
        TooltipSystem.Instance.Hide();
    }
}
