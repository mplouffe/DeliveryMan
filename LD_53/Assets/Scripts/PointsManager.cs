using lvl_0;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    [SerializeField]
    private TextMeshProUGUI m_scoreCounter;

    private int m_playerScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        m_playerScore = 0;
    }

    public void AddPoints(int points)
    {
        m_playerScore += points;
        m_scoreCounter.text = m_playerScore.ToString();
    }

    private void OnEnable()
    {
        Clock.Instance.OnClockElapsed += OnClockElapsed;
    }

    private void OnClockElapsed()
    {
        Clock.Instance.OnClockElapsed -= OnClockElapsed;
        GameManager.Instance.SetScore(m_playerScore);
    }
}
