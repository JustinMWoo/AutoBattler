using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapAnimator : MonoBehaviour
{
    //public LeanTweenType easeType;

    private bool isMoving = false;

    private LTDescr curAnim;
    public bool IsMapMoving()
    {
        return isMoving;
    }

    public bool OpenMap()
    {
        if (isMoving)
        {
            return false;
        }

        if (curAnim != null)
        {
            LeanTween.cancel(curAnim.uniqueId);
        }

        isMoving = true;
        gameObject.SetActive(true);
        //curAnim= LeanTween.moveLocalY(gameObject, 0f, 2f).setEaseInOutCubic().setOnComplete(Done);
        transform.DOLocalMoveY(0f, 2f).SetEase(Ease.InOutCubic).OnComplete(() => { Done(); });

        return true;

    }
    public bool CloseMap()
    {
        if (isMoving)
        {
            return false;
        }

        if (curAnim != null)
        {
            LeanTween.cancel(curAnim.uniqueId);
        }

        isMoving = true;
        //curAnim = LeanTween.moveLocalY(gameObject, 1200f, 2f).setEaseInOutCubic().setOnComplete(Deactivate);
        transform.DOLocalMoveY(1200f, 2f).SetEase(Ease.InOutCubic).OnComplete(() => { Deactivate(); });
        return true;

    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
        isMoving = false;
    }
    private void Done()
    {
        isMoving = false;
    }
}
