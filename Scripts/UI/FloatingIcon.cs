using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingIcon : MonoBehaviour
{
    public float moveYSpeed = 1f;
    public float distance = 1f;

    public float DestroyTime = 1f;

    [Tooltip("If set to false then will drop down")]
    public bool FloatUp = true;

    private Vector3 destination;

    private SpriteRenderer sprite;

    [SerializeField]
    private TextMeshProUGUI text;
    private float startTime;

    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, DestroyTime);

        if (FloatUp)
        {
            destination = new Vector3(transform.position.x, transform.position.y, transform.position.z) + Camera.main.transform.up * distance;
        }
        else
        {
            destination = new Vector3(transform.position.x, transform.position.y, transform.position.z) - Camera.main.transform.up * distance;
        }
        sprite = GetComponent<SpriteRenderer>();
        
        startTime = Time.time;

        if (text != null)
        {
            text.enabled = false;
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, destination, moveYSpeed * Time.deltaTime);

        // Fade out icon
        float t = (Time.time - startTime) / DestroyTime;

        if (sprite != null)
        {
            sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(1f, 0f, t));
        }

        if (text != null)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.SmoothStep(1f, 0f, t));
        }
    }

    public void SetText(string newText)
    {
        if (text != null)
        {
            text.enabled = true;
            text.text = newText;
        }
    }

}
