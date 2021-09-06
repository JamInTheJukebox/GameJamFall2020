using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterUI : MonoBehaviour
{
    [SerializeField] AbilityWheel AbilitySlots;
    [SerializeField] MinigameRulesUI rulesUI;       // this might change in the future to contain all rules, not just UI.
                                                    // future possible rules: Controls, what controls are enabled during the minigame?
    [SerializeField] GamestateUI gameState;
    [SerializeField] TimerUI globalTimer;
        // serialize field for Pause menu

    private void Awake()
    {
        SetUIRules(rulesUI);
    }
    public void ChangeCurrentAbility(int Delta)
    {

    }

    public void SetUIRules(MinigameRulesUI ruleUI)
    {
        var playerPanelRule = rulesUI.GetPlayerPanelInfo();
        var timePanelRule = rulesUI.GetGlobalTimerInfo();
        gameState.SetPlayerInfo(playerPanelRule);
        globalTimer.SetTimerUIInfo(timePanelRule);
    }

}
