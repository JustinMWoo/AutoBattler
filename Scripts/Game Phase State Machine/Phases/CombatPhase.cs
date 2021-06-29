using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPhase : Phase
{
    public override IEnumerator Start()
    {
        MapManager.Instance.CloseMap();
        ButtonManager.Instance.EnableButton(Buttons.Map);
        ButtonManager.Instance.DisableButton(Buttons.Done);
        ButtonManager.Instance.EnableButton(Buttons.Combat);
        CameraController.Instance.MoveCamera(new Vector3(3, 8.5f, 3f), 3, 3);
        // Load NPC units
        NPCBoardLoader.Instance.LoadBoard(MapManager.Instance.GetCurrentNode().Layout);
        yield return new WaitForSeconds(0.25f);
        SceneAnimationManager.Instance.ActivateEnemyBoard();

    }

    public override IEnumerator End()
    {
        bool playerWin = false;
        if (Board.Instance.NPCUnitCount() == 0)
        {
            playerWin = true;
        }

        if (playerWin)
        {
            TurnManager.Instance.CombatWinDisplay(true);
        }
        else
        {
            TurnManager.Instance.CombatLossDisplay(true);
        }

        yield return new WaitForSeconds(1);

        SceneAnimationManager.Instance.DeactivateEnemyBoard();
        Board.Instance.ResetBoardAfterCombat();

        TurnManager.Instance.CombatWinDisplay(false);
        TurnManager.Instance.CombatLossDisplay(false);

        if (playerWin)
        {
            PlayerManager.Instance.Currency += 3;
            GameManager.Instance.SetPhase(new BuyingPhase());
        }
        else
        {
            GameManager.Instance.SetPhase(new LossPhase());
        }
    }
}