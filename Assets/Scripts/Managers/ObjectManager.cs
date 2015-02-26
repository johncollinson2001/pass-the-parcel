using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoSingleton<ObjectManager> 
{
    public WorkerController _workerLeft;
    public WorkerController _workerRight;
    public List<ConveyorBeltController> _conveyorBelts;
    public ParcelSpawner _parcelSpawner;

    #region Public Methods

    // Resets all the game objects
    public void ResetGameObjects()
    {
        // Clear all parcels from the conveyors
        foreach (var conveyor in _conveyorBelts)
        {
            conveyor.ClearParcels();
        }

        // Destroy any other parcels in the scene (i.e. dropped parcels)
        foreach (var parcel in GameObject.FindGameObjectsWithTag(Tags.parcel))
        {
            Destroy(parcel);
        }

        // Reset the workers
        _workerLeft.Reset();
        _workerRight.Reset();

        // Reset the parcel spawner        
        _parcelSpawner.Reset();
    }

    public void PauseGameObjects()
    {
        // Pause the parcel spawner
        _parcelSpawner.PauseSpawning();

        DeactivateConveyorBelts();
        DeactivateWorkers();
    }

    public void UnpauseGameObjects()
    {
        // Unpause the parcel spawner
        _parcelSpawner.UnpauseSpawning();

        ActivateConveyorBelts();
        ActivateWorkers();
    }

    public void StartGameObjects()
    {
        // Enable the parcel spawner
        _parcelSpawner.StartSpawning();

        ActivateConveyorBelts();
        ActivateWorkers();
    }

    public void StopGameObjects()
    {
        // Stop the parcel spawner
        _parcelSpawner.StopSpawning();

        DeactivateConveyorBelts();
        DeactivateWorkers();
    }

    #endregion

    #region Private Methods

    void ActivateConveyorBelts()
    {
        // Enable the conveyor belts
        foreach (var conveyorBelt in _conveyorBelts)
        {
            conveyorBelt.StartConveyorBelt();
        }

        // Unfreeze all dropped parcels
        foreach (var parcel in GameObject.FindGameObjectsWithTag(Tags.parcel))
        {
            ParcelController parcelController = parcel.GetComponent<ParcelController>();
            if (parcelController.State == ParcelState.Dropped)
            {
                // Oh dear the players about to loose another life!
                parcelController.Unfreeze();
            }
        }
    }

    void ActivateWorkers()
    {
        // Enable the workers
        _workerLeft.Active = true;
        _workerRight.Active = true;
    }

    void DeactivateConveyorBelts()
    {
        // Disable the conveyor belts
        foreach (var conveyorBelt in _conveyorBelts)
        {
            conveyorBelt.StopConveyorBelt();
        }

        // Freeze all dropped parcels
        foreach (var parcel in GameObject.FindGameObjectsWithTag(Tags.parcel))
        {
            ParcelController parcelController = parcel.GetComponent<ParcelController>();
            if (parcelController.State == ParcelState.Dropped)
            {
                parcelController.Freeze();
            }
        }
    }

    void DeactivateWorkers()
    {
        // Disable the workers
        _workerLeft.Active = false;
        _workerRight.Active = false;        
    }
    
	#endregion
}
