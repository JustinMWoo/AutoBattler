using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;

    public TextMeshProUGUI contentField;

    public LayoutElement layoutElement;

    public int characterWrapLimit;

    private RectTransform rectTransform;

    public GameObject mapBonusDisplay;

    private List<GameObject> mapBonusDisplays = new List<GameObject>();


    public GameObject unitBonusDisplay;
    public GameObject set;
    public GameObject tribe;
    [Header("Unit Stats")]
    public GameObject unitStats;
    public TextMeshProUGUI healthField;
    public TextMeshProUGUI speedField;
    public TextMeshProUGUI damageField;
    public TextMeshProUGUI defenceField;
    public GameObject[] stars;

    private bool frozen = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content = "", string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
            headerField.text = "";
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        if (string.IsNullOrEmpty(content))
        {
            contentField.gameObject.SetActive(false);
            contentField.text = "";
        }
        else
        {
            contentField.gameObject.SetActive(true);
            contentField.text = content;
        }

        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        HideAllBonuses();
        unitBonusDisplay.SetActive(false);

        if (unitStats != null)
        {
            unitStats.SetActive(false);
        }
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }
    }

    public void SetBonuses(NPCBoardState layout)
    {
        // Get the number of sets to display
        int totalBonuses = layout.setNames.Count + layout.tribeNames.Count;
        int setCount = 0;

        int tribeCount = 0;

        for (int i = 0; i < totalBonuses; i++)
        {
            if (mapBonusDisplays.Count < totalBonuses)
            {
                mapBonusDisplays.Add(Instantiate(mapBonusDisplay, this.gameObject.transform));
            }

            mapBonusDisplays[i].SetActive(true);

            if (i < layout.setNames.Count)
            {
                mapBonusDisplays[i].GetComponent<TooltipBonus>().UpdateSetDisplay(layout.setNames[setCount], layout.setCounts[setCount]);
                setCount++;
            }
            else
            {
                mapBonusDisplays[i].GetComponent<TooltipBonus>().UpdateTribeDisplay(layout.tribeNames[tribeCount], layout.tribeCounts[tribeCount]);
                tribeCount++;
            }
        }

        //for (int i = 0; i < tribeCount; i++)
        //{

        //    if (mapBonusDisplays.Count < (setCount + tribeCount))
        //    {
        //        mapBonusDisplays.Add(Instantiate(mapBonusDisplay, this.gameObject.transform));
        //    }
        //    mapBonusDisplays[i].SetActive(true);

        //    mapBonusDisplays[i].GetComponent<TooltipBonus>().UpdateTribeDisplay(layout.tribeNames[i], layout.tribeCounts[i]);
        //}
    }

    public void SetUnitData(Unit unit)
    {
        unitBonusDisplay.SetActive(true);
        unitStats.SetActive(true);

        Image setIcon = set.GetComponent<Image>();
        if (unit.UnitData.set != null)
        {
            setIcon.enabled = true;
            setIcon.sprite = unit.UnitData.set.icon;
            set.GetComponent<SubTooltipTrigger>().bonus = unit.UnitData.set;
            set.GetComponent<GraphicRaycaster>().enabled = true;
        }
        else
        {
            setIcon.enabled = false;
            set.GetComponent<GraphicRaycaster>().enabled = false;
        }

        Image tribeIcon = tribe.GetComponent<Image>();
        if (unit.UnitData.tribe != null)
        {
            tribeIcon.enabled = true;
            tribeIcon.sprite = unit.UnitData.tribe.icon;
            tribe.GetComponent<SubTooltipTrigger>().bonus = unit.UnitData.tribe;
            tribe.GetComponent<GraphicRaycaster>().enabled = true;
        }
        else
        {
            tribeIcon.enabled = false;
            tribe.GetComponent<GraphicRaycaster>().enabled = false;
        }

        // show unit's base stats
        healthField.text = unit.UnitData.health.ToString();
        speedField.text = unit.UnitData.speed.ToString();
        damageField.text = unit.UnitData.damage.ToString();
        defenceField.text = unit.UnitData.defence.ToString();

        for (int i = 0; i < unit.Level; i++)
        {
            stars[i].SetActive(true);
        }
    }

    public void DisableRaycasters()
    {
        set.GetComponent<GraphicRaycaster>().enabled = false;
        tribe.GetComponent<GraphicRaycaster>().enabled = false;
    }

    public void FreezePosition()
    {
        frozen = true;
    }

    public void UnfreezePosition()
    {
        frozen = false;
    }


    private void HideAllBonuses()
    {
        foreach (GameObject bonus in mapBonusDisplays)
        {
            bonus.SetActive(false);
        }
    }

    private void Update()
    {
        if (!frozen)
        {
            Vector2 position = Input.mousePosition;

            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;

            rectTransform.pivot = new Vector2(pivotX, pivotY);

            transform.position = position;
        }
    }
}
