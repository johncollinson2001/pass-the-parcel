﻿using UnityEngine;
using System.Collections;

public class ParcelSpawner : MonoBehaviour 
{
	private float _nextSpawn = Constants.ParcelSpawner.startSpawningBuffer;
	private int _spawnCount;
    private int _spawnCountWhenWaveCompleted;
    private int _spawnsInCurrentWave;
    private bool _isWaveActive;
    private bool _alerting;
    private float _nextSpawnPauseBuffer;

	public GameObject _parcel;
    public GameObject _parcelAlert;

    public bool Operational { get; private set; }
    public bool Paused { get; private set; }
	public float SpawnRate { get; set; } 
    public int MinimumSpawnsPerWave { get; set; }
    public int MaximumSpawnsPerWave { get; set; }
    public float SpawnWaveGap { get; set; }
    public bool UseSpawnRateRandomiser { get; set; }

    #region Mono Behaviours

	void Update ()
	{
		// Check the spawner is operational
		if (Operational && !Paused)
		{
            // Check if its time to spawn a parcel
            if (Time.time > _nextSpawn)
            {
                // Hide the alert
                HideAlert();

                // Spawn a parcel
                SpawnParcel();
            } 
            // Check if it's time to alert that a spawn wave is iminent
            else if (!_isWaveActive && !_alerting && Time.time > (_nextSpawn - Constants.ParcelSpawner.spawnWaveAlertTime))
            {
                ShowAlert();
            }
		}
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

    // Starts the spawner
    public void StartSpawning()
    {
        Operational = true;
    }

    // Stops the spawner
    public void StopSpawning()
    {
        Operational = false;
    }

    // Unpauses the spawner
    public void UnpauseSpawning()
    {        
        // Set the next spawn to time + the buffer
        _nextSpawn = Time.time + _nextSpawnPauseBuffer;

        // Set the paused flag
        Paused = false;
    }

    // Stops the spawner
    public void PauseSpawning()
    {        
        // Set the pause buffer so we can add it back onto next spawn when the spawner is started again
        _nextSpawnPauseBuffer = _nextSpawn - Time.time;

        // Set the paused flag
        Paused = true;
    }

    // Resets the spawner including the next spawn, spawn count and wave values
    public void Reset()
    {        
        _spawnCount = 0;
        _spawnCountWhenWaveCompleted = 0;
        _nextSpawnPauseBuffer = 0;
        _isWaveActive = false;
        _nextSpawn = Time.time + Constants.ParcelSpawner.startSpawningBuffer;
        Paused = false;
        HideAlert();
    }

    #endregion

    #region Private Methods

    // Shows the parcel alert
    void ShowAlert()
    {
        _alerting = true;
        _parcelAlert.SetActive(true);

        // Make alert blink
        StartCoroutine(FlashAlert());
    }

    // Hides the parcel alert
    void HideAlert() 
    {
        _alerting = false;
        _parcelAlert.SetActive(false);

        // Stop all coroutines incase we are half way through a flashing sequence
        StopAllCoroutines();
    }

    // Makes the alert flash
    IEnumerator FlashAlert()
    {
        // Iterate through the pause length to hold the game
        for (float i = 0; i < Constants.ParcelSpawner.spawnWaveAlertTime; i += Constants.ParcelSpawner.spawnAlertBlinkSpeed)
        {            
            // If the game is paused, we need to pause the alert
            if (Paused)
            {
                // This code will make the coroutine infinite until the game is unpaused
                i -= Constants.ParcelSpawner.spawnAlertBlinkSpeed;
            }
            else
            {
                // Make the alert game object blink on/off
                _parcelAlert.GetComponent<Renderer>().enabled = !_parcelAlert.GetComponent<Renderer>().enabled;
            }

            yield return new WaitForSeconds(Constants.ParcelSpawner.spawnAlertBlinkSpeed);
        }
    }

    // Spawns a new parcel
    void SpawnParcel()
    {
        // If there is not an active wave then setup a spawn wave
        if (!_isWaveActive)
        {
            CreateNewWave();
        }        

        // Create a new parcel
        GameObject newParcel = (GameObject)Instantiate(_parcel, transform.position, transform.rotation);
        newParcel.name = "Parcel " + _spawnCount.ToString();

        // Increment spawn count
        _spawnCount += 1;        

        // Set the next spawn time
        // ... Check to see if the spawn wave has been completed
        if (_spawnCount == _spawnCountWhenWaveCompleted)
        {
            // Wave completed... start the next spawn after the wave gap
            _nextSpawn = Time.time + SpawnWaveGap;

            // End the current wave
            EndCurrentWave();
        }
        else
        {
            // Spawning as part of a wave.. start the next spawn at the spawn rate
            // if specified by the flag, chuck in some randomness to make things interesting
            if (UseSpawnRateRandomiser)
            {
                // Randomise between range of the spawn rate i.e...
                // if spawn rate = 5, randomise between 2.5 and 7.5
                _nextSpawn = Time.time + Random.Range(SpawnRate / 2f, SpawnRate * 1.5f);
            }
            else
            {
                _nextSpawn = Time.time + SpawnRate;
            }
        }	
    }

    void EndCurrentWave()
    {
        // Decrement the spawn wave gap so the waves get progressively closer together
        SpawnWaveGap -= Constants.ParcelSpawner.reduceSpawnWaveGapModifier;

        // Set the flag for an active wave to false
        _isWaveActive = false;
    }

    // Sets up a new spawn wave
    void CreateNewWave()
    {
        // Set the current wave count
        // NOTE: Upper bound is exclusive, so add one to max spawns per wave
        _spawnsInCurrentWave = Random.Range(MinimumSpawnsPerWave, MaximumSpawnsPerWave + 1);

        // Increment the spawns per wave onto the counter that indicates how many spawns there
        // will have been when the wave is completed
        _spawnCountWhenWaveCompleted += _spawnsInCurrentWave;

        // Set the flag to mark an active wave in in place
        _isWaveActive = true;
    }

    // Writes the parcel spawner data to the debugger
    void WriteTextToDebugger(GUIText guiText)
    {
        guiText.text += "Parcel Spawner Info";
        guiText.text += "\nOperational: " + Operational.ToString();
        guiText.text += "\nTime: " + Time.time;
        guiText.text += "\nNext Spawn: " + _nextSpawn.ToString();
        guiText.text += "\nActive wave: " + _isWaveActive.ToString();
        guiText.text += "\nCurrent wave count: " + _spawnsInCurrentWave.ToString();
        guiText.text += "\nSpawn Count: " + _spawnCount.ToString();
        guiText.text += "\nSpawn Count When Wave Completed: " + _spawnCountWhenWaveCompleted.ToString();
        guiText.text += "\nSpawn Rate: " + SpawnRate.ToString();
        guiText.text += "\nMax Spawns Per Wave: " + MaximumSpawnsPerWave.ToString();
        guiText.text += "\nMin Spawns Per Wave: " + MinimumSpawnsPerWave.ToString();
        guiText.text += "\nSpawn Wave Gap: " + SpawnWaveGap.ToString();
        guiText.text += "\nUse Randomiser: " + UseSpawnRateRandomiser.ToString();
        guiText.text += "\n\n";
    }

    #endregion
}
