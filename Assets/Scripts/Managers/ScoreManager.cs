using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

public class ScoreManager 
{
    private static ScoreManager _instance = new ScoreManager();
    public static ScoreManager Instance { get { return _instance; } }

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }
    public int LivesRemaining { get; set; }
    public int ParcelsLoadedTotal { get; set; }
    public int ParcelsLoadedOnCurrentTruck { get; set; }

    // Singleton constructor
    private ScoreManager() 
    {
        // Get the current high score
        HighScore = PlayerPrefs.GetInt("HighScore");

        // Register events
        EventManager.Instance.ParcelPassed += RegisterScoreForParcelPassed;
        EventManager.Instance.ParcelLoaded += RegisterScoreForParcelLoaded;
        EventManager.Instance.LevelUp += RegisterScoreForLevelUp;
    }

    #region Private Methods    

    // Resets the Score manager
    public void Reset()
    {
        CurrentScore = 0;
        LivesRemaining = Constants.Game.startingLives;
        ParcelsLoadedTotal = 0;
        ParcelsLoadedOnCurrentTruck = 0;
    }

    // Saves the high score the current score is higher
    public void SetHighScore()
    {
        // If the score of this game > the current high score
        if (HighScore < CurrentScore)
        {
            // Set the high score
            HighScore = CurrentScore;

            // Persist the high score
            PlayerPrefs.SetInt("HighScore", HighScore);
        }
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

    void RegisterScoreForParcelLoaded(GameObject parcel)
    {
        // For a parcel loaded, user gets X points multiplied by the level they are on
        CurrentScore += Constants.Scores.parcelLoaded * LevelManager.Instance.CurrentLevel.LevelNumber;
    }
    
    #endregion
}