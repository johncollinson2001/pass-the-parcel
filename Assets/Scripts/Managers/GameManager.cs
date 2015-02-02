using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	private int _livesRemaining;
    private int _parcelsLoadedTotal;
    private int _parcelsLoadedOnCurrentTruck;

    public HUDController _hudController;    
    public ParcelSpawner _parcelSpawner;
    public GameObject _workerLeft;
    public GameObject _workerRight;
    public List<GameObject> _conveyorBelts = new List<GameObject>();

    public bool IsActiveGame { get { return _livesRemaining > 0; } }

	#region Mono Behaviours

	void Start()
	{
		// Start the game
        StartNewGame();
	}

    void Update()
    {
        // Update the heads up display unit
        _hudController.UpdateHUD(
            _livesRemaining.ToString(),
            LevelManager.Instance.CurrentLevel.LevelNumber.ToString(),
            LevelManager.Instance.CurrentLevel.TruckCapacity.ToString(),
            _parcelsLoadedOnCurrentTruck.ToString()
        );
    }

    void OnEnable()
    {
        EventManager.Instance.ParcelBroken += HandleDroppedParcel;
		EventManager.Instance.ParcelLoaded += HandleLoadedParcel;
    }

    void OnDisable()
    {
        EventManager.Instance.ParcelBroken -= HandleDroppedParcel;
        EventManager.Instance.ParcelLoaded -= HandleLoadedParcel;
    }

	#endregion	

    #region Public Methods

    // Starts a new game
    public void StartNewGame()
    {
        // Clear the text from the HUD
        _hudController.ClearText();

        // Reset the game
        ResetGame();        

        // Start the game
        ActivateGameObjects();
    }

    #endregion

    #region Private Methods

    // Resets the game to default values
    void ResetGame()
    {
        // Reset game manager variables
        _livesRemaining = Defaults.Game.startingLives;
        _parcelsLoadedTotal = 0;
        _parcelsLoadedOnCurrentTruck = 0;

        // Clear all parcels from the conveyors
        foreach (var conveyor in _conveyorBelts)
        {
            conveyor.GetComponent<ConveyorBeltController>().ClearParcels();
        }                

        // Destroy any other parcels in the scene (i.e. dropped parcels)
        foreach(var parcel in GameObject.FindGameObjectsWithTag(Tags.parcel))
        {
            Destroy(parcel);
        }

        // Reset the workers
        _workerLeft.GetComponent<WorkerController>().Reset();
        _workerRight.GetComponent<WorkerController>().Reset();

        // Reset the parcel spawner        
        _parcelSpawner.Reset();

        // Ask the level manager to go back to starting level
        LevelManager.Instance.Reset();

        // Apply the level settings to the game
        ApplyLevelSettingsToGame();        
    }

    // Handles the event of a worker dropping a parcel
	void HandleDroppedParcel()
	{
        // Ensure the game has not already ended
        if (IsActiveGame)
        {
            // Decrement worker life counter
            _livesRemaining -= 1;

            // See if the game is over
            if (_livesRemaining == 0)
            {
                // End the game
                EndGame();
            }
            else
            {
                // Handle the loss of a worker life
                PauseForLostLife();
            }
        }
	}

    void PauseForLostLife()
    {
        // Pause the game
        PauseGame();

        // Update the hud with lost life
        _hudController.WriteLifeLostTextToScreen(_livesRemaining.ToString());

        // Kick off a coroutine to pause until the game starts again
        StartCoroutine(CountdownToRestartLevelFollowingLostLife());
    }

    // Counts down to the game restart used in a coroutine
    IEnumerator CountdownToRestartLevelFollowingLostLife()
    {
        // Iterate through the pause length to hold the game
        for (float timer = Defaults.Game.lifeLostPauseLength; timer >= 0; timer -= Time.deltaTime)
        {
            // Update the hud
            _hudController.WriteCountdownToScreen(Defaults.Game.lifeLostCountdownLength, timer);

            yield return 0;
        }

        // Restart the level
        RestartLevelFollowingLostLife();
    }

    // Starts the next level
    void RestartLevelFollowingLostLife()
    {
        // Clear the text from the HUD
        _hudController.ClearText();

        // Unpause the game
        UnpauseGame();
    }

    // Handles the event of a worker loading a parcel onto the truck
	void HandleLoadedParcel()
	{
		// Increment loaded parcels counter
		_parcelsLoadedTotal += 1;
        _parcelsLoadedOnCurrentTruck += 1;

		// See if the truck is full
        if (_parcelsLoadedOnCurrentTruck == LevelManager.Instance.CurrentLevel.TruckCapacity)
        {
            // Player has reached the next level
            MoveToNextLevel();
        }		
	}

    // Moves the worker to the next level
    void MoveToNextLevel()
    {
        // Pause the game
        PauseGame();

        // Update the hud with level up text
        _hudController.WriteLevelUpTextToScreen(LevelManager.Instance.CurrentLevel.LevelNumber.ToString(), (LevelManager.Instance.CurrentLevel.LevelNumber + 1).ToString());

        // Kick off a coroutine to pause until the next level starts
        StartCoroutine(CountdownToNextLevel());
    }

    // Counts down to the next level used in a coroutine
    IEnumerator CountdownToNextLevel()
    {
        // Iterate through the pause length to hold the game
        for (float timer = Defaults.Game.levelUpPauseLength; timer >= 0; timer -= Time.deltaTime)
        {
            // Update the hud
            _hudController.WriteCountdownToScreen(Defaults.Game.levelUpCountdownLength, timer);

            yield return 0;
        }        

        // Move to next level
        StartNextLevel();
    }

    // Starts the next level
    void StartNextLevel()
    {
        // Clear the text from the HUD
        _hudController.ClearText();

        // Up the level
        LevelManager.Instance.LevelUp();

        // Apply the level settings to the game
        ApplyLevelSettingsToGame();

        // Reset counter
        _parcelsLoadedOnCurrentTruck = 0;

        // Unpause the game
        UnpauseGame();
    }

    // Apply level settings to game 
    void ApplyLevelSettingsToGame()
    {
        // Apply the settings of the next level to the game objects
        _parcelSpawner.SpawnRate = LevelManager.Instance.CurrentLevel.SpawnRate;
        _parcelSpawner.MinimumSpawnsPerWave = LevelManager.Instance.CurrentLevel.MinimumSpawnsPerBurst;
        _parcelSpawner.MaximumSpawnsPerWave = LevelManager.Instance.CurrentLevel.MaximumSpawnsPerBurst;
        _parcelSpawner.SpawnWaveGap = LevelManager.Instance.CurrentLevel.SpawnBurstGap;
        _parcelSpawner.UseSpawnRateRandomiser = LevelManager.Instance.CurrentLevel.UseSpawnRateRandomiser;

        foreach (var conveyorBelt in _conveyorBelts)
        {
            conveyorBelt.GetComponent<ConveyorBeltController>()._speed = LevelManager.Instance.CurrentLevel.ConveyorBeltSpeed;
        }
    }

	void EndGame()
	{
		// Pause the game
        PauseGame();

        // Display game ended message
        _hudController.WriteGameOverText();
    }

    // Pauses the game
    void PauseGame()
    {        
        // Pause the parcel spawner
        _parcelSpawner.PauseSpawning();

        // Deactivate the game objects
        DeactivateGameObjects();
    }    

    // Unpauses the game
    void UnpauseGame()
    {        
        // Unpause the parcel spawner
        _parcelSpawner.UnpauseSpawning();

        // Active the game objects
        ActivateGameObjects();
    }

    // Activates the game objects
    void ActivateGameObjects()
    {
        // Enable the conveyor belts
        foreach (var conveyorBelt in _conveyorBelts)
        {
            conveyorBelt.GetComponent<ConveyorBeltController>().StartConveyorBelt();
        }        

        // Enable the workers
        _workerLeft.GetComponent<WorkerController>().Active = true;
        _workerRight.GetComponent<WorkerController>().Active = true;

        // Enable the parcel spawner
        _parcelSpawner.StartSpawning();
    }

    // Deactivates the game objects
    void DeactivateGameObjects()
    {
        // Disable the conveyor belts
        foreach (var conveyorBelt in _conveyorBelts)
        {
            conveyorBelt.GetComponent<ConveyorBeltController>().StopConveyorBelt();
        }

        // Disable the workers
        _workerLeft.GetComponent<WorkerController>().Active = false;
        _workerRight.GetComponent<WorkerController>().Active = false;

        // Stop the parcel spawner
        _parcelSpawner.StopSpawning();
    }
    
	#endregion
}
