using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFadeInOut : MonoBehaviour
{
    private MeshRenderer rend;
    private float t = 0;
    private bool fadeOut = false;

    public float duration = 2;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (t > duration / 2)
        {
            if (!fadeOut)
            {
                fadeOut = true;
                t = 0;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        t += Time.deltaTime / (duration / 2);
        if (!fadeOut)
        {
            rend.material.color = Color.Lerp(Color.clear, new Color(1, 1, 1, 0.5f), t);
        }
        else
        {
            rend.material.color = Color.Lerp(new Color(1, 1, 1, 0.5f), Color.clear, t);
        }
    }
}
