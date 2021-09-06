using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using minigameEnumerators;

[CreateAssetMenu(fileName = "New Minigame UI", menuName = "ScriptableObjects/Minigame/MinigameUI", order = 1)]
public class MinigameRulesUI : ScriptableObject
{
    [Header("Player Panel Info")]
    [SerializeField] PlayerPanelOrientation _playerPanelOrientation = PlayerPanelOrientation.top_left;
    [SerializeField] ObjectiveItem _objectiveItem = ObjectiveItem.off;

    [Header("Global Timer Info")]
    [SerializeField] GlobalTimeOrientation _globalTimeOrientation = GlobalTimeOrientation.top;
    [SerializeField] GlobalTimeCountingMode _globalTimeCountingMode;
    [SerializeField] [Tooltip("Count up: Minigame ends automatically after it reaches this point.\n Count Down: Minigame starts counting down from this time")]
    float MaxTime;
    [SerializeField] bool useMinutes;
    [SerializeField] bool useSeconds = true;
    [SerializeField] bool useMilliseconds;
    public PlayerPanelInfo GetPlayerPanelInfo()
    {
        return new PlayerPanelInfo(_playerPanelOrientation, _objectiveItem);
    }
    public GlobalTimerInfo GetGlobalTimerInfo()
    {
        return new GlobalTimerInfo(_globalTimeOrientation, _globalTimeCountingMode, MaxTime, useMinutes, useSeconds, useMilliseconds);
    }
}
