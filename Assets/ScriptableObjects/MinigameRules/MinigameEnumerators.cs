namespace minigameEnumerators
{
    #region  PlayerPanel
    public enum PlayerPanelOrientation
    {
        off = 0,
        top = 1,
        top_left = 2,
        bottom = 3,
        corners = 4
    }

    public enum ObjectiveItem
    {
        off = 0,
        COIN = 1,
        // add more items here.
    }
    public class PlayerPanelInfo
    {
        public PlayerPanelInfo(PlayerPanelOrientation _orientation, ObjectiveItem _item)
        {
            _playerPanel = _orientation;
            _objectiveItem = _item;
        }
        PlayerPanelOrientation _playerPanel;
        ObjectiveItem _objectiveItem;

        public PlayerPanelOrientation getOrientation()
        {
            return _playerPanel;
        }

        public ObjectiveItem getObjectiveItem()
        {
            return _objectiveItem;
        }
    }
    #endregion
    #region  GlobalTime
    public enum GlobalTimeOrientation
    {
        off = 0,
        top = 1,
        bottom = 2
    }

    public enum GlobalTimeCountingMode
    {
        CountingUP = 1,
        CountingDown = -1,
    }

    public class GlobalTimerInfo
    {
        public GlobalTimerInfo(GlobalTimeOrientation _orientation, GlobalTimeCountingMode _countingMode, float _maxTime, bool _min, bool _sec, bool _millisec)
        {
            _globalTimeOrientation = _orientation;
            _globalTimeCountingMode = _countingMode;
            MaxTime = _maxTime;
            useMinutes = _min;
            useSeconds = _sec;
            useMilliseconds = _millisec;
        }
        GlobalTimeOrientation _globalTimeOrientation;
        GlobalTimeCountingMode _globalTimeCountingMode;
        float MaxTime;  // Counting up: Counts up to this time and then stops the minigame.
                        // counting down: Starts at this time.
        bool useMinutes;
        bool useSeconds;
        bool useMilliseconds;

        public GlobalTimeOrientation GetTimeOrientation()
        {
            return _globalTimeOrientation;
        }

        public GlobalTimeCountingMode GetTimeMode()
        {
            return _globalTimeCountingMode;
        }
        public float GetMaxTime()
        {
            return MaxTime;
        }
        public bool GetUseMinutes()
        {
            return useMinutes;
        }
        public bool GetUseSeconds()
        {
            return useSeconds;
        }
        public bool GetUseMilliseconds()
        {
            return useMilliseconds;
        }
    }
    #endregion
}
