using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// Manages the game's main state machine, score, difficulty, and event handling.
/// </summary>
internal class GameManager : MonoBehaviour
{
    /// <summary>
    /// Gets the current game speed.
    /// </summary>
    public static float Speed { get; private set; }

    [SerializeField, Tooltip("Initial score of the player.")]
    private int _playerScore;

    [SerializeField, Tooltip("Maximum speed the game can reach.")]
    private float _maxSpeed = 10f;

    [SerializeField, Range(0.1f, 1f), Tooltip("Maximum probability of item spawning.")]
    private float _maxItemSpawnProb = 0.6f;

    [Header("UI-Refs")]
    [SerializeField, Tooltip("Show Player score.")]
    private Text _scoreTxt;
    [SerializeField, Tooltip("Show Player best score.")]
    private Text _bestScoreTxt;
    [SerializeField, Tooltip("Replay button for play again")]
    private Button _replayBtn;
    [SerializeField, Tooltip("Start button for game start")]
    private Button _startBtn;
    [SerializeField, Tooltip("Game over canvas")]
    private GameObject _gameOverPanel;



    private GameState _gameState;
    private int _scoreIncrementFactor = 1;
    private float _itemSpawnProbIncrementFactor;
    private float _currentItemSpawnProb;
    private bool _hasStarted = false;
    private void Awake()
    {
        HandleAppConfig();
        // Register event listeners
        EventManager.Instance.AddEventListener<int>(GamePlayEvents.CollectItemPick, OnCollectItem);
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.UpdateScore, OnUpdateScore);
        EventManager.Instance.AddEventListener<bool>(GamePlayEvents.GameOver, OnGameOver);
    }

    private void Start()
    {
        _startBtn.onClick.AddListener(StartGame);
        _replayBtn.onClick.AddListener(ReplayGame);
        _startBtn.gameObject.SetActive(true);
        _itemSpawnProbIncrementFactor = _maxItemSpawnProb / _maxSpeed;
        _gameState = GameState.Idle;
    }

    private void Update()
    {
        ManageGameState();
    }

    /// <summary>
    /// Handles App Configurations.
    /// </summary>
    private void HandleAppConfig()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }


    /// <summary>
    /// Manages the game's state transitions and behaviors.
    /// </summary>
    private void ManageGameState()
    {
        switch (_gameState)
        {
            case GameState.Idle:
                ResetGameParameters();
                break;
            case GameState.Playing:
                // Gameplay logic can be added here if needed
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
        }
    }

    /// <summary>
    /// Resets game parameters to their initial state.
    /// </summary>
    private void ResetGameParameters()
    {
        _playerScore = 0;
        _scoreIncrementFactor = 1;
        Speed = 5f;
        _currentItemSpawnProb = 0;
        Physics.gravity = new Vector3(0, -25f, 0);
    }

    /// <summary>
    /// Handles the Game Over state.
    /// </summary>
    private void HandleGameOver()
    {
        Physics.gravity = new Vector3(0, -60f, 0);
        SavePlayerScore();
        GameOverPanelEnabled(true);
        _gameState = GameState.Idle;
    }

    /// <summary>
    /// Event handler for when the game is over.
    /// </summary>
    private void OnGameOver(object sender, bool eventArgs)
    {
        _gameState = GameState.GameOver;
    }

    /// <summary>
    /// Saves the player's score to persistent storage.
    /// </summary>
    private void SavePlayerScore()
    {
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (_playerScore > bestScore)
        {
            bestScore = _playerScore;
            PlayerPrefs.SetInt("BestScore", _playerScore);
            PlayerPrefs.Save();
        }
        UpdatePlayerBestScoreUI(bestScore);
    }

    /// <summary>
    /// Event handler for collecting an item.
    /// </summary>
    private void OnCollectItem(object sender, int eventArgs)
    {
        _playerScore += eventArgs;
        UpdateScoreUI(_playerScore);
    }

    /// <summary>
    /// Event handler for updating the score.
    /// </summary>
    private void OnUpdateScore(object sender, bool eventArgs)
    {
        _playerScore += _scoreIncrementFactor;
        UpdateDifficulty();
        UpdateScoreUI(_playerScore);
    }


    /// <summary>
    /// Adjusts the game's difficulty based on the player's score.
    /// </summary>
    private void UpdateDifficulty()
    {
        if (_playerScore % 10 == 0 && Speed < _maxSpeed)
        {
            Speed++;
            _currentItemSpawnProb = _itemSpawnProbIncrementFactor * Speed;
            EventManager.Instance.Dispatch(GamePlayEvents.UpdateItemSpawnProb, null, _currentItemSpawnProb);
        }
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        _startBtn.gameObject.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(GameStartCoroutine());
    }

    /// <summary>
    /// Coroutine to initialize and start the game.
    /// </summary>
    private IEnumerator GameStartCoroutine()
    {
        EventManager.Instance.Dispatch(GamePlayEvents.Initialize, null, true);
        yield return new WaitForSeconds(0.5f);
        EventManager.Instance.Dispatch(GamePlayEvents.GameStart, null, true);
        _gameState = GameState.Playing;
    }

    /// <summary>
    /// Replay the game.
    /// </summary>
    public void ReplayGame()
    {
        _gameState = GameState.Idle;
        GameOverPanelEnabled(false);
        StopAllCoroutines();
        StartCoroutine(GameResetCoroutine());
    }

    /// <summary>
    /// Coroutine to reset the game.
    /// </summary>
    private IEnumerator GameResetCoroutine()
    {
        EventManager.Instance.Dispatch(GamePlayEvents.Reset, null, true);
        yield return new WaitForSeconds(0.3f);
        StartGame();
    }

    /// <summary>
    /// Update the player's score UI.
    /// </summary>
    private void UpdateScoreUI(int score)
    {
        _scoreTxt.text = score == 0 ? "Hop" : score.ToString();
    }

    /// <summary>
    /// Update the player's best score UI.
    /// </summary>
    private void UpdatePlayerBestScoreUI(int score)
    {
        _bestScoreTxt.text = score.ToString();
    }

    /// <summary>
    /// Activate or Deactivate game over panel.
    /// </summary>
    private void GameOverPanelEnabled(bool status)
    {
        _gameOverPanel.SetActive(status);
    }

}

/// <summary>
/// Defines the various states of the game.
/// </summary>
enum GameState
{
    Idle,
    Playing,
    GameOver
}
