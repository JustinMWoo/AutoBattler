using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TimedDebuffManager : MonoBehaviour
{
    #region Singleton
    private static TimedDebuffManager _instance;
    public static TimedDebuffManager Instance { get { return _instance; } }

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

    // Dictionary holding units with their debuffs
    protected Dictionary<Unit, List<Debuff>> debuffDict = new Dictionary<Unit, List<Debuff>>();

    private void Start()
    {
        EventManager.Instance.OnDebuffStart += ExecuteCurrentDebuffs;
        EventManager.Instance.OnCombatStart += ResetDebuffs;
    }

    public void AddDebuff(Unit unit, Debuff debuff)
    {
        if (debuffDict.ContainsKey(unit))
        {
            // Refresh timer if unit already has the same type of buff
            foreach (Debuff curDebuff in debuffDict[unit])
            {
                if (curDebuff.GetType().Equals(debuff.GetType()))
                {
                    curDebuff.RefreshTime();
                    return;
                }
            }

            debuffDict[unit].Add(debuff);
        }
        else
        {
            debuffDict[unit] = new List<Debuff>() { debuff };
        }
    }

    public void RemoveDebuff(Unit unit, Debuff debuff)
    {
        debuffDict[unit].Remove(debuff);

        if (debuffDict[unit].Count < 1)
        {
            debuffDict.Remove(unit);
        }
    }

    public IEnumerator ExecuteCurrentDebuffs()
    {
        if (!debuffDict.ContainsKey(TurnManager.Instance.GetCurrentUnit()))
        {
            yield break;
        }

        foreach (Debuff debuff in debuffDict[TurnManager.Instance.GetCurrentUnit()].ToList())
        {
            yield return StartCoroutine(debuff.Execute());
        }

        yield return null;
    }

    public IEnumerator ResetDebuffs()
    {
        debuffDict = new Dictionary<Unit, List<Debuff>>();
        yield return null;
    }
}
