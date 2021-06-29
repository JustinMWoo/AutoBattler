using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Singleton
    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }

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

    protected Vector3 destination;
    protected int speed = 5;
    protected bool moving;

    //public GameObject lookTarget;

    private void Start()
    {
        moving = false;
    }

    public void MoveCamera(Vector3 destination, int smoothAmount, float duration)
    {
        StartCoroutine(MoveTo(destination, smoothAmount, duration, Camera.main.transform.rotation));
    }
    public void MoveCamera(Vector3 destination, int smoothAmount, float duration, Quaternion quaternion)
    {
        StartCoroutine(MoveTo(destination, smoothAmount, duration, quaternion));
    }

    IEnumerator MoveTo(Vector3 destination, int smoothAmount, float duration, Quaternion quaternion)
    {
        if (moving)
        {
            yield break;
        }
        moving = true;

        Vector3 startPos = Camera.main.transform.position;
        float time = 0;
        //float debugTime = 0;
        while (time <= 1)
        {
            time += Time.deltaTime / duration;

            //debugTime += Time.deltaTime;
            // Debug.Log(debugTime);

            float smooth = time;

            // Smooth step multiple times depending on smooth amount
            for (int i = 0; i < smoothAmount; i++)
            {
                smooth = Mathf.SmoothStep(0, 1, smooth);
            }

            Camera.main.transform.position = Vector3.Lerp(startPos, destination, smooth);

            //Camera.main.transform.LookAt(lookTarget.transform);
            yield return null;
        }
        moving = false;
    }
}