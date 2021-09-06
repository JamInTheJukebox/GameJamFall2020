using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using minigameEnumerators;

public class GamestateUI : MonoBehaviour
{
    [SerializeField] GameObject[] PlayerUI = new GameObject[4];
    private Dictionary<GameObject, PlayerPanel> _panels = new Dictionary<GameObject, PlayerPanel>();
    [SerializeField] Transform[] PlayerPanelPositions = new Transform[4];
    Vector2[] CornerOrientationAnchorValues = new Vector2[4]
    {
        Vector2.up,Vector2.one,Vector2.zero,Vector2.right
    };

    [SerializeField] GameObject GlobalTimer;
    private void Awake()
    {
        foreach (GameObject p in PlayerUI)
            _panels.Add(p, p.GetComponent<PlayerPanel>());
    }
    int debugCount = 0;
    private void Update()
    {
        debugCount = Mathf.Clamp(debugCount, 0, 3);
        if (Movement.PlayerInput.JumpTriggered() && debugCount == 0)
        {
            SetPlayerPanelOrientation(PlayerPanelOrientation.bottom);
        }
        else if (Movement.PlayerInput.JumpTriggered() && debugCount == 1)
        {
            SetPlayerPanelOrientation(PlayerPanelOrientation.top);
        }
        else if(Movement.PlayerInput.JumpTriggered() && debugCount == 2)
        {
            SetPlayerPanelOrientation(PlayerPanelOrientation.top_left);
        }
        else if(Movement.PlayerInput.JumpTriggered() && debugCount == 3)
        {
            SetPlayerPanelOrientation(PlayerPanelOrientation.corners);
        }
        if (Movement.PlayerInput.JumpTriggered())
        {
            debugCount++;
            if(debugCount > 3) { debugCount = 0; }
        }
    }
    #region Player Panel
    public void SetPlayerInfo(PlayerPanelInfo playerInfo)
    {
        if(playerInfo.getOrientation() == 0) { SetPlayerPanelActivity(false); return; }
        SetPlayerPanelOrientation(playerInfo.getOrientation());
        foreach(GameObject p in PlayerUI)
        {
            if (p.activeInHierarchy)
                _panels[p].SetObjectiveItemInfo(playerInfo.getObjectiveItem());
        }
    }

    public void SetPlayerPanelOrientation(PlayerPanelOrientation orientation)
    {
        if((int)orientation != 4)
        {
            for (int i = 0; i < PlayerUI.Length; i++)
            {
                var rectP = PlayerUI[i].GetComponent<RectTransform>();
                rectP.pivot = 0.5f*Vector2.one;
            }
        }

        SetPlayerPanelParent(PlayerPanelPositions[(int)orientation - 1]);

        if ((int)orientation == 4)
        {
            for(int i = 0; i < PlayerUI.Length; i++)
            {
                var rectP = PlayerUI[i].GetComponent<RectTransform>();
                rectP.anchorMin = CornerOrientationAnchorValues[i];
                rectP.anchorMax = CornerOrientationAnchorValues[i];
                rectP.pivot = CornerOrientationAnchorValues[i];
                rectP.anchoredPosition = Vector3.zero;
            }
        }
    }

    private void SetPlayerPanelParent(Transform parent)
    {
        foreach(GameObject p in PlayerUI)
        {
            p.transform.parent = parent;
        }
    }

    public void SetPlayerPanelActivity(bool activity)
    {
        foreach (GameObject p in PlayerUI)
        {
            p.SetActive(activity);
        }
    }

    #endregion

    public void SetGlobalTimeOrientation(int orientation)
    {

    }
}
