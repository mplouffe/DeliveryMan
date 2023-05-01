using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace lvl_0
{
    public class GameOverManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_scoreCounter;

        [SerializeField]
        private TextMeshProUGUI m_distanceCounter;

        [SerializeField]
        private TextMeshProUGUI m_totalCounter;

        private void OnEnable()
        {
            var playerScore = GameManager.Instance.GetPlayerScore();
            var playerDistance = GameManager.Instance.GetPlayerDistance();
            var totalScore = playerScore + playerDistance;
            m_scoreCounter.text = playerScore.ToString();
            m_distanceCounter.text = playerDistance.ToString();
            m_totalCounter.text = totalScore.ToString();

        }
    }
}
