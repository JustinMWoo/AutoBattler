using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit : MonoBehaviour
{
    public GameObject healthBar;
    public bool Moving { get; set; }
    public GameObject Model { get; set; }
    public UnitData UnitData { get; set; }
    public BasicAttack AttackType { get; set; }
    private List<UnitAction> TurnStart = new List<UnitAction>();
    private List<UnitAction> TurnEnd = new List<UnitAction>();
    private List<UnitAction> OnHealthChanged = new List<UnitAction>();
    private List<UnitAction> onDeath = new List<UnitAction>();

    private List<AttackModifier> AttackModifiers = new List<AttackModifier>();
    private List<DefenceModifier> DefenceModifiers = new List<DefenceModifier>();

    public BaseSet Set { get; set; }
    public BaseTribe Tribe { get; set; }
    public int Level { get; set; }

    [SerializeField]
    private int currentHealth;

    public CharacterStat speed, damage, maxHealth, defence;

    [SerializeField]
    private bool playerUnit = true;

    public Animator animator;

    private Vector3 destination;
    private int boardMoveSpeed = 10;

    private Vector3 modelScale;
    private void Start()
    {
        // Add each unit to have its current health set to maximum when combat starts
        EventManager.Instance.OnCombatStart += ResetCurrentHealth;
        EventManager.Instance.OnEndCombat += ResetModifiers;
        healthBar.SetActive(false);

        modelScale = Model.transform.localScale;
        Moving = false;
    }
    private void Update()
    {
        if (destination != Vector3.zero)
        {
            Moving = true;
            float step = boardMoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
            if (transform.position == destination)
            {
                destination = Vector3.zero;
                Moving = false;
                //Debug.Log("Destination reached");
            }
        }
    }


    #region Getters/Setters
    public bool IsPlayerUnit()
    {
        return playerUnit;
    }
    public void SetAsNPC()
    {
        playerUnit = false;
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public void AddTurnStart(UnitAction action)
    {
        TurnStart.Add(action);
    }
    public void AddTurnEnd(UnitAction action)
    {
        TurnEnd.Add(action);
    }
    public void AddHealthChanged(UnitAction action)
    {
        OnHealthChanged.Add(action);
    }
    public void AddOnDeath(UnitAction action)
    {
        onDeath.Add(action);
    }
    public void RemoveTurnStart(UnitAction action)
    {
        TurnStart.Remove(action);
    }
    public void RemoveTurnEnd(UnitAction action)
    {
        TurnEnd.Remove(action);
    }
    public void RemoveHealthChanged(UnitAction action)
    {
        OnHealthChanged.Remove(action);
    }
    public void RemoveOnDeath(UnitAction action)
    {
        onDeath.Remove(action);
    }

    public List<AttackModifier> GetAttackModifiers()
    {
        return AttackModifiers;
    }
    public void AddAttackModifier(AttackModifier modifier)
    {
        AttackModifiers.Add(modifier);
    }
    public void RemoveAttackModifier(AttackModifier modifier)
    {
        AttackModifiers.Remove(modifier);
    }

    public List<DefenceModifier> GetDefenceModifiers()
    {
        return DefenceModifiers;
    }
    public void AddDefenceModifier(DefenceModifier modifier)
    {
        DefenceModifiers.Add(modifier);
    }
    public void RemoveDefenceModifier(DefenceModifier modifier)
    {
        DefenceModifiers.Remove(modifier);
    }

    public List<UnitAction> GetTurnStart()
    {
        return TurnStart;
    }
    #endregion

    public void SubscribeToEvents()
    {
        // Add all turn start actions as well as highlighting the tile the unit is standing on
        EventManager.Instance.OnTurnStart += HighlightUnitTile;
        foreach (UnitAction action in TurnStart)
        {
            //Debug.Log(action.gameObject.name);
            EventManager.Instance.OnTurnStart += action.Execute;
        }


        if (AttackType != null)
        {
            if (playerUnit)
            {
                EventManager.Instance.OnPlayerAttack += AttackType.Execute;
            }
            else
            {
                EventManager.Instance.OnNPCAttack += AttackType.Execute;
            }
        }

        foreach (UnitAction action in TurnEnd)
        {
            EventManager.Instance.OnEndTurn += action.Execute;
        }

    }

    public void RemoveFromEvents()
    {
        EventManager.Instance.OnTurnStart -= HighlightUnitTile;
        foreach (UnitAction action in TurnStart)
        {
            EventManager.Instance.OnTurnStart -= action.Execute;
        }
        if (AttackType != null)
        {
            if (playerUnit)
            {
                EventManager.Instance.OnPlayerAttack -= AttackType.Execute;
            }
            else
            {
                EventManager.Instance.OnNPCAttack -= AttackType.Execute;
            }
        }
        foreach (UnitAction action in TurnEnd)
        {
            EventManager.Instance.OnEndTurn -= action.Execute;
        }
    }
    public IEnumerator ResetModifiers()
    {
        AttackModifiers.Clear();
        DefenceModifiers.Clear();

        maxHealth = new CharacterStat(UnitData.health);
        damage = new CharacterStat(UnitData.damage);
        speed = new CharacterStat(UnitData.speed);
        defence = new CharacterStat(UnitData.defence);

        yield return null;
    }
    public IEnumerator ResetCurrentHealth()
    {
        currentHealth = (int)maxHealth.Value;
        healthBar.GetComponent<HealthBar>().SetMaxHealth((int)maxHealth.Value);
        yield return null;
    }

    public void Heal(int amount)
    {
        if (currentHealth + amount <= maxHealth.Value)
        {
            currentHealth += amount;
        }
        else
        {
            currentHealth = (int)maxHealth.Value;
        }

        healthBar.GetComponent<HealthBar>().SetHealth(currentHealth);

        foreach (UnitAction action in OnHealthChanged)
        {
            StartCoroutine(action.Execute());
        }
    }

    // Subtract damage from the unit's health and check if it dies
    public void TakeDamage(int damage)
    {
        healthBar.SetActive(true);

        currentHealth -= damage;
        healthBar.GetComponent<HealthBar>().SetHealth(currentHealth);
        foreach (UnitAction action in OnHealthChanged)
        {
            StartCoroutine(action.Execute());
        }

        if (currentHealth <= 0)
        {
            TurnManager.Instance.WaitDeathAnimation(StartCoroutine(Die()));
        }
    }

    // Play death animation, remove unit from the board and turn manager and remove from events
    public IEnumerator Die()
    {
        Stack animations = new Stack();

        TurnManager.Instance.RemoveUnitFromCycle(this);
        animator.SetBool("Die", true);

        // 0.26 for the transition between animations
        yield return new WaitForSeconds(0.26f);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.26f);

        // Run actions for when unit dies
        foreach (UnitAction action in onDeath)
        {
            animations.Push(StartCoroutine(action.Execute()));
        }

        if (playerUnit)
        {
            animations.Push(StartCoroutine(EventManager.Instance.PlayerUnitDeath(this)));
        }
        else
        {
            animations.Push(StartCoroutine(EventManager.Instance.NPCUnitDeath(this)));
        }

        Model.transform.localScale = Vector3.zero;
        // Model.SetActive(false);
        healthBar.SetActive(false);

        while (animations.Count > 0)
        {
            var animation = animations.Pop();
            yield return animation;
        }

        if (playerUnit)
        {
            Board.Instance.RemovePlayerUnit(this);
        }
        else
        {
            Board.Instance.RemoveNPCUnit(this);
        }

        if (TurnManager.Instance.GetCurrentUnit() == this)
        {
            RemoveFromEvents();
        }

        // Wait for units to reposition
        yield return new WaitForSeconds(0.3f);

        //TurnManager.Instance.pause = false;

       
    }

    public void ShowUnit()
    {
        animator.SetBool("Die", false);

        // use scale to prevent animation problems
        Model.transform.localScale = modelScale;

        healthBar.SetActive(false);
    }

    // Move unit to new location
    public void MoveTo(Vector3 newPos)
    {
        destination = newPos;
    }

    public IEnumerator HighlightUnitTile()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, 3, transform.position.z), Vector3.down, out hit, Mathf.Infinity, Board.Instance.GetTileLayerMask()) && hit.transform.CompareTag("tile"))
        {
            Tile tile = hit.collider.gameObject.GetComponent<Tile>();
            if (tile != null)
            {
                tile.SetUnitTurn();
            }
            else
            {
                Debug.LogError("Tile not found");
            }
        }
        else
        {
            Debug.LogError("");
        }
        yield return null;
    }
    private void OnDestroy()
    {
        EventManager.Instance.OnCombatStart -= ResetCurrentHealth;
        EventManager.Instance.OnEndCombat -= ResetModifiers;
    }
}
