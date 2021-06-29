using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitChoiceManager : MonoBehaviour
{
    #region Singleton/Awake
    private static UnitChoiceManager _instance;
    public static UnitChoiceManager Instance { get { return _instance; } }

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


        allUnitData = new List<UnitData[]>();

        //allUnitData = Resources.LoadAll<UnitData>("UnitData");
        //Debug.Log( Directory.GetDirectories("Assets/Resources/UnitData").Length);
        for (int i = 1; i <= Directory.GetDirectories("Assets/Resources/UnitDataBase").Length; i++)
        {
            allUnitData.Add(Resources.LoadAll<UnitData>("UnitDataBase/Tier " + i));
        }

        TierSpawnRates[] allSpawnRates = Resources.LoadAll<TierSpawnRates>("SpawnRates");

        spawnRates = new List<int[]>();
        foreach (TierSpawnRates rate in allSpawnRates)
        {
            spawnRates.Add(rate.rates);
        }

        currentChoices = new List<GameObject>();

    }
    #endregion

    [SerializeField]
    protected List<Transform> basicSpawnLocations;

    [SerializeField]
    protected List<Transform> shopSpawnLocations;

    [SerializeField]
    protected GameObject poofEffect;

    [SerializeField]
    protected Transform playerBoardParent;

    // Index of list is tier of unit
    protected List<UnitData[]> allUnitData;

    // index is tier of player - 1, array is spawn rates for each tier of unit at that tier for player
    protected List<int[]> spawnRates;

    protected List<GameObject> currentChoices;

    [Header("Selling prefabs")]
    [SerializeField]
    protected GameObject sellArea;
    [SerializeField]
    protected GameObject sellAnimation;

    private Coroutine current;
    private bool spawned = false;

    public List<UnitData[]> GetUnitData()
    {
        return allUnitData;
    }

    public bool IsSpawned()
    {
        return spawned;
    }


    public void SpawnBasicChoices()
    {
        if (current != null)
        {
            StopCoroutine(current);
        }
        current = StartCoroutine(Spawn(false));
    }

    public void SpawnShopChoices()
    {
        if (current != null)
        {
            StopCoroutine(current);
        }
        current = StartCoroutine(Spawn(true));
    }

    public void RemoveFromChoices(GameObject unit)
    {
        if (currentChoices.Contains(unit))
        {
            // Set the parent of the unit that is removed to the board parent
            unit.transform.SetParent(playerBoardParent);
            currentChoices.Remove(unit);
        }
    }

    private IEnumerator Spawn(bool shop)
    {        
        if (PlayerManager.Instance.Tier < 0 || PlayerManager.Instance.Tier >= spawnRates.Count)
        {
            Debug.LogError("Error: Tier spawn rate does not exist");
            yield break;
        }
        spawned = false;

        while (SceneAnimationManager.Instance.IsMoving())
        {
            yield return null;
        }

        PlayEffects();

        yield return StartCoroutine(RemoveCurrent());

        int[] curRate = spawnRates[PlayerManager.Instance.Tier];

        int rateSum = 0;
        foreach (int rate in curRate)
        {
            rateSum += rate;
        }

        List<Transform> spawns;
        if (shop)
        {
            spawns = shopSpawnLocations;
        }
        else
        {
            spawns = basicSpawnLocations;
        }

        foreach (Transform spawn in spawns)
        {
            int roll = Random.Range(0, rateSum + 1);

            int curSum = 0;
            for (int i = 0; i < curRate.Length; i++)
            {
                curSum += curRate[i];
                if (roll <= curSum)
                {
                    UnitData[] randomTier = allUnitData[i];

                    int randIndex = Random.Range(0, randomTier.Length);
                    UnitData randomUnit = randomTier[randIndex];
                    GameObject choice = UnitSpawner.Instance.SpawnUnit(spawn.position, randomUnit, false);

                    if (choice != null)
                    {
                        choice.AddComponent<Purchasable>().Cost = i + 1;
                        currentChoices.Add(choice);
                        choice.transform.SetParent(spawn.transform);
                        break;
                    }

                }
            }
        }

        spawned = true;
    }

    // TODO: Only poof effect if there is a unit there
    private void PlayEffects()
    {
        foreach (Transform spawn in shopSpawnLocations)
        {
            Instantiate(poofEffect, spawn);
        }
    }

    private IEnumerator RemoveCurrent()
    {
        foreach (GameObject choice in currentChoices)
        {
            Destroy(choice);
        }
        yield return null;
    }

    public void RemoveCurrentChoices()
    {
        PlayEffects();
        StartCoroutine(RemoveCurrent());
    }

    public void ActivateSellArea()
    {
        sellArea.SetActive(true);
    }

    public void DeactivateSellArea()
    {
        sellArea.SetActive(false);
    }

    public bool CheckSell(Unit unit)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("sell"))
            {
                // sell unit
                Board.Instance.RemovePlayerUnit(unit);
                Destroy(unit.gameObject);
                PlayerManager.Instance.Currency += 1;
                PlayerManager.Instance.CurNumUnits -= 1;
                
                Instantiate(sellAnimation, unit.gameObject.transform.position, Quaternion.identity);
                return true;
            }
        }
        return false;
    }

    public void Refresh()
    {
        if (PlayerManager.Instance.Currency >= 2 && spawned)
        {
            PlayerManager.Instance.Currency -= 2;
            SpawnShopChoices();
        }
    }
}
