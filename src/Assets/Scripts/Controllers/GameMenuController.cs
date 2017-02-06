using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour 
{
    public Canvas _gameMenuCanvas;
    public GameObject _resumeButton;
    public Text _highScoreText;    

    #region Public Methods

    // Shows the level completed panel 
    public void OpenMenu(GameState gameState)
    {
        _gameMenuCanvas.enabled = true;

        // Only show the resume button if the game is currently paused
        if(gameState == GameState.Paused)
        {
            _resumeButton.SetActive(true);
        }
        else
        {
            _resumeButton.SetActive(false);
        }

        // Set the high score text
        _highScoreText.text = string.Format(GameMenuText.highScore, ScoreManager.Instance.HighScore);
    }

    // Hides the panel
    public void CloseMenu()
    {
        _gameMenuCanvas.enabled = false;
    }

    #endregion

    #region Private Methods

    
	#endregion
}
