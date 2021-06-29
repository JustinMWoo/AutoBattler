using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAction : MonoBehaviour
{
    protected Unit unit;
    protected LayerMask layerMask;
    public abstract IEnumerator Execute();

    protected virtual void Awake()
    {
        unit = GetComponentInParent<Unit>();
        layerMask = Board.Instance.GetTileLayerMask();
    }

    protected IEnumerator AttackAnimation()
    {
        unit.animator.SetBool("Attacking", true);

        // 0.26 for the transition between animations
        yield return new WaitForSeconds(0.26f);

        yield return new WaitForSeconds(unit.animator.GetCurrentAnimatorStateInfo(0).length - 0.26f);

        unit.animator.SetBool("Attacking", false);
    }

    protected virtual IEnumerator AbilityAnimation()
    {
        unit.animator.SetBool("Ability", true);
        // 0.26 for the transition between animations
        yield return new WaitForSeconds(0.26f);

        yield return new WaitForSeconds(unit.animator.GetCurrentAnimatorStateInfo(0).length - 0.26f);

        unit.animator.SetBool("Ability", false);
    }

    protected virtual void HighlightTargetTile(Unit target)
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(target.transform.position.x, 3, target.transform.position.z), Vector3.down, out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
        {
            Tile tile = hit.collider.gameObject.GetComponent<Tile>();
            if (tile != null)
            {
                tile.SetAttackTarget();
            }
        }
    }
}
