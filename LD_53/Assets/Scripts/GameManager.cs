using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace lvl_0
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField]
        private PlayerController m_playerPrefab;

        [SerializeField]
        private Vector3 m_playerStartingPosition;

        [SerializeField]
        private float m_startingDeliveryDuration;

        [SerializeField]
        private float m_gameOverWaitDuration;

        [SerializeField]
        private float m_gameStartWaitDuration;

        [SerializeField]
        private float m_scoreDisplayWaitDuration;

        private PlayerController m_currentPlayer;

        private GameState m_gameState;

        private Duration m_gameOverDuration;
        private Duration m_gameStartDuration;
        private Duration m_scoreDisplayDuration;

        public bool GameIsRunning { get { return m_gameState == GameState.GameRunning; } }

        public void Pause()
        {
            if (m_gameState == GameState.Paused)
            {
                ChangeState(GameState.GameRunning);
            }
            else if (m_gameState == GameState.GameRunning)
            {
                ChangeState(GameState.Paused);
            }
            else if (m_gameState == GameState.Escaped)
            {
                ChangeState(GameState.GameRunning);
            }
        }

        public void Escape()
        {
            if (m_gameState == GameState.GameRunning)
            {
                ChangeState(GameState.Escaped);
            }
            else if (m_gameState == GameState.Escaped)
            {
                SceneManager.LoadScene(0);
            }
        }

        public void StartGame()
        {
            SceneManager.sceneLoaded += OnGameSceneLoaded;
            SceneManager.LoadScene(2);    
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            m_gameState = GameState.Menu;
            m_gameOverDuration = new Duration(m_gameOverWaitDuration);
            m_gameStartDuration = new Duration(m_gameStartWaitDuration);
            m_scoreDisplayDuration = new Duration(m_scoreDisplayWaitDuration);
        }

        private void Update()
        {
            switch (m_gameState)
            {
                case GameState.GameOver:
                    m_gameOverDuration.Update(Time.deltaTime);
                    if (m_gameOverDuration.Elapsed())
                    {
                        ChangeState(GameState.DisplayingScore);
                    }
                    break;
                case GameState.GameStart:
                    m_gameStartDuration.Update(Time.deltaTime);
                    if (m_gameStartDuration.Elapsed())
                    {
                        ChangeState(GameState.GameRunning);
                    }
                    break;
                case GameState.DisplayingScore:
                    m_scoreDisplayDuration.Update(Time.deltaTime);
                    if (m_scoreDisplayDuration.Elapsed())
                    {
                        ChangeState(GameState.Menu);
                    }
                    break;
            }

        }

        private void StartLevel()
        {
            m_currentPlayer = Instantiate(m_playerPrefab, m_playerStartingPosition, Quaternion.identity);
            CinemachineCameraAccessor.Instance.GetCamera().Follow = m_currentPlayer.transform;
            Clock.Instance.SetClock(m_startingDeliveryDuration);
        }

        private void ChangeState(GameState newState)
        {
            switch (newState)
            {
                case GameState.GameOver:
                    m_gameOverDuration.Reset();
                    PopupsManager.Instance.GameOver(true);
                    break;
                case GameState.Paused:
                    PopupsManager.Instance.Pause(true);
                    Clock.Instance.StopClock();
                    break;
                case GameState.Escaped:
                    PopupsManager.Instance.Escaped(true);
                    Clock.Instance.StopClock();
                    break;
                case GameState.GameRunning:
                    switch (m_gameState)
                    {
                        case GameState.Paused:
                            PopupsManager.Instance.Pause(false);
                            Clock.Instance.StartClock();
                            break;
                        case GameState.GameStart:
                            PopupsManager.Instance.GetReady(false);
                            Clock.Instance.StartClock();
                            break;
                        case GameState.Escaped:
                            PopupsManager.Instance.Escaped(false);
                            Clock.Instance.StartClock();
                            break;
                    }
                    break;
                case GameState.GameStart:
                    m_gameStartDuration.Reset();
                    StartLevel();
                    PopupsManager.Instance.GetReady(true);
                    Clock.Instance.OnClockElapsed += OnClockElapsed;
                    break;
                case GameState.DisplayingScore:
                    SceneManager.LoadScene(3);
                    m_scoreDisplayDuration.Reset();
                    break;
            }
            m_gameState = newState;
        }

        private void OnClockElapsed()
        {
            Clock.Instance.OnClockElapsed -= OnClockElapsed;
            ChangeState(GameState.GameOver);
        }

        private void OnGameSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
            ChangeState(GameState.GameStart);
        }
    }

    public enum GameState
    {
        GameStart,
        GameRunning,
        Paused,
        Escaped,
        GameOver,
        DisplayingScore,
        Menu,
        Instructions
    }
}
