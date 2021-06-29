using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Color defColor;
    private Renderer tileRenderer;

    private void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        defColor = tileRenderer.material.color;
    }

    public void SetAttackTarget()
    {
        tileRenderer.material.color = Color.red;
        EventManager.Instance.OnEndTurn += ResetColor;
    }

    public IEnumerator ResetColor()
    {
        tileRenderer.material.color = defColor;
        EventManager.Instance.OnEndTurn -= ResetColor;
        yield return null;
    }

    public void SetUnitTurn()
    {
        tileRenderer.material.color = Color.green;
        EventManager.Instance.OnEndTurn += ResetColor;
    }
}
