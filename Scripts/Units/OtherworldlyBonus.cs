using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherworldlyBonus 
{
    private float percentStolen;
    private GameObject deathAnimationPrefab;

    public OtherworldlyBonus(float percentage, GameObject effectPrefab)
    {
        percentStolen = percentage;
        deathAnimationPrefab = effectPrefab;

        EventManager.Instance.OnEndCombat += RemoveBonus;
    }

    public IEnumerator ApplyBonus(Unit deadUnit)
    {
        Unit curUnit = TurnManager.Instance.GetCurrentUnit();

        // If the unit that killed is not from this tribe
        if (!(curUnit.Tribe is OtherworldlyTribe))
        {
            yield break;
        }

        GameObject effect = GameObject.Instantiate(deathAnimationPrefab, deadUnit.transform.position, Quaternion.identity);
        effect.GetComponentInChildren<ParticleSystemForceField>().gameObject.transform.position = curUnit.transform.position;

        int health = (int)(deadUnit.maxHealth.Value * (percentStolen / 100));
        int damage = (int)(deadUnit.damage.Value * (percentStolen / 100));
        int speed = (int)(deadUnit.speed.Value * (percentStolen / 100));
        int defence = (int)(deadUnit.defence.Value * (percentStolen / 100));

        curUnit.maxHealth.AddModifier(new StatModifier(health, StatModType.Flat));
        curUnit.Heal(health);
        yield return FloatingStatChangeSpawner.Instance.StartCoroutine(FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(curUnit.transform, health.ToString(), Stat.health, true));

        curUnit.damage.AddModifier(new StatModifier(damage, StatModType.Flat));
        yield return FloatingStatChangeSpawner.Instance.StartCoroutine(FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(curUnit.transform, damage.ToString(), Stat.damage, true));

        curUnit.speed.AddModifier(new StatModifier(speed, StatModType.Flat));
        yield return FloatingStatChangeSpawner.Instance.StartCoroutine(FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(curUnit.transform, speed.ToString(), Stat.speed, true));

        curUnit.defence.AddModifier(new StatModifier(defence, StatModType.Flat));
        yield return FloatingStatChangeSpawner.Instance.StartCoroutine(FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(curUnit.transform, defence.ToString(), Stat.defence, true));

        //Debug.Log("New stats, hp: " + curUnit.maxHealth.Value + " damage: " + curUnit.damage.Value + " speed: " + curUnit.speed.Value + " defence: " + curUnit.defence.Value);

        yield return null;
    }

    public IEnumerator RemoveBonus()
    {
        EventManager.Instance.OnNPCUnitDeath -= ApplyBonus;
        EventManager.Instance.OnPlayerUnitDeath -= ApplyBonus;
        yield return null;
    }
}
