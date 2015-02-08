using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HUDController : MonoBehaviour 
{
	public GUIText _livesRemainingText;
    public GUIText _levelText;
    public GUIText _truckCapacityText;
    public GUIText _parcelLoadedOnTruckText;
    public GUIText _scoreText;

    public Canvas _panelCanvas;
    public Text _panelTitleText;
    public Text _panelContentText;
    public Text _countdownText;

    public GameManager _gameManager;

	#region Mono Behaviours

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
            HidePanel();
        }
        else if (changedTo == GameState.LevelCompleted)
        {
            ShowLevelCompletedPanel();
        }
        else if (changedTo == GameState.LostLife)
        {
            ShowLostLifePanel();
        }
        else if (changedTo == GameState.GameOver)
        {
            ShowGameOverPanel();
        }
    }

    // Updates the HUD
    void UpdateHUD()
	{
		_livesRemainingText.text = string.Format(HudText.livesRemaining, _gameManager.LivesRemaining);
        _levelText.text = string.Format(HudText.level, LevelManager.Instance.CurrentLevel.LevelNumber);
        _truckCapacityText.text = string.Format(HudText.truckCapacity, LevelManager.Instance.CurrentLevel.TruckCapacity);
        _parcelLoadedOnTruckText.text = string.Format(HudText.parcelsLoadedOnTruck, _gameManager.ParcelsLoadedOnCurrentTruck);
        _scoreText.text = string.Format(HudText.score, ScoreManager.Instance.CurrentScore);
	}

    // Hides the panel
    void HidePanel()
    {
        _panelCanvas.enabled = false;
    }

    // Shows the panel
    void ShowPanel(string title, string content)
    {
        _panelTitleText.text = title;
        _panelContentText.text = content;

        _panelCanvas.enabled = true;
    }

    // Shows the level completed panel 
    void ShowLevelCompletedPanel()
    {
        ShowPanel(
            HudText.levelCompletedTitle, 
            string.Format(HudText.levelCompleted, LevelManager.Instance.CurrentLevel.LevelNumber.ToString(), (LevelManager.Instance.CurrentLevel.LevelNumber + 1).ToString())
        );
    }

    // Shows the level completed panel 
    void ShowLostLifePanel()
    {
        ShowPanel(HudText.lostLifeTitle, string.Format(HudText.lostLife, _gameManager.LivesRemaining));
    }

    // Shows the level completed panel 
    void ShowGameOverPanel()
    {
        ShowPanel(HudText.gameOverTitle, HudText.gameOver);
    }
    
	#endregion
}
