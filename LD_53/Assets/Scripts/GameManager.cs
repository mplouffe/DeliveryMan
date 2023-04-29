using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorkyKong_GameManager : MonoBehaviour
{
    public static BorkyKong_GameManager Instance;

    [SerializeField]
    private PlayerController m_playerPrefab;

    [SerializeField]
    private Vector3 m_playerStartingPosition;

    private PlayerController m_currentPlayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void StartLevel()
    {
        m_currentPlayer = Instantiate(m_playerPrefab, m_playerStartingPosition, Quaternion.identity);
    }
}
