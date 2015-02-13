using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMenuManager : MonoBehaviour 
{
    public GameManager _gameManager;
    public GameMenuController _gameMenuController;

    #region Mono Behaviours

    void Awake()
    {
        _gameMenuController.OpenMenu(false);
    }

    #endregion

    #region Public Methods

    // Starts a new game
    public void StartNewGameClickHandler()
    {
        // Close game menu
        _gameMenuController.CloseMenu();

        // Get game manager to start a new game
        _gameManager.StartNewGame();
    }

    // Opens the game menu and pauses the game
    public void InGameMenuButtonClickHandler()
    {
        // Pause the game 
        _gameManager.PauseGame();

        // Open the game menu with the resume button
        _gameMenuController.OpenMenu(true);
    }

    // Closes the game menu and un pauses the game
    public void ResumeButtonClickHandler()
    {
        // Close the menu
        _gameMenuController.CloseMenu();

        // Unpause the game
        _gameManager.UnpauseGame();
    }
    
    public void RestartAfterGameOverClickHandler()
    {
        // Check the game is in game over state
        if (_gameManager.CurrentState == GameState.GameOver)
        {
            // Reset the game
            _gameManager.ResetGame();
        }

        // Open the game menu
        _gameMenuController.OpenMenu(false);
    }

	#endregion
}
