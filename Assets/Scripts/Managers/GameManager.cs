using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    private static GameManager _instance;    

    public PanelController _panelController;
    public ParcelSpawner _parcelSpawner;
    public GameAI _ai;
    
    public PlayerModel Player { get; set; }
    public GameState CurrentState { get; private set; }

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    } 

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
            _ai.Activate();
        }
        else
        {
            _ai.Deactivate();
        }

        // Reset the game
        ResetGame();        

        // Activate the game objects
        ObjectManager.Instance.ActivateGameObjects();

        // Enable the parcel spawner
        _parcelSpawner.StartSpawning();

        // Set game state
        CurrentState = GameState.Active;
    }    

    // Resets the game to default values
    public void ResetGame()
    {
        // Reset game manager variables
        CurrentState = GameState.Inactive;
        
        // Reset the parcel spawner        
        _parcelSpawner.Reset();

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

        // Pause the parcel spawner
        _parcelSpawner.PauseSpawning();

        // Deactivate the game objects
        ObjectManager.Instance.DeactivateGameObjects();
    }

    // Unpauses the game
    public void UnpauseGame()
    {
        // Unpause the parcel spawner
        _parcelSpawner.UnpauseSpawning();

        // Active the game objects
        ObjectManager.Instance.ActivateGameObjects();

        // Set game state
        CurrentState = GameState.Active;
    }

    // Ends the game
    public void EndGame()
    {
        // Set game state
        CurrentState = GameState.GameOver;

        // Deactivate the game objects
        ObjectManager.Instance.DeactivateGameObjects();

        // Stop the parcel spawner
        _parcelSpawner.StopSpawning();

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
            _ai.Deactivate();

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
