using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubTooltipTrigger : TooltipTrigger, IPointerEnterHandler, IPointerExitHandler
{
    public Bonus bonus;
    private Image icon;
    private void Awake()
    {
        icon = gameObject.GetComponent<Image>();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (icon.enabled && bonus != null && TooltipSystem.Instance.IsOpen())
        {
            delay = LeanTween.delayedCall(0.5f, () =>
            {
                TooltipSystem.Instance.ShowMini(bonus.description, bonus.name);
            }).uniqueId;
        }

    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay);
        TooltipSystem.Instance.HideMini();
    }
}
