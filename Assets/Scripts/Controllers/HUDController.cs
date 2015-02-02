using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDController : MonoBehaviour 
{
	public GUIText _livesRemainingText;
    public GUIText _gameOverText;
    public GUIText _levelText;
    public GUIText _truckCapacityText;
    public GUIText _parcelLoadedOnTruckText;
    public GUIText _countdownText;
    public GUIText _levelCompletedText;
    public GUIText _lifeLostText;

	#region Mono Behaviours

	void Start()
	{
        // Clear down the temporary text
        ClearText();
	}

	#endregion	

	#region Public Methods

	public void UpdateHUD(string livesRemaining, string levelNumber, string truckCapacity, string parcelsLoadedOnCurrentTruck)
	{
		_livesRemainingText.text = string.Format(Messages.livesRemaining, livesRemaining);
        _levelText.text = string.Format(Messages.level, levelNumber);
        _truckCapacityText.text = string.Format(Messages.truckCapacity, truckCapacity);
        _parcelLoadedOnTruckText.text = string.Format(Messages.parcelsLoadedOnTruck, parcelsLoadedOnCurrentTruck);
	}

    // Writes the game over text
    public void WriteGameOverText()
    {
        _gameOverText.text = Messages.gameOver;
    }

    // Writes the level up text to the screen
    public void WriteLevelUpTextToScreen(string levelNumber, string nextLevelNumber)
    {
        // Make text show on the screen to say the level has been completed
        _levelCompletedText.text = string.Format(Messages.levelCompleted, levelNumber, nextLevelNumber);
    }

    // Writes the life lost text to the screen
    public void WriteLifeLostTextToScreen(string livesRemaining)
    {
        // Make text show on the screen to say a life has been lost
        _lifeLostText.text = string.Format(Messages.lostLife, livesRemaining);
    }

    // Writes a countdown to screen
    public void WriteCountdownToScreen(int countdownSteps, float countdownTimer)
    {
        for (int i = countdownSteps; i > 0; i--)
        {
            if(countdownTimer <= i && countdownTimer > (i - 1))
            {
                _countdownText.text = i.ToString();
            }
        }
    }

    // Clears down the temporary text
    public void ClearText()
    {
        _countdownText.text = string.Empty;
        _gameOverText.text = string.Empty;
        _lifeLostText.text = string.Empty;
        _levelCompletedText.text = string.Empty;

    }    
    
	#endregion
}
