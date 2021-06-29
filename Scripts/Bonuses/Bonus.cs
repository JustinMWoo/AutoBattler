using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bonus : ScriptableObject
{
    [TextArea(15, 20)]
    public string description;
    public List<GameObject> buffPrefabs;
    public Sprite icon;

    // Amount of units from the set required for bonuses
    [SerializeField]
    protected List<int> tierRequirements;

    protected IEnumerator SpawnBuffPrefab(Transform unit)
    {
        RaycastHit hit;
        Vector3 newPos = new Vector3(unit.transform.position.x, 3, unit.transform.position.z);
        if (Physics.Raycast(newPos, Vector3.down, out hit, Mathf.Infinity))
        {
            newPos = new Vector3(unit.position.x, hit.point.y, unit.position.z);
        }
        Instantiate(buffPrefabs[0], newPos, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
    }

    protected IEnumerator SpawnBuffPrefab(Transform unit, string text)
    {
        RaycastHit hit;
        Vector3 newPos = new Vector3(unit.transform.position.x, 3, unit.transform.position.z);
        if (Physics.Raycast(newPos, Vector3.down, out hit, Mathf.Infinity))
        {
            newPos = new Vector3(unit.position.x, hit.point.y, unit.position.z);
        }
        Instantiate(buffPrefabs[0], newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text);
        yield return new WaitForSeconds(0.5f);
    }

    protected IEnumerator SpawnBuffPrefabs(Transform unit, List<string> text)
    {
        RaycastHit hit;
        Vector3 newPos = new Vector3(unit.transform.position.x, 3, unit.transform.position.z);
        if (Physics.Raycast(newPos, Vector3.down, out hit, Mathf.Infinity))
        {
            newPos = new Vector3(unit.position.x, hit.point.y, unit.position.z);
        }
        for (int i = 0; i < buffPrefabs.Count; i++)
        {
            if (text[i] != null)
            {
                Instantiate(buffPrefabs[i], newPos, Quaternion.identity).GetComponent<FloatingIcon>().SetText(text[i]);
            }
            else
            {
                Instantiate(buffPrefabs[i], newPos, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.5f);
        }
        
    }
    public abstract IEnumerator ApplyBonus(int count, bool player);
    public virtual void RemoveBonus(bool player)
    {

    }

    public List<int> GetTierRequirements()
    {
        return tierRequirements;
    }
}
