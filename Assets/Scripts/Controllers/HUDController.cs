﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HUDController : MonoBehaviour 
{
	public GUIText _livesRemainingText;
    public GUIText _levelText;
    public GUIText _truckCapacityText;
    public GUIText _parcelLoadedOnTruckText;
    public GUIText _scoreText;

    public GameManager _gameManager;

	#region Mono Behaviours

    void Update()
    {
        UpdateHUD();
    }

	#endregion	

    #region Private Methods

    // Updates the HUD
    void UpdateHUD()
	{
		_livesRemainingText.text = string.Format(HudText.livesRemaining, _gameManager.LivesRemaining);
        _levelText.text = string.Format(HudText.level, LevelManager.Instance.CurrentLevel.LevelNumber);
        _truckCapacityText.text = string.Format(HudText.truckCapacity, LevelManager.Instance.CurrentLevel.TruckCapacity);
        _parcelLoadedOnTruckText.text = string.Format(HudText.parcelsLoadedOnTruck, _gameManager.ParcelsLoadedOnCurrentTruck);
        _scoreText.text = string.Format(HudText.score, ScoreManager.Instance.CurrentScore);
	}

	#endregion
}
