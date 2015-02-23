using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

public class ScoreManager 
{
    private static ScoreManager _instance;

    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }

    public static ScoreManager Instance
    {
        get
        {
            // Instantiate instance if not created
            if(_instance == null)
            {
                _instance = new ScoreManager();

                // Get the current high score
                _instance.HighScore = _instance.GetSavedHighScore();

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

    // Saves the high score the current score is higher
    public void SetHighScore()
    {
        // If the score of this game > the current high score
        if (HighScore < CurrentScore)
        {
            // Set the high score
            HighScore = CurrentScore;

            // Persist the high score
            SaveHighScore();
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

    // Sets the high score property from the xml file
    int GetSavedHighScore()
    {
        var serializer = new XmlSerializer(typeof(HighScoresModel));
        var stream = new FileStream(Constants.Scores.highScoreXmlPath, FileMode.Open);
        HighScoresModel highScores = serializer.Deserialize(stream) as HighScoresModel;        
        stream.Close();

        return highScores.HighScore;
    }

    void SaveHighScore()
    {
        // Write the high scores model to the xml file - overwriting as we go
        var serializer = new XmlSerializer(typeof(HighScoresModel));
        var stream = new FileStream(Constants.Scores.highScoreXmlPath, FileMode.Create);
        using (XmlWriter writer = XmlWriter.Create(stream))
        {
            // Create a new high scores model
            HighScoresModel highScores = new HighScoresModel()
            {
                HighScore = HighScore
            };
            // Save the model
            serializer.Serialize(writer, highScores);
        }
        stream.Close();
    }
    
    #endregion
}