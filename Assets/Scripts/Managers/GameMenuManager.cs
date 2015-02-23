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
        _gameMenuController.OpenMenu(_gameManager.CurrentState);
    }

    #endregion

    #region Public Methods

    // Starts a new game
    public void StartNewGameClickHandler()
    {
        // Close game menu
        _gameMenuController.CloseMenu();

        // Get game manager to start a new game
        _gameManager.StartNewGame(new PlayerModel() { IsHuman = true });
    }

    // Opens the game menu and pauses the game
    public void InGameMenuButtonClickHandler()
    {
        // Pause the game 
        _gameManager.PauseGame();

        // Open the game menu with the resume button
        _gameMenuController.OpenMenu(_gameManager.CurrentState);
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
        // Open the game menu
        _gameMenuController.OpenMenu(_gameManager.CurrentState);

        // Get game manager to start a new game
        _gameManager.StartNewGame(new PlayerModel() { IsHuman = false });
    }

	#endregion
}
