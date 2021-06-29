using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TriplerManager : MonoBehaviour
{
    #region Singleton
    private static TriplerManager _instance;
    public static TriplerManager Instance { get { return _instance; } }

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
    private GameObject LevelUpEffect;
    [SerializeField]
    private GameObject RemoveUnitEffect;

    // Dictionary of units of that type
    private Dictionary<UnitData, List<Unit>> units = new Dictionary<UnitData, List<Unit>>();

    public void RemoveUnit(Unit unit)
    {
        if (TurnManager.Instance.IsInCombat())
        {
            return;
        }
        units[unit.UnitData].Remove(unit);
    }

    public void AddUnit(Unit unit)
    {
        if (TurnManager.Instance.IsInCombat())
        {
            return;
        }
        if (!units.ContainsKey(unit.UnitData))
        {
            units[unit.UnitData] = new List<Unit>() { unit };
        }
        else
        {
            units[unit.UnitData].Add(unit);
            if (units[unit.UnitData].Count >= 3)
            {
                StartCoroutine(TripleUnitType(unit.UnitData, unit.Level));
            }
        }

        // Check if that level of units already exists
        //if (units[unit.UnitData].Count >= unit.Level)
        //{
        //    // if that level exists then add unit
        //    units[unit.UnitData][unit.Level - 1].Add(unit);

        //    // if there are now 3 or more of that unit type then triple
        //    if (units[unit.UnitData][unit.Level - 1].Count >= 3)
        //    {
        //        StartCoroutine(TripleUnitType(unit.UnitData, unit.Level));
        //    }
        //}
        //else if (unit.Level < 3) // < 3 because units can only be tripled twice
        //{
        //    List<Unit> newList = new List<Unit>
        //    {
        //        unit
        //    };
        //    units[unit.UnitData].Add(newList);
        //}
    }


    private IEnumerator TripleUnitType(UnitData data, int level)
    {
        // Save the position of the first unit to put the leveled up version there
        Vector3 oldPos = units[data][0].gameObject.transform.position;
        PlayerManager.Instance.CurNumUnits -= 2;

        UnitData newUnitData = Resources.Load<UnitData>("UnitDataLeveled/Level " + (level + 1) + "/" + data.unitName + " " + (level + 1));
        foreach (Unit unit in units[data].ToList())
        {
            if (unit.gameObject.transform.position != oldPos)
            {
                Instantiate(RemoveUnitEffect, unit.gameObject.transform.position, Quaternion.identity);
            }
            // remove the units
            Board.Instance.RemovePlayerUnit(unit, false);
            Destroy(unit.gameObject);
        }
        units[data].Clear();
        yield return new WaitForEndOfFrame();
        // Spawn the new unit after unit has been destroyed
        GameObject newUnit = UnitSpawner.Instance.SpawnUnit(oldPos, newUnitData, false, (level + 1));
        newUnit.AddComponent<Draggable>();
        Instantiate(LevelUpEffect, oldPos, Quaternion.identity);

        Board.Instance.AddPlayerUnit((int)oldPos.x, (int)oldPos.z, newUnit);
        Board.Instance.ReorganizePlayerBoard();
    }
}
