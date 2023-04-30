using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public static Clock Instance;

    [SerializeField]
    private TextMeshProUGUI m_timeLabel;

    [SerializeField]
    private TextMeshProUGUI m_timeCounter;

    public Action OnClockElapsed;

    private float m_timeRemaining;

    private ClockState m_clockState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
        m_clockState = ClockState.Stopped;
    }

    private void Update()
    {
        if (m_clockState == ClockState.Running)
        {
            m_timeRemaining -= Time.deltaTime;
            if (m_timeRemaining <= 0)
            {
                m_timeRemaining = 0;
                StopClock();
                OnClockElapsed?.Invoke();
            }
            DisplayTime();
        }
    }

    private void DisplayTime()
    {
        int minutes = Mathf.FloorToInt(m_timeRemaining / 60f);
        int seconds = Mathf.RoundToInt(m_timeRemaining % 60f);

        m_timeCounter.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetClock(float setDuration)
    {
        m_timeRemaining = setDuration;
        DisplayTime();
    }

    public void AddTimeToClock(float addedTime)
    {
        m_timeRemaining += addedTime;
    }

    public void StartClock()
    {
        m_clockState = ClockState.Running;
    }

    public void StopClock()
    {
        m_clockState = ClockState.Stopped;
    }

}

public enum ClockState
{
    Running,
    Stopped
}
