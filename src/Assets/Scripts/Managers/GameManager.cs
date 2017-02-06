using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoSingleton<GameManager> 
{
    public PanelController _panelController;    
    
    public PlayerModel Player { get; set; }
    public GameState CurrentState { get; private set; }

	#region Mono Behaviours

    void Start()
    {
        // Start a new game with the AI        
        StartNewGame(new PlayerModel() { IsHuman = false });
    }

	#endregion	

    #region Public Methods

    // Starts a new game
    public void StartNewGame(PlayerModel player)
    {
        // Set the player
        Player = player;

        // Set the AI going if the player is not human
        if(!player.IsHuman)
        {
            GameAI.Instance.Activate();
        }
        else
        {
            GameAI.Instance.Deactivate();
        }

        // Reset the game
        ResetGame();        

        // Start the game objects
        ObjectManager.Instance.StartGameObjects();        

        // Set game state
        CurrentState = GameState.Active;
    }    

    // Resets the game to default values
    public void ResetGame()
    {
        // Reset game manager variables
        CurrentState = GameState.Inactive;               

        // Reset the game objects
        ObjectManager.Instance.ResetGameObjects();

        // Reset the score
        ScoreManager.Instance.Reset();

        // Ask the level manager to go back to starting level
        LevelManager.Instance.Reset();

        // Hide the panel 
        _panelController.HidePanel();
    }

    // Pauses the game
    public void PauseGame()
    {
        // Set game state
        CurrentState = GameState.Paused;        

        // Pause the game objects
        ObjectManager.Instance.PauseGameObjects();
    }

    // Unpauses the game
    public void UnpauseGame()
    {        
        // Unpause the game objects
        ObjectManager.Instance.UnpauseGameObjects();

        // Set game state
        CurrentState = GameState.Active;
    }

    // Ends the game
    public void EndGame()
    {
        // Set game state
        CurrentState = GameState.GameOver;

        // Deactivate the game objects
        ObjectManager.Instance.StopGameObjects();        

        if (Player.IsHuman)
        {
            // Set the high score
            ScoreManager.Instance.SetHighScore();

            // Show the game over panel
            _panelController.ShowGameOverPanel();
        }
        else
        {
            // Deactive the AI
            GameAI.Instance.Deactivate();

            // Restart the game
            StartCoroutine(CountdownToRestartGameForAI());
        }

        // Trigger game over event
        EventManager.Instance.TriggerGameOver();
    }

    #endregion

    #region Private Methods

    // Counts down to the game restart used in a coroutine
    IEnumerator CountdownToRestartGameForAI()
    {
        // Iterate through the pause length to hold the game
        for (float timer = Constants.Game.aiGameRestartLength; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }

        // Restart the game
        StartNewGame(new PlayerModel() { IsHuman = false });
    }
    
	#endregion
}
