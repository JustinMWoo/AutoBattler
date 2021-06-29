using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPhase : Phase
{
    public override IEnumerator Start()
    {
        MapManager.Instance.CloseMap();
        ButtonManager.Instance.EnableButton(Buttons.Map);
        ButtonManager.Instance.EnableButton(Buttons.Done);
        ButtonManager.Instance.EnableButton(Buttons.TierUp);
        ButtonManager.Instance.EnableButton(Buttons.Refresh);
        ButtonManager.Instance.EnableButton(Buttons.MaxUnits);
        ButtonManager.Instance.DisableButton(Buttons.Combat);
        CameraController.Instance.MoveCamera(new Vector3(3, 8.5f, -1f), 3, 3);
        yield return GameManager.Instance.StartCoroutine(SceneAnimationManager.Instance.ActivateShopChoices());
        UnitChoiceManager.Instance.SpawnShopChoices();
    }

    public override IEnumerator End()
    {
        if (!MapManager.Instance.mapAnimator.IsMapMoving())
        {
            ButtonManager.Instance.DisableButton(Buttons.TierUp);
            ButtonManager.Instance.DisableButton(Buttons.Refresh);
            ButtonManager.Instance.DisableButton(Buttons.MaxUnits);

            UnitChoiceManager.Instance.RemoveCurrentChoices();
            yield return GameManager.Instance.StartCoroutine(SceneAnimationManager.Instance.DeactivateShopChoices());
            GameManager.Instance.SetPhase(new MapPhase());
            yield break;
        }
    }
}
