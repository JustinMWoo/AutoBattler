using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElementalBonus : UnitAction
{
    [SerializeField]
    private GameObject effectPrefab;
    public int MaxSpawns { get; set; }

    private List<GameObject> spawns = new List<GameObject>();
    public override IEnumerator Execute()
    {
        List<Vector3> adjacentTiles = new List<Vector3>();
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        int numSpawns = MaxSpawns;

        Unit newUnit = UnitSpawner.Instance.SpawnUnit(unit.gameObject.transform.position, unit.UnitData, !unit.IsPlayerUnit(), unit.Level).GetComponent<Unit>();
        Instantiate(effectPrefab, newUnit.transform);

        // Get y position for tiles
        RaycastHit hit;
        float ypos = 0;
        if (Physics.Raycast(new Vector3(pos.Item1, 3, pos.Item2), Vector3.down, out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
        {
            ypos = hit.point.y;
        }

        // Change new unit scale and stats
        newUnit.Model.transform.localScale = newUnit.Model.transform.localScale * 0.5f;
        newUnit.damage.AddModifier(new StatModifier(-0.5f, StatModType.PercentAdd));
        newUnit.maxHealth.AddModifier(new StatModifier(-0.5f, StatModType.PercentAdd));
        newUnit.speed.AddModifier(new StatModifier(-0.5f, StatModType.PercentAdd));
        newUnit.defence.AddModifier(new StatModifier(-0.5f, StatModType.PercentAdd));

        StartCoroutine(newUnit.ResetCurrentHealth());


        spawns.Add(newUnit.gameObject);
        if (unit.IsPlayerUnit())
        {
            Board.Instance.AddPlayerUnit(pos.Item1, pos.Item2, newUnit.gameObject, true);
        }
        else
        {
            Board.Instance.AddNPCUnit(pos.Item1, pos.Item2, newUnit.gameObject, true);
        }

        numSpawns -= 1;

        if (numSpawns > 0)
        {
            if (Board.Instance.CheckPositionForUnit(pos.Item1, pos.Item2 - 1, unit.IsPlayerUnit()) == null)
            {
                 Debug.Log("Target found behind");
                adjacentTiles.Add(new Vector3(pos.Item1, 0, pos.Item2 - 1));
            }

            if (Board.Instance.CheckPositionForUnit(pos.Item1 + 1, pos.Item2, unit.IsPlayerUnit()) == null)
            {
                 Debug.Log("Target found right");
                adjacentTiles.Add(new Vector3(pos.Item1 + 1, 0, pos.Item2));
            }

            if (Board.Instance.CheckPositionForUnit(pos.Item1 - 1, pos.Item2, unit.IsPlayerUnit()) == null)
            {
                 Debug.Log("Target found left");
                adjacentTiles.Add(new Vector3(pos.Item1 - 1, 0, pos.Item2));
            }

            while (adjacentTiles.Count > 0 && numSpawns > 0)
            {
                Vector3 targetTile = adjacentTiles[UnityEngine.Random.Range(0, adjacentTiles.Count - 1)];
                adjacentTiles.Remove(targetTile);
                numSpawns -= 1;

                newUnit = UnitSpawner.Instance.SpawnUnit(targetTile, unit.UnitData, !unit.IsPlayerUnit(), unit.Level).GetComponent<Unit>();
                Instantiate(effectPrefab, newUnit.transform);

                // Change new unit scale and stats
                newUnit.Model.transform.localScale = newUnit.Model.transform.localScale * 0.5f;
                newUnit.damage.AddModifier(new StatModifier(-0.5f, StatModType.PercentAdd));
                newUnit.maxHealth.AddModifier(new StatModifier(-0.5f, StatModType.PercentAdd));
                newUnit.speed.AddModifier(new StatModifier(-0.5f, StatModType.PercentAdd));
                newUnit.defence.AddModifier(new StatModifier(-0.5f, StatModType.PercentAdd));

                StartCoroutine(newUnit.ResetCurrentHealth());

                spawns.Add(newUnit.gameObject);
                if (unit.IsPlayerUnit())
                {
                    Board.Instance.AddPlayerUnit((int)targetTile.x, (int)targetTile.z, newUnit.gameObject);
                }
                else
                {
                    Board.Instance.AddNPCUnit((int)targetTile.x, (int)targetTile.z, newUnit.gameObject);
                }
            }
        }

        yield return null;
    }

    // Remove this component at the end of combat
    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.OnEndCombat += RemoveBonus;

    }
    private IEnumerator RemoveBonus()
    {
        unit.RemoveOnDeath(this);
        foreach (GameObject unit in spawns)
        {
            Destroy(unit);
        }

        Destroy(this.gameObject);
        yield return null;
    }
    private void OnDestroy()
    {
        EventManager.Instance.OnEndCombat -= RemoveBonus;
    }
}
