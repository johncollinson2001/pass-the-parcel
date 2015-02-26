using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrokenParcelEventFlow : MonoBehaviour 
{
    public PanelController _panelController;

	#region Mono Behaviours

    void OnEnable()
    {
        EventManager.Instance.ParcelBroken += HandleBrokenParcel;
    }

    void OnDisable()
    {
        EventManager.Instance.ParcelBroken -= HandleBrokenParcel;
    }

	#endregion	

    #region Private Methods

    // Handles the event of a worker dropping a parcel
	void HandleBrokenParcel(GameObject parcel)
	{
        // Ensure the game has not already ended
        if (GameManager.Instance.CurrentState == GameState.Active)
        {
            // Decrement worker life counter
            ScoreManager.Instance.LivesRemaining -= 1;

            // See if the game is over
            if (ScoreManager.Instance.LivesRemaining == 0)
            {
                // End the game
                GameManager.Instance.EndGame();
            }
            else
            {
                // Handle the loss of a worker life
                TakeLife();
            }
        }
	}

    void TakeLife()
    {
        // Pause the game
        GameManager.Instance.PauseGame();

        if (GameManager.Instance.Player.IsHuman)
        {
            // show the lost life panel
            _panelController.ShowLostLifePanel(Constants.Game.lifeLostPauseLength);
        }

        // Kick off a coroutine to pause until the game starts again
        StartCoroutine(CountdownToRestartLevelFollowingLostLife());

        // Trigger life lost event
        EventManager.Instance.TriggerLifeLost();
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
        // Hide the panel 
        _panelController.HidePanel();

        // Unpause the game
        GameManager.Instance.UnpauseGame();
    }

    #endregion
}