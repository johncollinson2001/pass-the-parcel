using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoSingleton<LevelManager>
{
    public ParcelSpawner _parcelSpawner;
    public List<GameObject> _conveyorBelts = new List<GameObject>();

    public LevelModel CurrentLevel { get; private set; }

    #region Mono Behaviors

    void Awake()
    {
        // Setup the current level as the starting level 
        CurrentLevel = LevelGenerator.CreateStartingLevel();
    }

    void OnEnable()
    {
        EventManager.Instance.DebugWrite += WriteTextToDebugger;
    }

    void OnDisable()
    {
        EventManager.Instance.DebugWrite -= WriteTextToDebugger;
    }

    #endregion

    #region Public Methods

    // Moves the current level to the next level
    public void LevelUp()
    {
        // Create the next level
        LevelModel nextLevel = LevelGenerator.CreateNextLevel(CurrentLevel);

        // Set the current level as the next level we have just created
        CurrentLevel = nextLevel;

        // Apply the new level settings to the game objects
        ApplyCurrentLevelToGame();
    }

    // Resets the level manager
    public void Reset()
    {
        // Setup the current level as the starting level 
        CurrentLevel = LevelGenerator.CreateStartingLevel();

        // Apply the new level settings to the game objects
        ApplyCurrentLevelToGame();
    }

    #endregion

    #region Private Methods

    // Apply level settings to game 
    void ApplyCurrentLevelToGame()
    {
        // Apply the settings of the next level to the game objects
        _parcelSpawner.SpawnRate = CurrentLevel.SpawnRate;
        _parcelSpawner.MinimumSpawnsPerWave = CurrentLevel.MinimumSpawnsPerBurst;
        _parcelSpawner.MaximumSpawnsPerWave = CurrentLevel.MaximumSpawnsPerBurst;
        _parcelSpawner.SpawnWaveGap = CurrentLevel.SpawnBurstGap;
        _parcelSpawner.UseSpawnRateRandomiser = CurrentLevel.UseSpawnRateRandomiser;

        foreach (var conveyorBelt in _conveyorBelts)
        {
            conveyorBelt.GetComponent<ConveyorBeltController>()._speed = CurrentLevel.ConveyorBeltSpeed;
        }
    }

    // Writes the level debug text to the debugger
    void WriteTextToDebugger(GUIText guiText)
    {
        guiText.text += "Level Info";
        guiText.text += "\nLevel: " + CurrentLevel.LevelNumber.ToString();
        guiText.text += "\nConveyor Speed: " + CurrentLevel.ConveyorBeltSpeed.ToString();
        guiText.text += "\nTruck Capacity: " + CurrentLevel.TruckCapacity.ToString();
        guiText.text += "\nSpawn Rate: " + CurrentLevel.SpawnRate.ToString();
        guiText.text += "\nMax Spawns Per Burst: " + CurrentLevel.MaximumSpawnsPerBurst.ToString();
        guiText.text += "\nMin Spawns Per Burst: " + CurrentLevel.MinimumSpawnsPerBurst.ToString();
        guiText.text += "\nSpawn Burst Gap: " + CurrentLevel.SpawnBurstGap.ToString();
        guiText.text += "\nUse Randomiser: " + CurrentLevel.UseSpawnRateRandomiser.ToString();
        guiText.text += "\n\n";
    }

    #endregion
}