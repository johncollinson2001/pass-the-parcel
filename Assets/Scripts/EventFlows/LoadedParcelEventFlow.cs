using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadedParcelEventFlow : MonoBehaviour 
{
    public PanelController _panelController;
    public WorkerController _workerLeft;
    public WorkerController _workerRight;
    public TruckController _truck;

	#region Mono Behaviours

    void OnEnable()
    {
        EventManager.Instance.ParcelLoaded += HandleLoadedParcel;
    }

    void OnDisable()
    {
        EventManager.Instance.ParcelLoaded -= HandleLoadedParcel;
    }

	#endregion	

    #region Private Methods

    // Handles the event of a worker loading a parcel onto the truck
    void HandleLoadedParcel(GameObject parcel)
    {
        // Increment loaded parcels counter
        ScoreManager.Instance.ParcelsLoadedTotal += 1;
        ScoreManager.Instance.ParcelsLoadedOnCurrentTruck += 1;

        // See if the truck is full
        if (ScoreManager.Instance.ParcelsLoadedOnCurrentTruck == LevelManager.Instance.CurrentLevel.TruckCapacity)
        {                
            // Player has reached the next level
            MoveToNextLevel();
        }
    }

    // Moves the worker to the next level
    void MoveToNextLevel()
    {
        // Pause the game
        GameManager.Instance.PauseGame();

        // Make the workers take their break
        _workerLeft.TakeBreak();
        _workerRight.TakeBreak();

        if (GameManager.Instance.Player.IsHuman)
        {
            // Tell the truck to deliver some parcels
            _truck.DeliverParcels();

            // Get the panel to display 
            _panelController.ShowLevelCompletedPanel(Constants.Game.levelUpPauseLength);

            // Kick off a coroutine to pause until the next level starts
            StartCoroutine(CountdownToNextLevel());
        }
        else
        {
            // Start the next level immediately for the ai player
            StartNextLevel();
        }
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

        // Make the workers get back to work
        _workerLeft.GetBackToWork();
        _workerRight.GetBackToWork();

        // Reset counter
        ScoreManager.Instance.ParcelsLoadedOnCurrentTruck = 0;

        // Hide the panel 
        _panelController.HidePanel();

        // Unpause the game
        GameManager.Instance.UnpauseGame();

        // Trigger the level up event
        EventManager.Instance.TriggerLevelUp(LevelManager.Instance.CurrentLevel);
    }

    #endregion
}