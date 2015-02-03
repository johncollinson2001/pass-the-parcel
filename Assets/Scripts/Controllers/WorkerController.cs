using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorkerController : MonoBehaviour
{
    private GameObject _currentPlatform;
    private bool _jumping;
    private Animator _animator;

    public GameObject _startingPlatform;
    public ScreenSide _playingSide;
    public ScreenSide _facing;

	public int ParcelsLoaded { get; set; }
    public bool Active { get; set; } 

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
        EventManager.Instance.GameStateChanged += OnGameOver;
    }

    void OnDisable()
    {
        EventManager.Instance.ParcelDropped -= PassTheParcel;
        EventManager.Instance.GameStateChanged -= OnGameOver;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player has landed on a platform
        if (collision.gameObject.tag == Tags.workerPlatform)
        {
            // Turn the jumping animation off
            _animator.SetBool("jump", false);
        }
    }

    #endregion

    #region Public Methods

    // Moves the worker up
    public void MoveWorkerUp()
    {
        _jumping = true;

        switch (_currentPlatform.GetComponent<PlatformController>()._platformLevel)
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

        _jumping = false;
    }

    // Moves the worker down
    public void MoveWorkerDown()
    {
        _jumping = true;

        switch (_currentPlatform.GetComponent<PlatformController>()._platformLevel)
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

        _jumping = false;
    }

    // Resets the worker
    public void Reset()
    {       
        // Reset animation
        ResetAnimation();

        // Reset the current platform back to starting platform
        _currentPlatform = _startingPlatform;

        // Move the work to the current platform
        MoveWorker(_currentPlatform.GetComponent<PlatformController>()._platformLevel);
    }

    #endregion

    #region Private Methods

    // Turns the worker to face the receiving conveyor belt of the platform he's standing on
    void TurnWorkerToFaceConveyorBelt()
    {
        // Check if we need to turn the worker round to face the receiving from platform
        if (_facing == ScreenSide.Left
            && _currentPlatform.GetComponent<PlatformController>()._receiveFromConveyor.GetComponent<ConveyorBeltController>()._travellingTo == ScreenSide.Left)
        {
            TurnWorker();
        }
        else if (_facing == ScreenSide.Right
            && _currentPlatform.GetComponent<PlatformController>()._receiveFromConveyor.GetComponent<ConveyorBeltController>()._travellingTo == ScreenSide.Right)
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
            _animator.SetBool("walk", false);
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
    void OnGameOver(GameState changedFrom, GameState changedTo)
    {
        // Look for state changing to game over
        if(changedTo == GameState.GameOver)
        {
            // Make the player die animation display
            _animator.SetBool("dead", true);
        }
    }

    // Passes the parecel along the conveyor belts
    void PassTheParcel(GameObject conveyorBelt, GameObject parcel)
	{
		// Check to see if the parcel is still falling
		if (parcel.GetComponent<ParcelController>().IsFalling) 
		{
			// Get the conveyor belt that can be received from for the platform the worker is standing on
			GameObject receiveFromConveyor = _currentPlatform.GetComponent<PlatformController>()._receiveFromConveyor;            

			// If the conveyor belt that the parcel is falling from is the conveyor belt
			// that can be received from
			if (conveyorBelt == receiveFromConveyor)
			{                
                // Set the animation of the sprite
                _animator.SetBool("passing", true);

                // Get the conveyor belt that can be passed to from the platform the worker is standing on
                GameObject passToConveyor = _currentPlatform.GetComponent<PlatformController>()._passToConveyor;

                // Turn the worker so they pass the parcel from one side of the platform to the other
                // if the platforms conveyors are setup to pass from one side to another
                if (_facing == ScreenSide.Right 
                    && receiveFromConveyor.GetComponent<ConveyorBeltController>()._travellingTo == ScreenSide.Left
                    && (passToConveyor != null && passToConveyor.GetComponent<ConveyorBeltController>()._travellingTo == ScreenSide.Left))
                {
                    TurnWorker();
                    // Turn the worker back to the way they were originally facing
                    StartCoroutine(TurnWorkerAfterPassingParcel(0.25f));
                }               

				// If the worker is on the platform that loads the truck
				if (_currentPlatform.GetComponent<PlatformController>()._loadsTruck)
				{
					// Load the parcel onto the truck
					LoadParcelOntoTruck(parcel);
				}
				else
				{
					// else move the parcel to the next conveyor
					PassParcelToNextConveyor(parcel);
				}    
            
                // Set parcel to passed
                parcel.GetComponent<ParcelController>().Pass();
			}
		}
	}

    // Turns the worker after a specified number of seconds delay
    IEnumerator TurnWorkerAfterPassingParcel(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        _animator.SetBool("passing", false);
        TurnWorker();
    }

	// Loads a parcel onto the truck
	void LoadParcelOntoTruck(GameObject parcel)
	{
		// Destroy the parcel object
		Destroy (parcel);

		// Increment the number of parcels loaded
		ParcelsLoaded ++;

		// Raise event to say a parcel has been loaded
        EventManager.Instance.TriggerParcelLoaded();
	}

	// Passes a parcel to the next conveyor relevant to the current platform the 
	// worker is standing on
	void PassParcelToNextConveyor(GameObject parcel)
	{
		// Get the conveyor belt that can be passed to for the platform the worker is 
		// standing on
		GameObject passToConveyor = 
			_currentPlatform.GetComponent<PlatformController>()._passToConveyor;
		
		// Pass the parcel onto the pass to conveyor
		// ... work out a bunch of values to help us work out where to put the parcel
		float passToConveyorPositionX = passToConveyor.transform.position.x;
		float passToConveyorPositionY = passToConveyor.transform.position.y;
		float passToConveyorWidth = passToConveyor.collider2D.bounds.size.x; // Work to the collider incase of offset
		float passToConveyorHeight = passToConveyor.collider2D.bounds.size.y; // Work to the collider incase of offset
		float parcelWidth = parcel.renderer.bounds.size.x;
		float parcelHeight = parcel.renderer.bounds.size.y;
		// Destination X is...
		float destinationX = 0;
		// Flip the calculations depending on the side of the screen
		if(_playingSide == ScreenSide.Left)
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
		parcel.transform.position = new Vector3 (destinationX, destinationY);
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
            if (platform._screenSide == _playingSide
                && platform._platformLevel == moveToLevel)
            {
                destinationPlatform = platform.gameObject;
				break;
            }
        }

        // Make the worker jump to the platform...

        // ... work out a bunch of values to help us work out where to jump to
		float colliderOffset = ((BoxCollider2D)destinationPlatform.collider2D).center.y; 
        float destinationPlatformPositionY = destinationPlatform.transform.position.y;
		float destinationPlatformHeight = destinationPlatform.collider2D.bounds.size.y; // Work to the collider incase of offset
		float workerHeight = renderer.bounds.size.y;
        // Destination Y is...
		float destinationY = destinationPlatformPositionY // position of the destination platform (which is centre of the Game Object)
						+ colliderOffset // The offset of the collider from the game object
						+ (destinationPlatformHeight / 2) // plus half the height
						+ (workerHeight / 2) // plus half the workers height
						+ (_jumping ? Defaults.Worker.jumpySkippyness : 0); // Plus a little extra to make it a little skippy jump
		// Destination X is the current X position - we don't move the worker horizontally
		float destinationX = transform.position.x;

		// Set the animator to jumping
        if (_jumping)
        {
            _animator.SetBool("jump", true);
        }
		
        // Jump to the new position
        transform.position = new Vector3 (destinationX, destinationY);

        // Set the current platform
		_currentPlatform = destinationPlatform;
    }

    #endregion
}
