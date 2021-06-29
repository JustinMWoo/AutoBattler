using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusManager : MonoBehaviour
{
    #region Singleton
    private static BonusManager _instance;
    public static BonusManager Instance { get { return _instance; } }

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
    public Color EnabledBonus;
    public Color DisabledBonus;

    [Tooltip("Main panel holding all set bonuses")]
    public GameObject SetBonusesPanel;
    [Tooltip("Main panel holding all tribe bonuses")]
    public GameObject TribeBonusesPanel;

    [Tooltip("Panel for one Bonus")]
    public GameObject BonusPanelPrefab;

    [Tooltip("Icon prefab for a set")]
    public GameObject BonusIconPrefab;

    [Tooltip("Sub panel for holding hexes")]
    public GameObject HexPanelPrefab;

    [Tooltip("Hexes for holding number of units needed for bonus")]
    public GameObject BonusHexPrefab;


    // Dictionary using set type as the key, holding the number of units from that set
    protected Dictionary<BaseSet, int> playerSetsDict = new Dictionary<BaseSet, int>();
    protected Dictionary<BaseSet, int> npcSetsDict = new Dictionary<BaseSet, int>();

    protected Dictionary<BaseTribe, int> playerTribesDict = new Dictionary<BaseTribe, int>();
    protected Dictionary<BaseTribe, int> npcTribesDict = new Dictionary<BaseTribe, int>();

    // Dictionaries for panel containing entire set/tribe
    protected Dictionary<BaseSet, GameObject> setDisplayDict = new Dictionary<BaseSet, GameObject>();
    protected Dictionary<BaseTribe, GameObject> tribeDisplayDict = new Dictionary<BaseTribe, GameObject>();

    // Dictionary using set type as the key, holding the hexs for that set (for updating UI)
    protected Dictionary<BaseSet, List<GameObject>> setHexDict = new Dictionary<BaseSet, List<GameObject>>();

    protected Dictionary<BaseTribe, List<GameObject>> tribeHexDict = new Dictionary<BaseTribe, List<GameObject>>();

    protected Dictionary<string, BaseSet> allSetsDict;
    protected Dictionary<string, BaseTribe> allTribesDict;

    private void Start()
    {
        allSetsDict = new Dictionary<string, BaseSet>();
        BaseSet[] sets = Resources.LoadAll<BaseSet>("Sets");
        foreach (BaseSet set in sets)
        {
            allSetsDict.Add(set.name, set);
        }

        allTribesDict = new Dictionary<string, BaseTribe>();
        BaseTribe[] tribes = Resources.LoadAll<BaseTribe>("Tribes");
        foreach (BaseTribe tribe in tribes)
        {
            allTribesDict.Add(tribe.name, tribe);
        }

        EventManager.Instance.OnCombatStart += CalculateBonuses;
        EventManager.Instance.OnEndCombat += ResetBonuses;
    }
    public IEnumerator CalculateBonuses()
    {
        Stack coroutines = new Stack();
        coroutines.Push(StartCoroutine(CalculatePlayerBonuses()));
        coroutines.Push(StartCoroutine(CalculateNPCBonuses()));

        while (coroutines.Count > 0)
        {
            var coroutine = coroutines.Pop();
            yield return coroutine;
        }
    }
    public IEnumerator ResetBonuses()
    {
        foreach (KeyValuePair<BaseSet, int> set in playerSetsDict)
        {
            // Remove bonuses from sets for player units
            set.Key.RemoveBonus(true);
        }
        foreach (KeyValuePair<BaseTribe, int> tribe in playerTribesDict)
        {
            // remove bonuses from tribes for player units
            tribe.Key.RemoveBonus(true);
        }
        yield return null;
    }

    public void AddPlayerUnit(Unit unit)
    {
        // If unit is being added during combat then do not update
        if (TurnManager.Instance.IsInCombat())
            return;

        if (unit.Set != null)
        {
            if (playerSetsDict.ContainsKey(unit.Set))
            {
                playerSetsDict[unit.Set] += 1;
            }
            else
            {
                CreateUI(unit.Set);
                playerSetsDict.Add(unit.Set, 1);
            }
        }
        if (unit.Tribe != null)
        {

            if (playerTribesDict.ContainsKey(unit.Tribe))
            {
                playerTribesDict[unit.Tribe] += 1;
            }
            else
            {
                CreateUI(unit.Tribe);
                playerTribesDict.Add(unit.Tribe, 1);
            }
        }
        UpdateUI(unit);
    }

    public void RemovePlayerUnit(Unit unit)
    {
        // If unit is being removed during combat then do not update
        if (TurnManager.Instance.IsInCombat())
            return;

        if (unit.Set != null)
        {
            if (playerSetsDict.ContainsKey(unit.Set))
            {
                playerSetsDict[unit.Set] -= 1;
            }
        }
        if (unit.Tribe != null)
        {
            if (playerTribesDict.ContainsKey(unit.Tribe))
            {
                playerTribesDict[unit.Tribe] -= 1;
            }
        }
        UpdateUI(unit);
    }

    private void UpdateUI(Unit unit)
    {
        if (unit.Set != null)
        {
            if (playerSetsDict[unit.Set] <= 0)
            {
                // remove the set panel
                Destroy(setDisplayDict[unit.Set]);

                playerSetsDict.Remove(unit.Set);
                setDisplayDict.Remove(unit.Set);
                setHexDict.Remove(unit.Set);
            }
            else
            {
                List<int> tierRequirements = unit.Set.GetTierRequirements();
                for (int i = tierRequirements.Count - 1; i >= 0; i--)
                {
                    if (playerSetsDict[unit.Set] >= tierRequirements[i])
                    {
                        setHexDict[unit.Set][i].GetComponent<Image>().color = EnabledBonus;
                    }
                    else
                    {
                        setHexDict[unit.Set][i].GetComponent<Image>().color = DisabledBonus;
                    }
                }
            }
        }

        if (unit.Tribe != null)
        {
            if (playerTribesDict[unit.Tribe] <= 0)
            {
                // remove the tribe panel
                Destroy(tribeDisplayDict[unit.Tribe]);

                playerTribesDict.Remove(unit.Tribe);
                tribeDisplayDict.Remove(unit.Tribe);
                tribeHexDict.Remove(unit.Tribe);
            }
            else
            {
                List<int> tierRequirements = unit.Tribe.GetTierRequirements();

                for (int i = tierRequirements.Count - 1; i >= 0; i--)
                {
                    if (playerTribesDict[unit.Tribe] >= tierRequirements[i])
                    {
                        tribeHexDict[unit.Tribe][i].GetComponent<Image>().color = EnabledBonus;
                    }
                    else
                    {
                        tribeHexDict[unit.Tribe][i].GetComponent<Image>().color = DisabledBonus;
                    }
                }
            }
        }
    }

    public void AddNPCUnit(Unit unit)
    {
        // If unit is being added during combat then do not update
        if (TurnManager.Instance.IsInCombat())
            return;

        if (unit.Set != null)
        {
            if (npcSetsDict.ContainsKey(unit.Set))
            {
                npcSetsDict[unit.Set] += 1;
            }
            else
            {
                npcSetsDict.Add(unit.Set, 1);
            }
        }
        if (unit.Tribe != null)
        {

            if (npcTribesDict.ContainsKey(unit.Tribe))
            {
                npcTribesDict[unit.Tribe] += 1;
            }
            else
            {
                npcTribesDict.Add(unit.Tribe, 1);
            }
        }
    }
    public void ClearNPCBonuses()
    {
        npcSetsDict.Clear();
        npcTribesDict.Clear();
    }

    private void CreateUI(Bonus bonus)
    {
        GameObject newBonusPanel;
        if (bonus is BaseSet)
        {
            newBonusPanel = Instantiate(BonusPanelPrefab, SetBonusesPanel.transform);
            setDisplayDict[(BaseSet)bonus] = newBonusPanel;
        }
        else
        {
            newBonusPanel = Instantiate(BonusPanelPrefab, TribeBonusesPanel.transform);
            tribeDisplayDict[(BaseTribe)bonus] = newBonusPanel;
        }


        Instantiate(BonusIconPrefab, newBonusPanel.transform).GetComponent<Image>().sprite = bonus.icon;

        GameObject newHexPanel = Instantiate(HexPanelPrefab, newBonusPanel.transform);

        RectTransform hexPanelRect = newHexPanel.GetComponent<RectTransform>();

        // Resize the hex panel
        List<int> tierRequirements = bonus.GetTierRequirements();
        GridLayoutGroup hexLayoutGroup = newBonusPanel.GetComponent<GridLayoutGroup>();
        int hexPadding = hexLayoutGroup.padding.left + hexLayoutGroup.padding.right;
        hexPanelRect.sizeDelta = new Vector2((hexPanelRect.sizeDelta.x * tierRequirements.Count) + (hexLayoutGroup.spacing.x * (tierRequirements.Count - 1)) + hexPadding, hexPanelRect.sizeDelta.y);
        hexPanelRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, hexLayoutGroup.padding.right, hexPanelRect.rect.width);

        // Change size of newPanel to accommodate for hexes
        RectTransform bonusPanelRect = newBonusPanel.GetComponent<RectTransform>();
        //GridLayoutGroup setLayoutGroup = newSetPanel.GetComponent<GridLayoutGroup>();
        //int setPadding = setLayoutGroup.padding.left + setLayoutGroup.padding.right;

        //setPanelRect.sizeDelta = new Vector2((setPanelRect.sizeDelta.x + setLayoutGroup.spacing.x) + setPadding + hexPanelRect.sizeDelta.x, setPanelRect.sizeDelta.y); // Tier requirements + 1 for the icon
        bonusPanelRect.sizeDelta = new Vector2(bonusPanelRect.sizeDelta.x + hexPanelRect.sizeDelta.x, bonusPanelRect.sizeDelta.y);
        bonusPanelRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, bonusPanelRect.rect.width);

        List<GameObject> hexList = new List<GameObject>();

        for (int i = 0; i < tierRequirements.Count; i++)
        {
            GameObject hex = Instantiate(BonusHexPrefab, newHexPanel.transform);
            hex.GetComponentInChildren<TextMeshProUGUI>().text = tierRequirements[i].ToString();
            hex.GetComponent<Image>().color = DisabledBonus;
            hexList.Add(hex);
        }
        if (bonus is BaseSet set)
        {
            setHexDict.Add(set, hexList);
        }
        else if (bonus is BaseTribe tribe)
        {
            tribeHexDict.Add(tribe, hexList);
        }
    }

    private IEnumerator CalculatePlayerBonuses()
    {
        foreach (KeyValuePair<BaseSet, int> set in playerSetsDict)
        {
            // Apply the bonus for that number of units in the set
            yield return StartCoroutine(set.Key.ApplyBonus(set.Value, true));
        }

        foreach (KeyValuePair<BaseTribe, int> tribe in playerTribesDict)
        {
            // Apply the bonus for that number of units in the set
            yield return StartCoroutine(tribe.Key.ApplyBonus(tribe.Value, true));
        }
    }
    private IEnumerator CalculateNPCBonuses()
    {
        foreach (KeyValuePair<BaseSet, int> set in npcSetsDict)
        {
            // Apply the bonus for that number of units in the set
            yield return StartCoroutine(set.Key.ApplyBonus(set.Value, false));
        }

        foreach (KeyValuePair<BaseTribe, int> tribe in npcTribesDict)
        {
            // Apply the bonus for that number of units in the set
            yield return StartCoroutine(tribe.Key.ApplyBonus(tribe.Value, false));
        }
    }

    public Dictionary<BaseSet, int> GetPlayerSetBonuses()
    {
        return playerSetsDict;
    }

    public Dictionary<BaseTribe, int> GetPlayerTribeBonuses()
    {
        return playerTribesDict;
    }

    public BaseSet FindSet(string name)
    {
        allSetsDict.TryGetValue(name, out BaseSet set);
        return set;
    }

    public BaseTribe FindTribe(string name)
    {
        allTribesDict.TryGetValue(name, out BaseTribe tribe);
        return tribe;
    }
}
