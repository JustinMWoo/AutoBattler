using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitSpawnSelector : MonoBehaviour
{
    public TMP_Dropdown tierDropdown;
    public TMP_Dropdown unitDropdown;

    private List<UnitData[]> allUnitData;

    [SerializeField]
    protected List<Transform> spawnLocations;

    private List<GameObject> currentChoices = new List<GameObject>();

    private void Start()
    {
        allUnitData = UnitChoiceManager.Instance.GetUnitData();

        for (int i = 1; i <= allUnitData.Count; i++)
        {
            tierDropdown.options.Add(new TMP_Dropdown.OptionData("Tier " + i));
        }
        tierDropdown.RefreshShownValue();

        ChangeUnitDropdown();

        tierDropdown.onValueChanged.AddListener(delegate
        {
            ChangeUnitDropdown();
        });
    }

    private void ChangeUnitDropdown()
    {
        unitDropdown.ClearOptions();

        // For all units in the tier
        foreach (UnitData unit in allUnitData[tierDropdown.value])
        {
            unitDropdown.options.Add(new TMP_Dropdown.OptionData(unit.name));
        }
        unitDropdown.RefreshShownValue();
    }

    public void SpawnUnit()
    {
        StartCoroutine(SpawnUnitHelper());
    }

    IEnumerator SpawnUnitHelper()
    {
        foreach (GameObject curUnit in currentChoices)
        {
            if (curUnit != null && !Board.Instance.GetPlayerUnits().Contains(curUnit.GetComponent<Unit>())) // if the unit is not on the board then remove it
            {
                Destroy(curUnit);
            }
        }
        currentChoices.Clear();

        yield return new WaitForEndOfFrame();

        UnitData unit = allUnitData[tierDropdown.value][unitDropdown.value];
        foreach (Transform spawn in spawnLocations)
        {
            GameObject newUnit = UnitSpawner.Instance.SpawnUnit(spawn.position, unit, false);
            currentChoices.Add(newUnit);
            newUnit.AddComponent<Draggable>();
        }
    }
}
