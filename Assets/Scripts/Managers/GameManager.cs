using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    private GameState _currentState;

    public HUDController _hudController;    
    public ParcelSpawner _parcelSpawner;
    public GameObject _workerLeft;
    public GameObject _workerRight;
    public GameObject _truck;
    public List<GameObject> _conveyorBelts = new List<GameObject>();

    public int LivesRemaining { get; private set; }
    public int ParcelsLoadedTotal { get; private set; }
    public int ParcelsLoadedOnCurrentTruck { get; private set; }

    public GameState CurrentState
    {
        get { return _currentState; }
        private set
        {
            EventManager.Instance.TriggerGameStateChanged(_currentState, value);
            _currentState = value;
        }
    }

	#region Mono Behaviours

	void Start()
	{
		// Start the game
        StartNewGame();
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
        // Reset the game
        ResetGame();        

        // Start the game
        ActivateGameObjects();

        // Set game state
        CurrentState = GameState.Active;
    }

    #endregion

    #region Private Methods

    // Resets the game to default values
    void ResetGame()
    {
        // Reset game manager variables
        LivesRemaining = Constants.Game.startingLives;
        ParcelsLoadedTotal = 0;
        ParcelsLoadedOnCurrentTruck = 0;

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

        // Reset the score
        ScoreManager.Instance.Reset();

        // Ask the level manager to go back to starting level
        LevelManager.Instance.Reset();

        // Apply the level settings to the game
        ApplyLevelSettingsToGame();        
    }

    // Handles the event of a worker dropping a parcel
	void HandleDroppedParcel()
	{
        // Ensure the game has not already ended
        if (CurrentState == GameState.Active)
        {
            // Decrement worker life counter
            LivesRemaining -= 1;

            // See if the game is over
            if (LivesRemaining == 0)
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
        // Set game state
        CurrentState = GameState.LostLife;

        // Pause the game
        PauseGame();

        // Get the HUD to display a countdown
        _hudController.ShowCountdown(Constants.Game.lifeLostPauseLength, Constants.Game.lifeLostCountdownLength);

        // Kick off a coroutine to pause until the game starts again
        StartCoroutine(CountdownToRestartLevelFollowingLostLife());
    }

    // Counts down to the game restart used in a coroutine
    IEnumerator CountdownToRestartLevelFollowingLostLife()
    {
        // Iterate through the pause length to hold the game
        for (float timer = Constants.Game.lifeLostPauseLength; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }

        // Restart the level
        RestartLevelFollowingLostLife();
    }

    // Starts the next level
    void RestartLevelFollowingLostLife()
    {
        // Unpause the game
        UnpauseGame();
    }

    // Handles the event of a worker loading a parcel onto the truck
	void HandleLoadedParcel()
	{
		// Increment loaded parcels counter
		ParcelsLoadedTotal += 1;
        ParcelsLoadedOnCurrentTruck += 1;

		// See if the truck is full
        if (ParcelsLoadedOnCurrentTruck == LevelManager.Instance.CurrentLevel.TruckCapacity)
        {
            // Tell the truck to deliver some parcels
            _truck.GetComponent<TruckController>().DeliverParcels();

            // Player has reached the next level
            MoveToNextLevel();
        }		
	}

    // Moves the worker to the next level
    void MoveToNextLevel()
    {
        // Set game state
        CurrentState = GameState.LevelCompleted;

        // Pause the game
        PauseGame();

        // Make the workers take their break
        _workerLeft.GetComponent<WorkerController>().TakeBreak();
        _workerRight.GetComponent<WorkerController>().TakeBreak();

        // Get the HUD to display a countdown
        _hudController.ShowCountdown(Constants.Game.levelUpPauseLength, Constants.Game.levelUpCountdownLength);

        // Kick off a coroutine to pause until the next level starts
        StartCoroutine(CountdownToNextLevel());
    }

    // Counts down to the next level used in a coroutine
    IEnumerator CountdownToNextLevel()
    {
        // Iterate through the pause length to hold the game
        for (float timer = Constants.Game.levelUpPauseLength; timer >= 0; timer -= Time.deltaTime)
        {
            yield return 0;
        }        

        // Move to next level
        StartNextLevel();
    }

    // Starts the next level
    void StartNextLevel()
    {
        // Up the level
        LevelManager.Instance.LevelUp();

        // Apply the level settings to the game
        ApplyLevelSettingsToGame();

        // Reset counter
        ParcelsLoadedOnCurrentTruck = 0;

        // Make the workers get back to work
        _workerLeft.GetComponent<WorkerController>().GetBackToWork();
        _workerRight.GetComponent<WorkerController>().GetBackToWork();

        // Unpause the game
        UnpauseGame();

        // Trigger the level up event
        EventManager.Instance.TriggerLevelUp(LevelManager.Instance.CurrentLevel);
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
        // Set game state
        CurrentState = GameState.GameOver;

		// Pause the game
        PauseGame();
    }

    // Pauses the game
    void PauseGame()
    {      
        // Set game state
        if(CurrentState == GameState.Active)
        {
            CurrentState = GameState.Paused;
        }        

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

        // Set game state
        CurrentState = GameState.Active;
    }

    // Activates the game objects
    void ActivateGameObjects()
    {
        // Enable the conveyor belts
        foreach (var conveyorBelt in _conveyorBelts)
        {
            conveyorBelt.GetComponent<ConveyorBeltController>().StartConveyorBelt();
        }

        // Unfreeze all dropped parcels
        foreach (var parcel in GameObject.FindGameObjectsWithTag(Tags.parcel))
        {
            if (parcel.GetComponent<ParcelController>().State == ParcelState.Dropped)
            {
                // Oh dear the players about to loose another life!
                parcel.GetComponent<ParcelController>().Unfreeze();
            }
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

        // Freeze all dropped parcels
        foreach (var parcel in GameObject.FindGameObjectsWithTag(Tags.parcel))
        {
            if (parcel.GetComponent<ParcelController>().State == ParcelState.Dropped)
            {
                parcel.GetComponent<ParcelController>().Freeze();
            }
        }

        // Disable the workers
        _workerLeft.GetComponent<WorkerController>().Active = false;
        _workerRight.GetComponent<WorkerController>().Active = false;

        // Stop the parcel spawner
        _parcelSpawner.StopSpawning();
    }
    
	#endregion
}
