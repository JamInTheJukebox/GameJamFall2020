using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public static string timeText;
    public static bool stopTimer = false;
    private TextMeshProUGUI TimerText;

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
                TimerText.text = "Time: " + m_TimeElapsed;
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
        stopTimer = false;
    }
    private void Update()
    {
        if (!stopTimer)
        {
            CurrentTime += Time.deltaTime;
        }
    }

    public string UpdateTime(float newTime)
    {
        // hours??? Lmao
        string minutes, seconds, milliseconds;
        minutes = Mathf.Floor(CurrentTime / 60).ToString("0");
        seconds = Mathf.Floor(CurrentTime % 60).ToString("00");
        milliseconds = (100*CurrentTime%100).ToString("00");        // floats are wierd
        string Time = minutes + ":" + seconds + "." + milliseconds;
        TimerUI.timeText = Time;
        return Time;
    }
}
