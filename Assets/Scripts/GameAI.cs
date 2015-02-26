using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is ropey at best, but would require a more fundamental rewrite of other parts of the game
/// until it is totally robust. It plays the game for quite a while though but gets a bit confused when the 
/// conveyors get really fast!
/// </summary>
public class GameAI : MonoBehaviour
{
    private bool _active;
    private HashSet<GameObject> _parcelsInQueue = new HashSet<GameObject>();
    private Queue<GameObject> _parcelsToPass = new Queue<GameObject>();

    public GameObject _workerLeft;
    public GameObject _workerRight;

    #region Mono Behaviours

    void Update()
    {
        // Play the game if the player is not human
        if(_active)
        {
            PlayGame();
        }
    }

    #endregion

    #region Public Methods

    public void Activate()
    {
        _active = true;
        EventManager.Instance.ParcelAboutToDrop += AddParcelToQueue;
        EventManager.Instance.ParcelDropped += RemoveParcelFromQueue;
        EventManager.Instance.GameOver += Reset;
        EventManager.Instance.LifeLost += Reset;
    }

    public void Deactivate()
    {
        _active = false;
        Reset();
        EventManager.Instance.ParcelAboutToDrop -= AddParcelToQueue;
        EventManager.Instance.ParcelDropped -= RemoveParcelFromQueue;
        EventManager.Instance.GameOver -= Reset;
        EventManager.Instance.LifeLost -= Reset;
    }

    #endregion

    #region Private Methods

    void Reset()
    {
        _parcelsInQueue.Clear();
        _parcelsToPass.Clear();
    }

    // Chucks the parcel onto the queue
    void AddParcelToQueue(GameObject parcel)
    {
        // Add parcel to hashset of those already in the queue
        // This statement will return false if the parcel has already been queued
        if (_parcelsInQueue.Add(parcel))
        {
            _parcelsToPass.Enqueue(parcel);
        }
    }

    // Removes the parcel from the queue
    void RemoveParcelFromQueue(GameObject parcel)
    {
        // Get rid of the freaking parcel
        try
        {
            if (_parcelsToPass.Count > 0 && _parcelsToPass.Peek() == parcel)
            {
                _parcelsToPass.Dequeue();
                _parcelsInQueue.Remove(parcel);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    void PlayGame()
    {
        // Check to see if any parcels to pass
        if (_parcelsToPass.Count > 0)
        {
            bool handledWorkerLeft = false;
            bool handledWorkerRight = false;
            foreach(GameObject parcel in _parcelsToPass)
            {
                // Pull out the platform that a worker needs to be standing on to collect this parcel
                ParcelController parcelController = parcel.GetComponent<ParcelController>();
                PlatformController receivingPlatformController = GetReceivingPlatformForConveyor(parcelController.ConveyorBelt).GetComponent<PlatformController>();
                    
                // Work out which worker needs to move
                if (receivingPlatformController._screenSide == ScreenSide.Left && !handledWorkerLeft)
                {
                    handledWorkerLeft = true;

                    // Check to see if we need to move a worker to collect the parcel
                    if (!receivingPlatformController.HasWorker)
                    {
                        // Move worker left
                        MoveWorkerLeft(receivingPlatformController._platformLevel);
                    }
                }
                else if (receivingPlatformController._screenSide == ScreenSide.Right && !handledWorkerRight)
                {
                    handledWorkerRight = true;

                    // Check to see if we need to move a worker to collect the parcel
                    if (!receivingPlatformController.HasWorker)
                    {
                        // Move worker right
                        MoveWorkerRight(receivingPlatformController._platformLevel);
                    }
                }

                // If we have moved both workers, then we can exit this loop
                if(handledWorkerLeft && handledWorkerRight)
                {
                    break;
                }
            }            
        }
    }

    void MoveWorkerLeft(PlatformLevel levelToMoveTo)
    {
        // Check the worker is not already jumping
        WorkerController worker = _workerLeft.GetComponent<WorkerController>();
        if (!worker.Jumping)
        {
            // See if we're moving the worker up or down
            if (_workerLeft.GetComponent<WorkerController>().CurrentPlatform.GetComponent<PlatformController>()._platformLevel < levelToMoveTo)
            {
                _workerLeft.GetComponent<WorkerController>().MoveWorkerUp();
            }
            else
            {
                _workerLeft.GetComponent<WorkerController>().MoveWorkerDown();
            }
        }
    }

    void MoveWorkerRight(PlatformLevel levelToMoveTo)
    {
        // Check the worker is not already jumping
        WorkerController worker = _workerRight.GetComponent<WorkerController>();
        if (!worker.Jumping)
        {
            // See if we're moving the worker up or down
            if (_workerRight.GetComponent<WorkerController>().CurrentPlatform.GetComponent<PlatformController>()._platformLevel < levelToMoveTo)
            {
                _workerRight.GetComponent<WorkerController>().MoveWorkerUp();
            }
            else
            {
                _workerRight.GetComponent<WorkerController>().MoveWorkerDown();
            }
        }
    }

    // Ronseal Method. Does as it says on the tin.
    GameObject GetReceivingPlatformForConveyor(GameObject conveyorBelt)
    {
        // Iterate over all platforms
        foreach (GameObject platform in GameObject.FindGameObjectsWithTag(Tags.workerPlatform))
        {
            // Test to see if the conveyor the platform receives from is the conveyor of the parcel
            GameObject receiveFromConveyorForPlatfrom = platform.GetComponent<PlatformController>()._receiveFromConveyor;
            if (receiveFromConveyorForPlatfrom == conveyorBelt)
            {
                return platform;
            }
        }

        return null;
    }

    #endregion
}