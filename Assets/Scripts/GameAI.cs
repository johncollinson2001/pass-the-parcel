using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is ropey at best, but would require a more fundamental rewrite of other parts of the game
/// until it is totally robust. It plays the game for quite a while though but gets a bit confused when the 
/// conveyors get really fast!
/// </summary>
public class GameAI : MonoSingleton<GameAI>
{
    private bool _active;
    private HashSet<ParcelController> _parcelsInQueue = new HashSet<ParcelController>();
    private Queue<ParcelController> _parcelsToPass = new Queue<ParcelController>();

    public WorkerController _workerLeft;
    public WorkerController _workerRight;

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
        ParcelController parcelController = parcel.GetComponent<ParcelController>();
        if (_parcelsInQueue.Add(parcelController))
        {
            _parcelsToPass.Enqueue(parcelController);
        }
    }

    // Removes the parcel from the queue
    void RemoveParcelFromQueue(GameObject parcel)
    {
        // Get rid of the freaking parcel
        try
        {
            ParcelController parcelController = parcel.GetComponent<ParcelController>();
            if (_parcelsToPass.Count > 0 && _parcelsToPass.Peek() == parcelController)
            {
                _parcelsToPass.Dequeue();
                _parcelsInQueue.Remove(parcelController);
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
            foreach(ParcelController parcel in _parcelsToPass)
            {
                // Pull out the platform that a worker needs to be standing on to collect this parcel
                PlatformController receivingPlatformController = GetReceivingPlatformForConveyor(parcel.ConveyorBelt);
                    
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
        if (!_workerLeft.Jumping)
        {
            // See if we're moving the worker up or down
            if (_workerLeft.CurrentPlatform._platformLevel < levelToMoveTo)
            {
                _workerLeft.MoveWorkerUp();
            }
            else
            {
                _workerLeft.MoveWorkerDown();
            }
        }
    }

    void MoveWorkerRight(PlatformLevel levelToMoveTo)
    {
        // Check the worker is not already jumping
        if (!_workerRight.Jumping)
        {
            // See if we're moving the worker up or down
            if (_workerRight.CurrentPlatform._platformLevel < levelToMoveTo)
            {
                _workerRight.MoveWorkerUp();
            }
            else
            {
                _workerRight.MoveWorkerDown();
            }
        }
    }

    // Ronseal Method. Does as it says on the tin.
    PlatformController GetReceivingPlatformForConveyor(ConveyorBeltController conveyorBelt)
    {
        // Iterate over all platforms
        foreach (GameObject platform in GameObject.FindGameObjectsWithTag(Tags.workerPlatform))
        {
            // Test to see if the conveyor the platform receives from is the conveyor of the parcel
            PlatformController platformController = platform.GetComponent<PlatformController>();
            if (platformController._receiveFromConveyor == conveyorBelt)
            {
                return platformController;
            }
        }

        return null;
    }

    #endregion
}