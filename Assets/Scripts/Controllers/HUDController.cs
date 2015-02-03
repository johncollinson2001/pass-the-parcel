using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDController : MonoBehaviour 
{
    private bool _showGameOverText;
    private bool _showLevelCompletedText;
    private bool _showLifeLostText;

	public GUIText _livesRemainingText;
    public GUIText _levelText;
    public GUIText _truckCapacityText;
    public GUIText _parcelLoadedOnTruckText;
    public GUIText _gameOverText;
    public GUIText _levelCompletedText;
    public GUIText _lifeLostText;
    public GUIText _countdownText;

    public GameManager _gameManager;

	#region Mono Behaviours

	void Awake()
	{
        _countdownText.text = string.Empty;
	}

    void Update()
    {
        UpdateHUD();
    }

    void OnEnable()
    {
        EventManager.Instance.GameStateChanged += OnGameStateChanged;
    }

    void OnDisable()
    {
        EventManager.Instance.GameStateChanged -= OnGameStateChanged;
    }

	#endregion	

    #region Public Methods

    // Writes a countdown to screen
    public void ShowCountdown(int countdownSteps)
    {
        ShowCountdown(countdownSteps, countdownSteps);
    } 

    public void ShowCountdown(int countdownSteps, int showCountdownFrom)
    {
        StartCoroutine(ExecuteCountdown(countdownSteps, showCountdownFrom));
    }  

    #endregion

    #region Private Methods

    // Executes a countdown command
    IEnumerator ExecuteCountdown(int countdownSteps, int showCountdownFrom)
    {
        // Loop through countdown steps
        for (int i = countdownSteps; i > 0; i--)
        {            
            // Show the iterator index on screen
            if (i <= showCountdownFrom)
            {
                _countdownText.text = i.ToString();
            }

            // Wait for one second before next update
            yield return new WaitForSeconds(1);
        }

        // Reset text to empty string
        _countdownText.text = string.Empty;
    }

    // Handles the game state changed event
    void OnGameStateChanged(GameState changedFrom, GameState changedTo)
    { 
        if(changedTo == GameState.Active)
        {
            _showGameOverText = false;
            _showLevelCompletedText = false;
            _showLifeLostText = false;
        }
        else if (changedTo == GameState.LevelCompleted)
        {
            _showGameOverText = false;
            _showLevelCompletedText = true;
            _showLifeLostText = false;
        }
        else if (changedTo == GameState.LostLife)
        {
            _showGameOverText = false;
            _showLevelCompletedText = false;
            _showLifeLostText = true;
        }
        else if (changedTo == GameState.GameOver)
        {
            _showGameOverText = true;
            _showLevelCompletedText = false;
            _showLifeLostText = false;
        }
    }

    // Updates the HUD
    void UpdateHUD()
	{
		_livesRemainingText.text = string.Format(Messages.livesRemaining, _gameManager.LivesRemaining);
        _levelText.text = string.Format(Messages.level, LevelManager.Instance.CurrentLevel.LevelNumber);
        _truckCapacityText.text = string.Format(Messages.truckCapacity, LevelManager.Instance.CurrentLevel.TruckCapacity);
        _parcelLoadedOnTruckText.text = string.Format(Messages.parcelsLoadedOnTruck, _gameManager.ParcelsLoadedOnCurrentTruck);

        WriteGameOverText();
        WriteLevelCompletedTextToScreen();
        WriteLifeLostTextToScreen();
	}

    // Writes the game over text
    void WriteGameOverText()
    {
        if (_showGameOverText)
        {
            _gameOverText.text = Messages.gameOver;
        }
        else
        {
            _gameOverText.text = string.Empty;
        }
    }

    // Writes the level up text to the screen
    void WriteLevelCompletedTextToScreen()
    {
        if (_showLevelCompletedText)
        {
            _levelCompletedText.text = string.Format(Messages.levelCompleted, LevelManager.Instance.CurrentLevel.LevelNumber.ToString(), (LevelManager.Instance.CurrentLevel.LevelNumber + 1).ToString());
        }
        else
        {
            _levelCompletedText.text = string.Empty;
        }
    }

    // Writes the life lost text to the screen
    void WriteLifeLostTextToScreen()
    {
        if (_showLifeLostText)
        {
            _lifeLostText.text = string.Format(Messages.lostLife, _gameManager.LivesRemaining);
        }
        else
        {
            _lifeLostText.text = string.Empty;
        }
    }   
    
	#endregion
}
