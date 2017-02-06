using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoSingleton<MenuManager> 
{
    public GameMenuController _menuController;

    #region Mono Behaviours

    void Awake()
    {
        _menuController.OpenMenu(GameManager.Instance.CurrentState);
    }

    #endregion

    #region Public Methods

    // Starts a new game
    public void StartNewGame()
    {
        // Close game menu
        _menuController.CloseMenu();

        // Get game manager to start a new game
        GameManager.Instance.StartNewGame(new PlayerModel() { IsHuman = true });
    }

    // Opens the game menu and pauses the game
    public void OpenMenuDuringGame()
    {
        // Pause the game 
        GameManager.Instance.PauseGame();

        // Open the game menu with the resume button
        _menuController.OpenMenu(GameManager.Instance.CurrentState);
    }

    // Closes the game menu and un pauses the game
    public void ResumeGame()
    {
        // Close the menu
        _menuController.CloseMenu();

        // Unpause the game
        GameManager.Instance.UnpauseGame();
    }
    
    public void OpenMenuWhenGameOver()
    {       
        // Open the game menu
        _menuController.OpenMenu(GameManager.Instance.CurrentState);

        // Get game manager to start a new game
        GameManager.Instance.StartNewGame(new PlayerModel() { IsHuman = false });
    }

	#endregion
}
