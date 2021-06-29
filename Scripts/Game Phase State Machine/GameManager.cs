using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

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


    protected Phase phase;

    private void Start()
    {
        StartGame();
    }

    public void SetPhase(Phase phase)
    {
        this.phase = phase;

        Debug.Log("Starting phase..." + phase);
        StartCoroutine(phase.Start());
    }
    public Phase GetPhase()
    {
        return phase;
    }

    public void StartGame()
    {
        PlayerManager.Instance.Tier = 0;
        PlayerManager.Instance.Currency = 200;
        PlayerManager.Instance.CurNumUnits = 0;
        PlayerManager.Instance.MaxUnits = 5;
        SetPhase(new BuyingPhase());
    }

    public void EndCurrent()
    {
        StartCoroutine(phase.End());
    }

}
