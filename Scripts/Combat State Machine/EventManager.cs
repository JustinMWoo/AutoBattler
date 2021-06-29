using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    #region Singleton
    private static EventManager _instance;
    public static EventManager Instance { get { return _instance; } }

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
    /*
    // Combat start is when combat is initiated by the player after setting up the board (the very beginning of combat before any unit has taken an action)
    public event Action OnCombatStart;
    public void CombatStart()
    {
        if (OnCombatStart != null)
        {
            OnCombatStart();
        }
    }

    // Cycle start is when the next set of turns is beginning i.e. after each unit on the board has acted and the turn order is set up for the next cycle of turns
    public event Action OnCycleStart;
    public void CycleStart()
    {
        if (OnCycleStart != null)
        {
            OnCycleStart();
        }
    }

    // Turn start is when an individual unit is starting it's turn
    public event Action OnTurnStart;
    public void TurnStart()
    {
        if (OnTurnStart != null)
        {
            OnTurnStart();
        }
    }

    // Player attack start is when the player attack is initated (does not include the attack itself). Should have events such as giving units buffs on attacks
    public event Action OnPlayerAttackStart;
    public void PlayerAttackStart()
    {
        if (OnPlayerAttackStart != null)
        {
            OnPlayerAttackStart();
        }
    }

    // NPC attack start is the same as player attack start but for NPCs
    public event Action OnNPCAttackStart;
    public void NPCAttackStart()
    {
        if (OnNPCAttackStart != null)
        {
            OnNPCAttackStart();
        }
    }

    // End of the current unit's turn (end of turn bonuses, next unit etc)
    public event Action OnEndTurn;
    public void EndTurn()
    {
        if (OnEndTurn != null)
        {
            OnEndTurn();
        }
    }

    // End of combat (resetting the player's board, buffs, etc)
    public event Action OnEndCombat;
    public void EndCombat()
    {
        if (OnEndCombat != null)
        {
            OnEndCombat();
        }
    }

    // Unit death is called when any unit dies. Includes events that buff units when one dies (does not include actual death of unit or removing from board etc)
    public event Action OnUnitDeath;
    public void UnitDeath()
    {
        if (OnUnitDeath != null)
        {
            OnUnitDeath();
        }
    }
    */



    // Combat start is when combat is initiated by the player after setting up the board (the very beginning of combat before any unit has taken an action)
    public delegate IEnumerator EventHandler();

    public EventHandler OnCombatStart;

    public IEnumerator CombatStart()
    {
        if (OnCombatStart != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnCombatStart.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke();
            }
        }
    }

    // Cycle start is when the next set of turns is beginning i.e. after each unit on the board has acted and the turn order is set up for the next cycle of turns

    public EventHandler OnCycleStart;

    public IEnumerator CycleStart()
    {
        if (OnCycleStart != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnCycleStart.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke();
            }
        }
    }


    // Debuff start happens at turn start but directly before turn start
    public EventHandler OnDebuffStart;

    // Turn start is when an individual unit is starting it's turn
    public EventHandler OnTurnStart;

    public IEnumerator TurnStart()
    {
        if (OnDebuffStart != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnDebuffStart.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke();
            }
        }

        if (OnTurnStart != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnTurnStart.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke();
            }
        }
    }

    // Player attack is the player unit's attack

    public EventHandler OnPlayerAttack;

    public IEnumerator PlayerAttack()
    {
        if (OnPlayerAttack != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnPlayerAttack.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke();
            }
        }
    }

    // NPC attack start is the same as player attack start but for NPCs

    public EventHandler OnNPCAttack;

    public IEnumerator NPCAttack()
    {
        if (OnNPCAttack != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnNPCAttack.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke();
            }
        }
    }

    // End of the current unit's turn (end of turn bonuses, next unit etc)

    public EventHandler OnEndTurn;

    public IEnumerator EndTurn()
    {
        if (OnEndTurn != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnEndTurn.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke();
            }
        }
    }

    // End of combat (resetting the player's board, buffs, etc)

    public EventHandler OnEndCombat;

    public IEnumerator EndCombat()
    {
        if (OnEndCombat != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnEndCombat.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke();
            }
        }
    }

    public delegate IEnumerator DeathEventHandler(Unit unit);

    // Unit death is called when any unit dies. Includes events that buff units when one dies (does not include actual death of unit or removing from board etc)

    public DeathEventHandler OnPlayerUnitDeath;

    public IEnumerator PlayerUnitDeath(Unit unit)
    {
        if (OnPlayerUnitDeath != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnPlayerUnitDeath.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke(unit);
            }
        }
    }

    public DeathEventHandler OnNPCUnitDeath;

    public IEnumerator NPCUnitDeath(Unit unit)
    {
        if (OnNPCUnitDeath != null)
        {
            //Invokes all planned actions and waits until they're done.
            foreach (var @delegate in OnNPCUnitDeath.GetInvocationList())
            {
                yield return @delegate.DynamicInvoke(unit);
            }
        }
    }

    public event Action OnPrePlayerBoardReorganize;

    public event Action OnPlayerBoardReorganize;

    public void PlayerBoardReorganize()
    {
        if (OnPrePlayerBoardReorganize != null)
        {
            OnPrePlayerBoardReorganize();
        }

        if (OnPlayerBoardReorganize != null)
        {
            OnPlayerBoardReorganize();
        }        
    }

    public event Action OnPreNPCBoardReorganize;

    public event Action OnNPCBoardReorganize;

    public void NPCBoardReorganize()
    {
        if (OnPreNPCBoardReorganize != null)
        {
            OnPreNPCBoardReorganize();
        }

        if (OnNPCBoardReorganize != null)
        {
            OnNPCBoardReorganize();
        }
    }
}
