using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stun : UnitAction
{
    [SerializeField]
    private GameObject floatingTextPrefab;

    private List<UnitAction> currentTurnStart;

    public void Initialize()
    {
        currentTurnStart = unit.GetTurnStart();

        // If the unit is already stunned
        if (currentTurnStart.OfType<Stun>().Any())
        {
            Destroy(this.gameObject);
            return;
        }

        for (int i = 0; i < currentTurnStart.Count; i++)
        {
            unit.RemoveTurnStart(currentTurnStart[i]);
        }
        unit.AddTurnStart(this);
    }

    public override IEnumerator Execute()
    {
        unit.RemoveFromEvents();
        unit.RemoveTurnStart(this);
        foreach (UnitAction action in currentTurnStart)
        {
            unit.AddTurnStart(action);
        }
        Instantiate(floatingTextPrefab, transform.position, Quaternion.identity).GetComponent<FloatingIcon>().SetText("Stunned");
        yield return new WaitForSeconds(0.5f);
        EventManager.Instance.OnEndCombat -= RemoveStun;
        Destroy(this.gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.OnEndCombat += RemoveStun;
    }
    private IEnumerator RemoveStun()
    {
        EventManager.Instance.OnEndCombat -= RemoveStun;
        unit.RemoveTurnStart(this);
        foreach (UnitAction action in currentTurnStart)
        {
            unit.AddTurnStart(action);
        }
        Destroy(this.gameObject);
        yield return null;
    }

    // If enemy unit is killed to prevent error
    private void OnDestroy()
    {
        EventManager.Instance.OnEndCombat -= RemoveStun;
    }
}
