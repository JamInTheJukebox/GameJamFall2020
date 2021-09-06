using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using minigameEnumerators;

public class TimerUI : MonoBehaviour
{
    [SerializeField] GameObject Parent;
    public bool stopTimer = false;
    private TextMeshProUGUI TimerText;
    float MaxTime;
    bool countMinutes;
    bool countSeconds;
    bool countMilliseconds;

    private int TimeScale;
    private string m_TimeElapsed;
    public string TimeElapsed
    {
        get
        {
            return m_TimeElapsed;
        }
        private set
        {
            m_TimeElapsed = value;
            if(TimerText != null)
                TimerText.text = m_TimeElapsed;
        }
    }
    private float m_CurrentTime;
    public float CurrentTime
    {
        get
        {
            return m_CurrentTime;
        }
        private set
        {
            m_CurrentTime = value;
            TimeElapsed = UpdateTime(m_CurrentTime);
        }
    }

    private void Awake()
    {
        TimerText = GetComponent<TextMeshProUGUI>();
    }

    public void SetTimerUIInfo(GlobalTimerInfo _info)
    {
        if (_info.GetTimeOrientation() == 0)
            SetTimeVisibility(false);
        else
            SetTimeVisibility(true);
        // set anchor.
        TimeScale = (int)_info.GetTimeMode();
        MaxTime = _info.GetMaxTime();
        countMinutes = _info.GetUseMinutes();
        countSeconds = _info.GetUseSeconds();
        countMilliseconds = _info.GetUseMilliseconds();
        CurrentTime = (TimeScale == -1) ? MaxTime : 0;
        AnchorDown(_info.GetTimeOrientation());
    }
    private void Update()
    {
        if (!stopTimer)
        {
            CurrentTime += TimeScale * Time.deltaTime;
            CurrentTime = Mathf.Clamp(CurrentTime, 0, MaxTime);
        }
    }

    public string UpdateTime(float newTime)
    {
        string minutes = "", seconds = "", milliseconds = "";
        if(countMinutes)
        {
            minutes = Mathf.Floor(CurrentTime / 60).ToString("0");
        }
        if (countSeconds)
        {
            seconds = Mathf.Floor(CurrentTime % 60).ToString("00");
            if (countMinutes)
                seconds = ":" + seconds;
            else
                seconds = Mathf.Ceil(CurrentTime).ToString();
            if (countMilliseconds)
                seconds += ".";
        }
        if (countMilliseconds)
        {
            float dec = (100 * CurrentTime % 100);        // floats are wierd
            if(dec == 100)
            {
                dec = 0;
            }
            milliseconds = dec.ToString("00");
        }
        string str = minutes + seconds + milliseconds;
        return str;
    }

    public void SetTimeVisibility(bool status)
    {
        Parent.SetActive(status);
        stopTimer = !status;
    }

    public void AnchorDown(GlobalTimeOrientation orientation)
    {
        var parTransform = Parent.GetComponent<RectTransform>();
        if(orientation == GlobalTimeOrientation.bottom)
        {
            parTransform.anchorMin = new Vector2(0.5f, 0);
            parTransform.anchorMax = new Vector2(0.5f, 0);
            parTransform.pivot = new Vector2(0.5f, -0.1f);
        }
        else if(orientation == GlobalTimeOrientation.top)
        {
            parTransform.anchorMin = new Vector2(0.5f, 1);
            parTransform.anchorMax = new Vector2(0.5f, 1);
            parTransform.pivot = new Vector2(0.5f, 1.1f);
        }

        parTransform.anchoredPosition = Vector3.zero;
    }
}
