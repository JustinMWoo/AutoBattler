using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    #region Singleton
    private static UnitSpawner _instance;
    public static UnitSpawner Instance { get { return _instance; } }

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
    protected GameObject unitBasePrefab;

    public GameObject SpawnUnit(Vector3 pos, UnitData data, bool npc, int level = 1)
    {
        // Use raycast to determine Y
        RaycastHit hit;
        Vector3 newPos = new Vector3(pos.x, SceneAnimationManager.Instance.deactivatedY + 3, pos.z);
        if (Physics.Raycast(newPos, Vector3.down, out hit, Mathf.Infinity))
        {
            GameObject newUnit;
            // Spawn the unit and all its components
            if (!npc)
            {
                newUnit = Instantiate(unitBasePrefab, new Vector3(pos.x, hit.point.y, pos.z), Quaternion.identity);
            }
            else
            {
                newUnit = Instantiate(unitBasePrefab, new Vector3(pos.x, hit.point.y, pos.z), Quaternion.Euler(0, 180, 0));
            }

            Unit unitScript = newUnit.GetComponent<Unit>();

            unitScript.UnitData = data;
            GameObject unitModel = null;
            if (data.modelPrefab != null)
            {
                unitModel = Instantiate(data.modelPrefab, newUnit.transform);
                unitScript.animator = unitModel.GetComponent<Animator>();
            }
            else
            {
                Debug.Log("No model assigned to unit...");
            }


            if (data.attackType != null)
            {

                BasicAttack attack = Instantiate(data.attackType, newUnit.transform).GetComponent<BasicAttack>();
                unitScript.AttackType = attack;
            }

            if (data.turnEnd != null)
            {
                UnitAction turnEnd = Instantiate(data.turnEnd, newUnit.transform).GetComponent<UnitAction>();
                unitScript.AddTurnEnd(turnEnd);
            }

            if (data.turnStart != null)
            {
                UnitAction turnStart = Instantiate(data.turnStart, newUnit.transform).GetComponent<UnitAction>();
                unitScript.AddTurnStart(turnStart);
            }

            if (data.onHealthChanged != null)
            {
                UnitAction onHealthChanged = Instantiate(data.onHealthChanged, newUnit.transform).GetComponent<UnitAction>();
                unitScript.AddHealthChanged(onHealthChanged);
            }

            if (data.onDeath != null)
            {
                UnitAction onDeath = Instantiate(data.onDeath, newUnit.transform).GetComponent<UnitAction>();
                unitScript.AddOnDeath(onDeath);
            }

            // Set stats  
            unitScript.speed = new CharacterStat(data.speed);
            unitScript.damage = new CharacterStat(data.damage);
            unitScript.maxHealth = new CharacterStat(data.health);
            unitScript.defence = new CharacterStat(data.defence);
            unitScript.Model = unitModel;
            unitScript.Set = data.set;
            unitScript.Tribe = data.tribe;
            unitScript.Level = level;

            return newUnit;
        }
        return null;
    }

    //public GameObject SpawnUnit(Vector3 pos, Unit unit)
    //{
    //    // Use raycast to determine Y
    //    RaycastHit hit;
    //    Vector3 newPos = new Vector3(pos.x, SceneAnimationManager.Instance.deactivatedY + 3, pos.z);
    //    if (Physics.Raycast(newPos, Vector3.down, out hit, Mathf.Infinity))
    //    {
    //        GameObject newUnit;

    //        if (unit.IsPlayerUnit())
    //        {
    //            newUnit = Instantiate(unit.gameObject, new Vector3(pos.x, hit.point.y, pos.z), Quaternion.identity);
    //        }
    //        else
    //        {
    //            newUnit = Instantiate(unit.gameObject, new Vector3(pos.x, hit.point.y, pos.z), Quaternion.Euler(0, 180, 0));
    //        }
    //        newUnit.GetComponent<Unit>().Model = newUnit.GetComponent<Unit>().animator.gameObject;

    //        return newUnit;
    //    }
    //    return null;
    //}
}
