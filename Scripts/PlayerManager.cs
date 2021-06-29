using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

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

    [SerializeField]
    private TextMeshProUGUI tierText;

    [SerializeField]
    private TextMeshProUGUI currencyText;

    [SerializeField]
    private TextMeshProUGUI unitsText;

    [SerializeField]
    private ParticleSystem currencyEffect;

    private int currency = 0;

    private int tier = 0;

    private int curNumUnits = 0;

    private int maxUnits = 0;


    public int Currency
    {
        get { return currency; }
        set
        {
            currency = value;
            currencyText.text = "Currency: " + currency;
            currencyEffect.Play();
        }
    }

    public int Tier
    {
        get { return tier; }
        set
        {
            tier = value;
            tierText.text = "Tier: " + (tier + 1);
        }
    }

    public int CurNumUnits
    {
        get { return curNumUnits; }
        set
        {
            curNumUnits = value;
            unitsText.text = "Units: " + curNumUnits + "/" + maxUnits;
        }
    }

    public int MaxUnits
    {
        get { return maxUnits; }
        set
        {
            maxUnits = value;
            unitsText.text = "Units: " + curNumUnits + "/" + maxUnits;
        }
    }

    public void TryTierUp()
    {
        if (UnitChoiceManager.Instance.IsSpawned())
        {
            int currentCost = (tier + 1) * 3;

            if (currency >= currentCost)
            {
                Currency -= currentCost;
                MaxUnits += 1;
                Tier = tier + 1;
            }
        }
    }

    public void TryIncreaseUnitMax()
    {
        if (UnitChoiceManager.Instance.IsSpawned())
        {
            if (currency >= 2 && maxUnits <= 28)
            {
                Currency -= 2;
                MaxUnits += 1;
            }
        }
    }
}
