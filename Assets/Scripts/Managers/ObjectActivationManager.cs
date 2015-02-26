using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectActivationManager : MonoBehaviour 
{
    private static ObjectActivationManager _instance;

    public GameObject _workerLeft;
    public GameObject _workerRight;
    public List<GameObject> _conveyorBelts = new List<GameObject>();

    public static ObjectActivationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType(typeof(ObjectActivationManager)) as ObjectActivationManager;

                if (_instance == null)
                {
                    GameObject go = new GameObject("GameObjectActivationManager");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<ObjectActivationManager>();
                }
            }
            return _instance;
        }
    }    

    #region Public Methods

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
