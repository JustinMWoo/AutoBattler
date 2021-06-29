using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyingPhase : Phase
{
    public override IEnumerator Start()
    {
        ButtonManager.Instance.EnableButton(Buttons.Map);
        ButtonManager.Instance.EnableButton(Buttons.Done);
        ButtonManager.Instance.DisableButton(Buttons.Combat);
        CameraController.Instance.MoveCamera(new Vector3(3, 6.5f, 0f), 3, 3);
        yield return GameManager.Instance.StartCoroutine(SceneAnimationManager.Instance.ActivateBasicChoices());
        UnitChoiceManager.Instance.SpawnBasicChoices();
    }

    public override IEnumerator End()
    {
        if (!MapManager.Instance.mapAnimator.IsMapMoving())
        {
            UnitChoiceManager.Instance.RemoveCurrentChoices();
            yield return GameManager.Instance.StartCoroutine(SceneAnimationManager.Instance.DeactivateBasicChoices());
            GameManager.Instance.SetPhase(new MapPhase());
            yield break;
        }
    }
}