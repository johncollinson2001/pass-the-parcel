using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorkerController : MonoBehaviour
{
    private Animator _animator;

    public PlatformController _startingPlatform;
    public ScreenSide _playingSide;
    public ScreenSide _facing;

    public PlatformController CurrentPlatform { get; set; }
    public int ParcelsLoaded { get; set; }
    public bool Active { get; set; }
    public bool Jumping { get; set; }

    #region Mono Behaviours

    void Awake()
    {
        // Get the animator and ensure all animation is reset
        _animator = gameObject.GetComponent<Animator>();
        ResetAnimation();
    }
    
    void OnEnable()
    {
        EventManager.Instance.ParcelDropped += PassTheParcel;
        EventManager.Instance.GameOver += KillWorker;
    }

    void OnDisable()
    {
        EventManager.Instance.ParcelDropped -= PassTheParcel;
        EventManager.Instance.GameOver -= KillWorker;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player has landed on a platform
        if (collision.gameObject.tag == Tags.workerPlatform)
        {
            Jumping = false;
            // Turn the jumping animation off
            _animator.SetBool("jump", false);
        }
    }

    #endregion

    #region Public Methods

    // Moves the worker up
    public void MoveWorkerUp()
    {
        Jumping = true;

        switch (CurrentPlatform._platformLevel)
        {
            case PlatformLevel.Bottom:
                MoveWorker(PlatformLevel.Middle);
                break;
            case PlatformLevel.Middle:
                MoveWorker(PlatformLevel.Top);
                break;
        }

        // Turn the worker to face the right direction
        TurnWorkerToFaceConveyorBelt();
    }

    // Moves the worker down
    public void MoveWorkerDown()
    {
        Jumping = true;

        switch (CurrentPlatform._platformLevel)
        {
            case PlatformLevel.Top:
                MoveWorker(PlatformLevel.Middle);
                break;
            case PlatformLevel.Middle:
                MoveWorker(PlatformLevel.Bottom);
                break;
        }

        // Turn the worker to face the right direction
        TurnWorkerToFaceConveyorBelt();
    }

    // Resets the worker
    public void Reset()
    {       
        // Reset animation
        ResetAnimation();

        // Reset the current platform back to starting platform
        CurrentPlatform = _startingPlatform;

        // Move the work to the current platform
        MoveWorker(CurrentPlatform._platformLevel);
    }

    // Makes the worker take a break
    public void TakeBreak()
    {
        _animator.SetBool("take-break", true);
    }

    // Makes the worker get back to work
    public void GetBackToWork()
    {
        _animator.SetBool("take-break", false);
    }

    #endregion

    #region Private Methods

    // Turns the worker to face the receiving conveyor belt of the platform he's standing on
    void TurnWorkerToFaceConveyorBelt()
    {
        // Check if we need to turn the worker round to face the receiving from platform
        if (_facing == ScreenSide.Left
            && CurrentPlatform._receiveFromConveyor._travellingTo == ScreenSide.Left)
        {
            TurnWorker();
        }
        else if (_facing == ScreenSide.Right
            && CurrentPlatform._receiveFromConveyor._travellingTo == ScreenSide.Right)
        {
            TurnWorker();
        }
    }

    // Turns the player to face in the opposite direction
    void TurnWorker()
    {
        // Work out the new x scale
        Vector3 newLocalScale = transform.localScale;
        newLocalScale.x *= -1;       
        transform.localScale = newLocalScale;

        // Transform the x position to align the player exactly to where they are standing
        if (newLocalScale.x > 0)
        {
            transform.position += new Vector3(((newLocalScale.x - 1f) / 2), 0, 0);
        }
        else
        {
            transform.position += new Vector3(((newLocalScale.x + 1f) / 2), 0, 0);
        }

        // Set facing flag
        _facing = (_facing == ScreenSide.Left ? ScreenSide.Right : ScreenSide.Left);
    }

    // Resets the animation
    void ResetAnimation()
    {
        if(_animator != null)
        {            
            _animator.SetBool("take-break", false);
            _animator.SetBool("dead", false);
            _animator.SetBool("jump", false);
        }

        if(_playingSide == ScreenSide.Left && _facing == ScreenSide.Left)
        {
            TurnWorker();
        }
        else if (_playingSide == ScreenSide.Right && _facing == ScreenSide.Right)
        {
            TurnWorker();
        }
    }

    // Looks for game over state change and makes the player die animation 
    void KillWorker()
    {
        // Make the player die animation display
        _animator.SetBool("dead", true);
    }

    // Passes the parecel along the conveyor belts
    void PassTheParcel(GameObject parcel)
	{
		// If the conveyor belt that the parcel is falling from is the conveyor belt
		// that can be received from
        if (parcel.GetComponent<ParcelController>().ConveyorBelt == CurrentPlatform._receiveFromConveyor)
		{                            
            // Turn the worker so it looks like they pass the parcel from one side to the other
            // if the current platform loads the truck (bit of a hack here - as we assume that loading the truck needs a turn)
            // the worker is facing in the opposite direction to the conveyor they must pass to
            if (CurrentPlatform._loadsTruck
                || _facing != CurrentPlatform._passToConveyor._travellingTo)
            {
                TurnWorker();
                // Turn the worker back to the way they were originally facing
                StartCoroutine(TurnWorkerAfterPassingParcel(0.25f, CurrentPlatform));
            }               

			// If the worker is on the platform that loads the truck
			if (CurrentPlatform._loadsTruck)
			{
				LoadParcelOntoTruck(parcel);
			}
			else
			{
				PassParcelToNextConveyor(parcel);
			}                            
		}
	}

    // Turns the worker after a specified number of seconds delay
    IEnumerator TurnWorkerAfterPassingParcel(float seconds, PlatformController currentPlatformWhenTurned)
    {
        yield return new WaitForSeconds(seconds);

        // Only turn the worker back if they are still on the same platform
        if (currentPlatformWhenTurned == CurrentPlatform)
        {
            TurnWorker();
        }
    }

	// Loads a parcel onto the truck
	void LoadParcelOntoTruck(GameObject parcel)
	{
		// Destroy the parcel object
		Destroy (parcel);

		// Increment the number of parcels loaded
		ParcelsLoaded ++;

		// Set the state of the parcel to loaded and raise game event
        parcel.GetComponent<ParcelController>().State = ParcelState.Loaded;
        EventManager.Instance.TriggerParcelLoaded(parcel);
	}

	// Passes a parcel to the next conveyor relevant to the current platform the 
	// worker is standing on
	void PassParcelToNextConveyor(GameObject parcel)
	{
		// Move the parcel to the next conveyor
        MoveParcelToDestinationConveyor(parcel);

        // Set parcel to travelling again
        parcel.GetComponent<ParcelController>().State = ParcelState.Travelling;

        // Raise event to say a parcel has been loaded
        EventManager.Instance.TriggerParcelPassed(parcel);
	}    

    // Moves the parcel to the destination conveyor for the current platfrom this worker is stood on
    void MoveParcelToDestinationConveyor(GameObject parcel)
    {
        // Get the conveyor belt that can be passed to for the platform the worker is 
        // standing on
        GameObject passToConveyor = CurrentPlatform._passToConveyor.gameObject;

        // Pass the parcel onto the pass to conveyor
        // ... work out a bunch of values to help us work out where to put the parcel
        float passToConveyorPositionX = passToConveyor.transform.position.x;
        float passToConveyorPositionY = passToConveyor.transform.position.y;
        float passToConveyorWidth = passToConveyor.GetComponent<Collider2D>().bounds.size.x; // Work to the collider incase of offset
        float passToConveyorHeight = passToConveyor.GetComponent<Collider2D>().bounds.size.y; // Work to the collider incase of offset
        float parcelWidth = parcel.GetComponent<Renderer>().bounds.size.x;
        float parcelHeight = parcel.GetComponent<Renderer>().bounds.size.y;
        // Destination X is...
        float destinationX = 0;
        // Flip the calculations depending on the side of the screen
        if (_playingSide == ScreenSide.Left)
        {
            destinationX = passToConveyorPositionX // position of the pass to conveyor
                - (passToConveyorWidth / 2) // minus half the conveyor width
                    + (parcelWidth / 2); // plus half the parcels width
        }
        else
        {
            destinationX = passToConveyorPositionX // position of the pass to conveyor
                + (passToConveyorWidth / 2) // plus half the conveyor width
                    - (parcelWidth / 2); // minus half the parcels width
        }
        // Destination Y is...
        float destinationY = passToConveyorPositionY // position of the pass to conveyor
            + (passToConveyorHeight / 2) // plus half the conveyor height
                + (parcelHeight / 2); // plus half the parcels height

        // Jump to the new position
        parcel.transform.position = new Vector3(destinationX, destinationY);
    }

	// Moves the worker to the specified platform
    void MoveWorker(PlatformLevel moveToLevel)
    {
        // We need to find the platform that we are jumping too...
        GameObject destinationPlatform = null;

        // Iterate over all platforms in the game
        foreach (PlatformController platform in GameObject.FindObjectsOfType<PlatformController>())
        {
            // Find the middle platform which is on the workers screen side
            if (platform._screenSide == _playingSide && platform._platformLevel == moveToLevel)
            {
                destinationPlatform = platform.gameObject;
				break;
            }
        }

        // Make the worker jump to the platform...

        // ... work out a bunch of values to help us work out where to jump to
		float colliderOffset = ((BoxCollider2D)destinationPlatform.GetComponent<Collider2D>()).offset.y; 
        float destinationPlatformPositionY = destinationPlatform.transform.position.y;
		float destinationPlatformHeight = destinationPlatform.GetComponent<Collider2D>().bounds.size.y; // Work to the collider incase of offset
		float workerHeight = GetComponent<Renderer>().bounds.size.y;
        // Destination Y is...
		float destinationY = destinationPlatformPositionY // position of the destination platform (which is centre of the Game Object)
						+ colliderOffset // The offset of the collider from the game object
						+ (destinationPlatformHeight / 2) // plus half the height
						+ (workerHeight / 2) // plus half the workers height
						+ (Jumping ? Constants.Worker.jumpySkippyness : 0); // Plus a little extra to make it a little skippy jump
		// Destination X is the current X position - we don't move the worker horizontally
		float destinationX = transform.position.x;

		// Set the animator to jumping
        if (Jumping)
        {
            _animator.SetBool("jump", true);
        }
		
        // Jump to the new position
        transform.position = new Vector3 (destinationX, destinationY);

        // Set the current platform
		CurrentPlatform = destinationPlatform.GetComponent<PlatformController>();
    }

    #endregion
}
