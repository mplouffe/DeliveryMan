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
        private DeliveryController m_playerPrefab;

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

        [SerializeField]
        private AudioSource m_sfxSource;

        [SerializeField]
        private AudioSource m_introTheme;

        [SerializeField]
        private AudioSource m_levelTheme;

        [SerializeField]
        private AudioSource m_readyTheme;

        [SerializeField]
        private AudioSource m_gameOverTheme;

        [SerializeField]
        private AudioSource m_scoreTheme;

        [SerializeField]
        private AudioClip m_jumpSfx;

        [SerializeField]
        private AudioClip m_pickupSfx;

        [SerializeField]
        private AudioClip m_deliverySfx;

        private DeliveryController m_currentPlayer;

        public GameState m_gameState;

        private Duration m_gameOverDuration;
        private Duration m_gameStartDuration;
        private Duration m_scoreDisplayDuration;

        private int m_playerScore;
        private int m_playerDistance;

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

        public void LeaveInstructions()
        {
            ChangeState(GameState.Menu);
            SceneManager.LoadScene(0);
        }

        public void EnterInstructions()
        {
            ChangeState(GameState.Instructions);
            SceneManager.LoadScene(1);
        }

        public void StartGame()
        {
            SceneManager.LoadScene(2);    
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);

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
                    m_levelTheme.Stop();
                    m_gameOverTheme.Play();
                    var controller2D = m_currentPlayer.GetComponent<Controller2D>();
                    m_playerDistance = (int)controller2D.GetTotalDistance();
                    break;
                case GameState.Paused:
                    PopupsManager.Instance.Pause(true);
                    Clock.Instance.StopClock();
                    m_levelTheme.Pause();
                    break;
                case GameState.Escaped:
                    PopupsManager.Instance.Escaped(true);
                    Clock.Instance.StopClock();
                    m_levelTheme.Pause();
                    break;
                case GameState.GameRunning:
                    switch (m_gameState)
                    {
                        case GameState.Paused:
                            PopupsManager.Instance.Pause(false);
                            Clock.Instance.StartClock();
                            m_levelTheme.UnPause();
                            break;
                        case GameState.GameStart:
                            PopupsManager.Instance.GetReady(false);
                            Clock.Instance.StartClock();
                            Point startingPoint = DeliveryPointsManager.Instance.GetRandomStartingPoint();
                            m_currentPlayer.SetTarget(startingPoint);
                            m_readyTheme.Stop();
                            m_levelTheme.Play();
                            break;
                        case GameState.Escaped:
                            PopupsManager.Instance.Escaped(false);
                            Clock.Instance.StartClock();
                            m_levelTheme.UnPause();
                            break;
                    }
                    break;
                case GameState.GameStart:
                    if (m_gameState == GameState.Menu)
                    {
                        m_introTheme.Stop();
                    }
                    m_playerDistance = 0;
                    m_playerScore = 0;
                    m_gameStartDuration.Reset();
                    StartLevel();
                    PopupsManager.Instance.GetReady(true);
                    Clock.Instance.OnClockElapsed += OnClockElapsed;
                    m_readyTheme.Play();
                    break;
                case GameState.DisplayingScore:
                    m_gameOverTheme.Stop();
                    m_scoreTheme.Play();
                    SceneManager.LoadScene(3);
                    m_scoreDisplayDuration.Reset();
                    break;
                case GameState.Menu:
                    SceneManager.LoadScene(0);
                    if (m_gameState != GameState.Instructions)
                    {
                        m_scoreTheme.Stop();
                        m_introTheme.Play();
                    }
                    break;
            }
            m_gameState = newState;
        }

        public void AssignStartPoint(Point startingPoint)
        {
            m_currentPlayer.SetTarget(startingPoint);
        }

        private void OnClockElapsed()
        {
            Clock.Instance.OnClockElapsed -= OnClockElapsed;
            ChangeState(GameState.GameOver);
        }

        public void GameSceneLoaded()
        {
            ChangeState(GameState.GameStart);
        }

        public void SetScore(int points)
        {
            m_playerScore = points;
        }

        public void SetDistance(int distance)
        {
            m_playerDistance = distance;
        }

        public PlayerSide GetPlayerSide(float targetX)
        {
            return targetX < m_currentPlayer.transform.position.x ? PlayerSide.Right : PlayerSide.Left;
        }

        public void PlaySfx(Sfx effectToPlay)
        {
            switch (effectToPlay)
            {
                case Sfx.Jump:
                    m_sfxSource.PlayOneShot(m_jumpSfx);
                    break;
                case Sfx.Pickup:
                    m_sfxSource.PlayOneShot(m_pickupSfx);
                    break;
                case Sfx.Delivery:
                    m_sfxSource.PlayOneShot(m_deliverySfx);
                    break;
            }
        }

        public int GetPlayerScore()
        {
            return m_playerScore;
        }

        public int GetPlayerDistance()
        {
            return m_playerDistance;
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

    public enum Sfx
    {
        Jump,
        Pickup,
        Delivery
    }
}
