using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PanelController : MonoBehaviour 
{
    public Canvas _panelCanvas;
    public Text _panelTitleText;
    public Text _panelContentText;
    public Text _countdownText;

    #region Mono Behaviours

    void Awake()
    {
        HidePanel();
    }

    #endregion

    #region Public Methods

    // Shows the level completed panel 
    public void ShowLevelCompletedPanel(int countdownToNextLevel)
    {
        ShowPanel(
            PanelText.levelCompletedTitle,
            string.Format(PanelText.levelCompleted, LevelManager.Instance.CurrentLevel.LevelNumber.ToString(), (LevelManager.Instance.CurrentLevel.LevelNumber + 1).ToString())
        );

        // Show the countdown
        ShowCountdown(countdownToNextLevel);
    }

    // Shows the level completed panel 
    public void ShowLostLifePanel(int countdownToGameRestart)
    {
        ShowPanel(PanelText.lostLifeTitle, string.Format(PanelText.lostLife, ScoreManager.Instance.LivesRemaining, ScoreManager.Instance.LivesRemaining == 1 ? "life" : "lives"));

        // Show the countdown
        ShowCountdown(countdownToGameRestart);
    }

    // Shows the level completed panel 
    public void ShowGameOverPanel()
    {
        ShowPanel(PanelText.gameOverTitle, PanelText.gameOver);
    }

    // Hides the panel
    public void HidePanel()
    {
        _panelCanvas.enabled = false;
    }

    #endregion

    #region Private Methods

    // Writes a countdown to screen
    void ShowCountdown(int countdownSteps)
    {
        StartCoroutine(ExecuteCountdown(countdownSteps));
    }  

    // Executes a countdown command
    IEnumerator ExecuteCountdown(int countdownSteps)
    {
        // Loop through countdown steps
        for (int i = countdownSteps; i > 0; i--)
        {            
            // Show the iterator index on screen
            _countdownText.text = i.ToString();

            // Wait for one second before next update
            yield return new WaitForSeconds(1);
        }

        // Reset text to empty string
        _countdownText.text = string.Empty;
    }    

    // Shows the panel
    void ShowPanel(string title, string content)
    {
        _panelTitleText.text = title;
        _panelContentText.text = content;

        _panelCanvas.enabled = true;
    }    
    
	#endregion
}
