using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Stat
{
    health,
    damage,
    speed,
    defence
}

public class FloatingStatChangeSpawner : MonoBehaviour
{
    #region Singleton
    private static FloatingStatChangeSpawner _instance;
    public static FloatingStatChangeSpawner Instance { get { return _instance; } }

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
    [Header("Health")]
    public GameObject HealthUp;
    public GameObject HealthDown;

    [Header("Damage")]
    public GameObject DamageUp;
    public GameObject DamageDown;

    [Header("Speed")]
    public GameObject SpeedUp;
    public GameObject SpeedDown;

    [Header("Defence")]
    public GameObject DefenceUp;
    public GameObject DefenceDown;

    Dictionary<Unit, List<Tuple<Stat, int>>> StatDictionary = new Dictionary<Unit, List<Tuple<Stat, int>>>();

    public void AddStatChange(Unit unit, Stat stat, int value)
    {
        if (StatDictionary.ContainsKey(unit))
        {
            bool found = false;
            for (int i = 0; i < StatDictionary[unit].Count; i++)
            {
                if (StatDictionary[unit][i].Item1 == stat)
                {
                    found = true;

                    StatDictionary[unit][i] = new Tuple<Stat, int>(stat, StatDictionary[unit][i].Item2 + value);
                    break;
                }
            }

            if (!found)
            {
                StatDictionary[unit].Add(new Tuple<Stat, int>(stat, value));
            }
        }
        else
        {
            StatDictionary[unit] = new List<Tuple<Stat, int>>() { new Tuple<Stat, int>(stat, value) };
        }
    }

    public void SpawnOverallStatChanges()
    {

        foreach (KeyValuePair<Unit, List<Tuple<Stat, int>>> item in StatDictionary)
        {
            foreach (var stat in item.Value)
            {
                switch (stat.Item1)
                {
                    case Stat.health:
                        if (stat.Item2 > 0)
                        {

                        }
                        break;
                    case Stat.damage:
                        break;
                    case Stat.speed:
                        break;
                    case Stat.defence:
                        break;
                }
            }
        }
    }

    public IEnumerator SpawnFloatingIcon(Transform unit, string text, Stat stat, bool up)
    {
        RaycastHit hit;
        Vector3 newPos = new Vector3(unit.transform.position.x, 3, unit.transform.position.z);
        if (Physics.Raycast(newPos, Vector3.down, out hit, Mathf.Infinity))
        {
            newPos = new Vector3(unit.position.x, hit.point.y, unit.position.z);
        }

        switch (stat)
        {
            case Stat.health:
                if (up)
                {
                    Instantiate(HealthUp, newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text);
                }
                else
                {
                    Instantiate(HealthDown, newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text);
                }
                break;
            case Stat.damage:
                if (up)
                {
                    Instantiate(DamageUp, newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text);
                }
                else
                {
                    Instantiate(DamageDown, newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text);
                }
                break;
            case Stat.speed:
                if (up)
                {
                    Instantiate(SpeedUp, newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text);
                }
                else
                {
                    Instantiate(SpeedDown, newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text);
                }
                break;
            case Stat.defence:
                if (up)
                {
                    Instantiate(DefenceUp, newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text);
                }
                else
                {
                    Instantiate(DefenceDown, newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text);
                }
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(0.5f);
    }
}
