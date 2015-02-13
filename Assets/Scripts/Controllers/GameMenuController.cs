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
    public void OpenMenu(bool showResumeButton)
    {
        _gameMenuCanvas.enabled = true;

        if(showResumeButton)
        {
            _resumeButton.SetActive(true);
        }
        else
        {
            _resumeButton.SetActive(false);
        }

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
