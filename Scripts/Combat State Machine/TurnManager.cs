using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    #region Singleton
    private static TurnManager _instance;
    public static TurnManager Instance { get { return _instance; } }

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

    //private List<Unit> allUnits;
    protected Queue<Unit> unitsInCycle;

    protected Unit currentUnit;
    protected bool inCombat;

    protected State state;

    [SerializeField]
    protected GameObject winDisplay;

    [SerializeField]
    protected GameObject lossDisplay;

    public bool pause = false;
    private Stack deathAnimations = new Stack();


    #region Getters
    public bool IsInCombat()
    {
        return inCombat;
    }
    public void EndCombat()
    {
        inCombat = false;
    }

    public Unit GetCurrentUnit()
    {
        return currentUnit;
    }
    #endregion

    #region Unused
    public void Start()
    {

        // Handled in TurnMachine (if added to event it would become non-deterministic when it would sort by speed)
        // EventManager.Instance.onCycleStart += SortBySpeed;
    }

    //public void StartCombat()
    //{
    //    EventManager.Instance.CombatStart();
    //}

    //public void StartTurn()
    //{
    //    currentUnit = unitsInCycle.Dequeue();
    //    EventManager.Instance.TurnStart();
    //}

    //public void StartAttack()
    //{
    //    if (currentUnit.IsPlayerUnit())
    //    {
    //        EventManager.Instance.PlayerAttackStart();      
    //    }
    //    else
    //    {
    //        EventManager.Instance.NPCAttackStart();
    //    }

    //    currentUnit.Attack();
    //}
    #endregion

    public void SetState(State state)
    {
        StartCoroutine(SetStateHelper(state));
    }

    private IEnumerator SetStateHelper(State state)
    {
        while (deathAnimations.Count > 0)
        {
            var animation = deathAnimations.Pop();
            yield return animation;
        }

        yield return new WaitUntil(() => pause == false);

        this.state = state;
        if (this.state != null)
        {
            //Debug.Log("Starting state..." + state);
            yield return StartCoroutine(state.Start());
        }
        else
        {
            // End combat
            inCombat = false;
        }
    }

    public void StateStart()
    {
        if (state  == null)
        {
            ButtonManager.Instance.DisableButton(Buttons.Combat);
            inCombat = true;
            state = new CombatStart();
            SetState(new CombatStart());
        }
    }

    /*
     * Set the current unit to the next unit in the queue
     * @return true if the queue is not empty, otherwise returns false
     */
    public bool NextUnit()
    {
        if (unitsInCycle.Count != 0)
        {
            currentUnit = unitsInCycle.Dequeue();
            return true;
        }
        else
        {
            return false;
        }
    }

    // Sorts the units on the board by speed and creates a new queue for them (randomizes order if units have the same speed)
    public void SortBySpeed()
    {
        //List<Unit> allUnits = Board.Instance.GetAllUnits();
        //unitsInCycle = new Queue<Unit>(allUnits.OrderByDescending(o => (int) o.speed.Value).ToList());

        var allUnits = Board.Instance.GetAllUnits().GroupBy(x => (int)x.speed.Value).Select(y => new { Speed = y.Key, Units = y.ToList() }).OrderByDescending(o => o.Speed);
        
        unitsInCycle = new Queue<Unit>();
        foreach (var speed in allUnits)
        {
            List<Unit> shuffled = Shuffle(speed.Units);
            shuffled.ForEach(o => unitsInCycle.Enqueue(o));
        }

        //foreach (var unit in unitsInCycle)
        //{
        //    Debug.Log(unit.UnitData.name);
        //}
    }

    // Helper function to shuffle units with the same speed
    private List<Unit> Shuffle(List<Unit> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            Unit temp = units[i];
            int randomIndex = UnityEngine.Random.Range(i, units.Count);
            units[i] = units[randomIndex];
            units[randomIndex] = temp;
        }
        return units;
    }
    public void RemoveUnitFromCycle(Unit unit)
    {
        unitsInCycle = new Queue<Unit>(unitsInCycle.Where(x => x != unit));
    }

    public void CombatWinDisplay(bool show)
    {
        if (show)
        {
            winDisplay.SetActive(true);
        }
        else
        {
            winDisplay.SetActive(false);
        }
    }

    public void CombatLossDisplay(bool show)
    {
        if (show)
        {
            lossDisplay.SetActive(true);
        }
        else
        {
            lossDisplay.SetActive(false);
        }
    }

    public void WaitDeathAnimation(Coroutine anim)
    {
        deathAnimations.Push(anim);
    }
}
