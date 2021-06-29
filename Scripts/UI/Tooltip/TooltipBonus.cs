using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipBonus : MonoBehaviour
{
    public Image bonusIcon;
    public TextMeshProUGUI bonusCountField;

    public void UpdateSetDisplay(string setName, int setCount)
    {
        BaseSet set = BonusManager.Instance.FindSet(setName);
        if (set != null)
        {
            bonusIcon.sprite = set.icon;
            bonusCountField.text = setCount.ToString();
        }
        else
        {
            Debug.LogError("No set found with that name");
        }
    }
    public void UpdateTribeDisplay(string tribeName, int tribeCount)
    {
        BaseTribe tribe = BonusManager.Instance.FindTribe(tribeName);
        if (tribe != null)
        {
            bonusIcon.sprite = tribe.icon;
            bonusCountField.text = tribeCount.ToString();
        }
        else
        {
            Debug.LogError("No set found with that name");
        }
    }
}
