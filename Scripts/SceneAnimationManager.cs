using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAnimationManager : MonoBehaviour
{
    #region Singleton
    private static SceneAnimationManager _instance;
    public static SceneAnimationManager Instance { get { return _instance; } }

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
    [SerializeField]
    protected GameObject enemyBoard;
    [SerializeField]
    protected GameObject[] basicChoices;
    [SerializeField]
    protected GameObject[] shopChoices;


    // The y value that objects go to to be deactivated
    public int deactivatedY = 30;

    private int animationCount = 0;

    private List<LTDescr> animations = new List<LTDescr>();

    public bool IsMoving()
    {
        if (animationCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ActivateEnemyBoard()
    {
        LeanTween.moveLocalY(enemyBoard, 0, 3).setEaseInOutCubic();
    }
    public void DeactivateEnemyBoard()
    {
        LeanTween.moveLocalY(enemyBoard, deactivatedY, 3).setEaseInOutCubic();
    }
    public IEnumerator ActivateBasicChoices()
    {
        animationCount = 0;
        foreach (LTDescr anim in animations)
        {
            LeanTween.cancel(anim.uniqueId);
        }
        animations = new List<LTDescr>();
        for (int i = 0; i < basicChoices.Length; i++)
        {
            int x = i;
            animationCount++;
            animations.Add(LeanTween.delayedCall(i * 0.25f, () =>
            {
                LeanTween.moveLocalY(basicChoices[x], 0, 3).setEaseInOutCubic().setOnComplete(DecreaseAnimationCount);
            }));
        }
        yield return null;
    }

    public IEnumerator DeactivateBasicChoices()
    {
        animationCount = 0;
        foreach (LTDescr anim in animations)
        {
            LeanTween.cancel(anim.uniqueId);
        }
        animations = new List<LTDescr>();
        for (int i = 0; i < basicChoices.Length; i++)
        {
            int x = i;
            animationCount++;
            animations.Add(LeanTween.delayedCall(i * 0.25f, () =>
            {
                LeanTween.moveLocalY(basicChoices[x], deactivatedY, 3).setEaseInOutCubic().setOnComplete(DecreaseAnimationCount);
            }));
        }
        yield return null;
    }

    public IEnumerator ActivateShopChoices()
    {
        animationCount = 0;
        foreach (LTDescr anim in animations)
        {
            LeanTween.cancel(anim.uniqueId);
        }
        animations = new List<LTDescr>();
        for (int i = 0; i < shopChoices.Length; i++)
        {
            int x = i;
            animationCount++;
            animations.Add(LeanTween.delayedCall(i * 0.25f, () =>
            {
                LeanTween.moveLocalY(shopChoices[x], 0, 3).setEaseInOutCubic().setOnComplete(DecreaseAnimationCount).setOnComplete(DecreaseAnimationCount);
            }));
        }
        yield return null;
    }

    public IEnumerator DeactivateShopChoices()
    {
        animationCount = 0;
        foreach (LTDescr anim in animations)
        {
            LeanTween.cancel(anim.uniqueId);
        }
        animations = new List<LTDescr>();
        for (int i = 0; i < shopChoices.Length; i++)
        {
            int x = i;
            animationCount++;
            animations.Add(LeanTween.delayedCall(i * 0.25f, () =>
            {
                LeanTween.moveLocalY(shopChoices[x], deactivatedY, 3).setEaseInOutCubic().setOnComplete(DecreaseAnimationCount).setOnComplete(DecreaseAnimationCount);
            }));
        }
        yield return null;
    }

    private void DecreaseAnimationCount()
    {
        animationCount--;
    }
}