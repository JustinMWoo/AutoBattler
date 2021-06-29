using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    #region Singleton
    private static TooltipSystem _instance;
    public static TooltipSystem Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public Tooltip tooltip;
    public Tooltip miniTooltip;
    public CanvasGroup miniCanvas;
    private bool updateCheck = true;
    private bool open = false;

    public bool IsOpen()
    {
        return open;
    }

    public void Show(string content = "", string header = "")
    {
        tooltip.SetText(content, header);
        LeanTween.alphaCanvas(this.gameObject.GetComponent<CanvasGroup>(), 1f, 0.1f);
        open = true;

    }
    public void ShowMini(string content = "", string header = "")
    {
        miniTooltip.SetText(content, header);
        LeanTween.alphaCanvas(miniCanvas, 1f, 0.1f);
    }

    public void ShowBonuses(NPCBoardState layout, string header = "")
    {
        tooltip.SetText("Sets/Tribes", header);
        tooltip.SetBonuses(layout);

        LeanTween.alphaCanvas(this.gameObject.GetComponent<CanvasGroup>(), 1f, 0.1f);
        open = true;
    }

    public void ShowUnit(Unit unit)
    {
        tooltip.SetText(unit.UnitData.description, unit.UnitData.unitName);
        tooltip.SetUnitData(unit);
        tooltip.FreezePosition();
        LeanTween.alphaCanvas(this.gameObject.GetComponent<CanvasGroup>(), 1f, 0.1f);
        open = true;
    }

    public void Hide()
    {
        LeanTween.alphaCanvas(this.gameObject.GetComponent<CanvasGroup>(), 0f, 0.1f).setOnComplete(tooltip.UnfreezePosition);
        LeanTween.alphaCanvas(miniCanvas, 0f, 0.1f);
        tooltip.DisableRaycasters();
        open = false;
    }

    public void HideMini()
    {
        LeanTween.alphaCanvas(miniCanvas, 0f, 0.1f);
    }

    public void DisableCheckForFrame()
    {
        StartCoroutine(DisableUntilNextFrame());
    }
    IEnumerator DisableUntilNextFrame()
    {
        updateCheck = false;
        yield return new WaitForEndOfFrame();
        updateCheck = true;
    }



    private void Update()
    {
        if (updateCheck)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Hide();
            }
            if (Input.GetMouseButtonDown(1))
            {
                Hide();
            }
        }
    }
}
