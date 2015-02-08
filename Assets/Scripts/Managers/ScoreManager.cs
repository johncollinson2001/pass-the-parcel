using UnityEngine;
using System;
using System.Collections.Generic;

public class ScoreManager 
{
    private static ScoreManager _instance;

    public int CurrentScore { get; private set; }

    public static ScoreManager Instance
    {
        get
        {
            // Instantiate instance if not created
            if(_instance == null)
            {
                _instance = new ScoreManager();

                // Register events
                EventManager.Instance.ParcelPassed += _instance.RegisterScoreForParcelPassed;
                EventManager.Instance.ParcelLoaded += _instance.RegisterScoreForParcelLoaded;
                EventManager.Instance.LevelUp += _instance.RegisterScoreForLevelUp;
            }

            return _instance;
        }
    }

    // Singleton constructor
    private ScoreManager() { }

    #region Private Methods    

    // Resets the Score manager
    public void Reset()
    {
        // Setup the current Score back to zero
        CurrentScore = 0;
    }

    #endregion

    #region Private Methods

    void RegisterScoreForParcelPassed(GameObject parcel)
    {
        // For a parcel passed, user gets X points multiplied by the level they are on
        CurrentScore += Constants.Scores.passedParcel * LevelManager.Instance.CurrentLevel.LevelNumber;
    }

    void RegisterScoreForLevelUp(LevelModel nextLevel)
    {
        // For a level up, user gets X points multiplied by the level they are on
        CurrentScore += Constants.Scores.levelUp * (nextLevel.LevelNumber - 1);
    }

    void RegisterScoreForParcelLoaded()
    {
        // For a parcel loaded, user gets X points multiplied by the level they are on
        CurrentScore += Constants.Scores.parcelLoaded * LevelManager.Instance.CurrentLevel.LevelNumber;
    }

    #endregion
}