using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelManager 
{
    private static LevelManager _instance;

    public LevelModel CurrentLevel { get; private set; }

    public static LevelManager Instance
    {
        get
        {
            // Instantiate instance if not created
            if(_instance == null)
            {
                _instance = new LevelManager();

                // Setup the current level as the starting level 
                _instance.CurrentLevel = LevelGenerator.CreateStartingLevel();

                // Subscribe to the debugger event
                EventManager.Instance.DebugWrite += _instance.WriteTextToDebugger;
            }

            return _instance;
        }
    }

    // Singleton constructor
    private LevelManager() { }

    #region Public Methods

    // Moves the current level to the next level
    public void LevelUp()
    {
        // Create the next level
        LevelModel nextLevel = LevelGenerator.CreateNextLevel(CurrentLevel);

        // Set the current level as the next level we have just created
        CurrentLevel = nextLevel;
    }

    // Resets the level manager
    public void Reset()
    {
        // Setup the current level as the starting level 
        _instance.CurrentLevel = LevelGenerator.CreateStartingLevel();
    }

    #endregion

    #region Private Methods

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