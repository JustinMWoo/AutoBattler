using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator : MonoBehaviour
{
    [SerializeField]
    private GameObject FloatingDamageText;
    #region Singleton
    private static DamageCalculator _instance;
    public static DamageCalculator Instance { get { return _instance; } }

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

    public int CalculateDamage(Unit attacker, Unit defender)
    {
        //Debug.Log((int)(attacker.damage.Value - defender.defence.Value));
        foreach (AttackModifier mod in attacker.GetAttackModifiers())
        {
            mod.Calculate(defender);
        }

        int damage = (int)attacker.damage.Value;
        foreach (DefenceModifier mod in defender.GetDefenceModifiers())
        {
            damage = mod.Calculate(damage);
        }
        
        damage = (int)(damage - defender.defence.Value);
        if (damage < 0)
        {
            damage = 0;
        }

        Instantiate(FloatingDamageText, defender.transform.position, Quaternion.identity).GetComponent<FloatingIcon>().SetText(damage.ToString());

        foreach (AttackModifier mod in attacker.GetAttackModifiers())
        {
            mod.RemoveModifier();
        }

        return damage;
    }

    public int CalculateDamage(Unit attacker, Unit defender, float multiplier)
    {
        //Debug.Log((int)(attacker.damage.Value - defender.defence.Value));
        foreach (AttackModifier mod in attacker.GetAttackModifiers())
        {
            mod.Calculate(defender);
        }

        int damage = (int)(attacker.damage.Value * multiplier);
        foreach (DefenceModifier mod in defender.GetDefenceModifiers())
        {
            damage = mod.Calculate(damage);
        }

        damage = (int)(damage - defender.defence.Value);
        if (damage < 0)
        {
            damage = 0;
        }
        Instantiate(FloatingDamageText, defender.transform.position, Quaternion.identity).GetComponent<FloatingIcon>().SetText(damage.ToString());

        foreach (AttackModifier mod in attacker.GetAttackModifiers())
        {
            mod.RemoveModifier();
        }
        return damage;
    }

    /*
     * Calculates damage from a flat amount ignoring modifiers
     */
    public int CalculateDamage(Unit defender, int damage)
    {
        damage = (int)(damage - defender.defence.Value);
        if (damage < 0)
        {
            damage = 0;
        }
        Instantiate(FloatingDamageText, defender.transform.position, Quaternion.identity).GetComponent<FloatingIcon>().SetText(damage.ToString());
        return damage;
    }

    public void DamageText(Unit defender, int damage)
    {
        Instantiate(FloatingDamageText, defender.transform.position, Quaternion.identity).GetComponent<FloatingIcon>().SetText(damage.ToString());
    }
}
