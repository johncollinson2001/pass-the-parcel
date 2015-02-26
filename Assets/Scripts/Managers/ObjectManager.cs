using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour 
{
    private static ObjectManager _instance;

    public GameObject _workerLeft;
    public GameObject _workerRight;
    public List<GameObject> _conveyorBelts = new List<GameObject>();

    public static ObjectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType(typeof(ObjectManager)) as ObjectManager;

                if (_instance == null)
                {
                    GameObject go = new GameObject("ObjectManager");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<ObjectManager>();
                }
            }
            return _instance;
        }
    }    

    #region Public Methods

    // Resets all the game objects
    public void ResetGameObjects()
    {
        // Clear all parcels from the conveyors
        foreach (var conveyor in _conveyorBelts)
        {
            conveyor.GetComponent<ConveyorBeltController>().ClearParcels();
        }

        // Destroy any other parcels in the scene (i.e. dropped parcels)
        foreach (var parcel in GameObject.FindGameObjectsWithTag(Tags.parcel))
        {
            Destroy(parcel);
        }

        // Reset the workers
        _workerLeft.GetComponent<WorkerController>().Reset();
        _workerRight.GetComponent<WorkerController>().Reset();
    }

    // Activates the game objects
    public void ActivateGameObjects()
    {
        // Enable the conveyor belts
        foreach (var conveyorBelt in _conveyorBelts)
        {
            conveyorBelt.GetComponent<ConveyorBeltController>().StartConveyorBelt();
        }

        // Unfreeze all dropped parcels
        foreach (var parcel in GameObject.FindGameObjectsWithTag(Tags.parcel))
        {
            if (parcel.GetComponent<ParcelController>().State == ParcelState.Dropped)
            {
                // Oh dear the players about to loose another life!
                parcel.GetComponent<ParcelController>().Unfreeze();
            }
        }

        // Enable the workers
        _workerLeft.GetComponent<WorkerController>().Active = true;
        _workerRight.GetComponent<WorkerController>().Active = true;        
    }

    // Deactivates the game objects
    public void DeactivateGameObjects()
    {
        // Disable the conveyor belts
        foreach (var conveyorBelt in _conveyorBelts)
        {
            conveyorBelt.GetComponent<ConveyorBeltController>().StopConveyorBelt();
        }

        // Freeze all dropped parcels
        foreach (var parcel in GameObject.FindGameObjectsWithTag(Tags.parcel))
        {
            if (parcel.GetComponent<ParcelController>().State == ParcelState.Dropped)
            {
                parcel.GetComponent<ParcelController>().Freeze();
            }
        }

        // Disable the workers
        _workerLeft.GetComponent<WorkerController>().Active = false;
        _workerRight.GetComponent<WorkerController>().Active = false;        
    }
    
	#endregion
}
